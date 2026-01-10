using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class ReplaySystem
{
    private readonly WaitForFixedUpdate _wait = new WaitForFixedUpdate();
    private readonly MonoBehaviour _runner;

    public ReplaySystem(MonoBehaviour runner)
    {
        _runner = runner;
        runner.StartCoroutine(FixedUpdateCoroutine());
        runner.StartCoroutine(UpdateCoroutine());
    }

    #region Coroutines (timing)

    private IEnumerator FixedUpdateCoroutine()
    {
        while (true)
        {
            yield return _wait;
            AddSnapshot();
            _elapsedRecordingTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            yield return null;
            _replaySmoothedTime += Time.deltaTime;
            UpdateReplay();
        }
    }

    #endregion

    #region Recording state

    private readonly Dictionary<string, Recording> _runs = new Dictionary<string, Recording>();
    private Recording _currentRun;
    private float _elapsedRecordingTime;
    private int _snapshotEveryNFrames = 1;
    private int _frameCount;
    private float _maxRecordingTimeLimit = 60f;

    public void StartRun(Transform target, int snapshotEveryNFrames = 1, float maxRecordingTimeLimit = 60f, string[] floatParamNames = null, string[] boolParamNames = null)
    {
        _currentRun = new Recording(target, floatParamNames, boolParamNames);
        _elapsedRecordingTime = 0f;
        _snapshotEveryNFrames = Mathf.Max(1, snapshotEveryNFrames);
        _frameCount = 0;
        _maxRecordingTimeLimit = maxRecordingTimeLimit;
    }

    private void AddSnapshot()
    {
        if (_currentRun == null) return;
        if (_frameCount++ % _snapshotEveryNFrames == 0)
        {
            bool trailActive = false;
            var player = _currentRun.Target.GetComponent<PlayerMovement>();
            if (player != null) trailActive = player.isDashing;
            _currentRun.AddSnapshot(_elapsedRecordingTime, trailActive);
        }
        if (_currentRun.Duration >= _maxRecordingTimeLimit)
        {
            FinishRunAndSave("auto_run_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }
    }

    public bool FinishRunAndSave(string runName, bool saveToFile = true)
    {
        if (_currentRun == null) return false;
        _runs[runName] = _currentRun;
        if (saveToFile) SaveRunToFile(runName);
        _currentRun = null;
        return true;
    }

    public bool SaveRunToFile(string runName)
    {
        if (!_runs.TryGetValue(runName, out var run)) return false;
        try
        {
            string path = Path.Combine(Application.persistentDataPath, runName + ".ghost");
            File.WriteAllText(path, run.Serialize());
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save run: " + ex);
            return false;
        }
    }

    public void LoadRunFromStreamingAssets(string relativePathInStreamingAssets, string runName = null, System.Action<bool> onComplete = null)
    {
        _runner.StartCoroutine(LoadRunCoroutine(relativePathInStreamingAssets, runName, onComplete));
    }

    private IEnumerator LoadRunCoroutine(string relativePathInStreamingAssets, string runName, System.Action<bool> onComplete)
    {
        string fileName = System.IO.Path.GetFileName(relativePathInStreamingAssets);
        string keyName = string.IsNullOrEmpty(runName) ? System.IO.Path.GetFileNameWithoutExtension(fileName) : runName;
        string streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePathInStreamingAssets);

#if UNITY_WEBGL && !UNITY_EDITOR
        using (UnityWebRequest uwr = UnityWebRequest.Get(streamingPath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var run = new Recording(uwr.downloadHandler.text);
                    _runs[keyName] = run;
                    onComplete?.Invoke(true);
                }
                catch { onComplete?.Invoke(false); }
            }
            else { onComplete?.Invoke(false); }
        }
#else
        if (System.IO.File.Exists(streamingPath))
        {
            try
            {
                var run = new Recording(System.IO.File.ReadAllText(streamingPath));
                _runs[keyName] = run;
                onComplete?.Invoke(true);
            }
            catch { onComplete?.Invoke(false); }
        }
        else { onComplete?.Invoke(false); }
        yield return null;
#endif
    }

    public bool LoadRunFromFile(string filePathOrName, string runName = null)
    {
        string path = filePathOrName;
        if (!Path.IsPathRooted(path)) path = Path.Combine(Application.persistentDataPath, filePathOrName);
        if (!File.Exists(path)) return false;
        try
        {
            string data = File.ReadAllText(path);
            var run = new Recording(data);
            string key = string.IsNullOrEmpty(runName) ? Path.GetFileNameWithoutExtension(path) : runName;
            _runs[key] = run;
            return true;
        }
        catch { return false; }
    }

    public bool GetRun(string runName, out Recording run)
    {
        return _runs.TryGetValue(runName, out run);
    }

    #endregion

    #region Playback

    private Recording _currentReplay;
    private GameObject _ghostObj;
    private bool _destroyOnComplete = true;
    private float _replaySmoothedTime;
    private Animator _ghostAnimator;

    public bool PlayRecording(string runName, GameObject ghostObj, bool destroyOnCompletion = true)
    {
        if (_ghostObj != null && !_ghostObj.CompareTag("Player")) Object.Destroy(_ghostObj);

        if (GetRun(runName, out _currentReplay))
        {
            StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
            return true;
        }
        
        string candidateFileName = runName + ".ghost";
        if (LoadRunFromFile(candidateFileName, runName))
        {
            if (GetRun(runName, out _currentReplay))
            {
                StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
                return true;
            }
        }

        _runner.StartCoroutine(LoadFromStreamingAndPlayCoroutine(runName, ghostObj, destroyOnCompletion));
        return true;
    }

    private void StartPlaybackWithGhost(GameObject ghostObj, bool destroyOnCompletion)
    {
        _replaySmoothedTime = 0f;
        _destroyOnComplete = destroyOnCompletion;
        _ghostObj = ghostObj;

        var pose = _currentReplay.EvaluatePoint(0f);
        _ghostObj.transform.SetPositionAndRotation(pose.position, pose.rotation);

        var trail = _ghostObj.GetComponent<TrailRenderer>();
        if (trail != null) trail.Clear();

        bool isPlayer = _ghostObj.CompareTag("Player");
        
        var col = _ghostObj.GetComponent<Collider>();
        if (col && !isPlayer) col.enabled = false;
        var col2 = _ghostObj.GetComponent<Collider2D>();
        if (col2 && !isPlayer) col2.enabled = false;
        
        var rb = _ghostObj.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        var rb2 = _ghostObj.GetComponent<Rigidbody2D>();
        if (rb2) rb2.bodyType = RigidbodyType2D.Kinematic;

        _ghostAnimator = _ghostObj.GetComponent<Animator>();
    }

    private IEnumerator LoadFromStreamingAndPlayCoroutine(string runName, GameObject ghostObj, bool destroyOnCompletion)
    {
        string relative = Path.Combine("ghosts", runName + ".ghost");
        bool completed = false;
        bool success = false;

        LoadRunFromStreamingAssets(relative, runName, (ok) => { success = ok; completed = true; });
        yield return new WaitUntil(() => completed);

        if (!success || !GetRun(runName, out _currentReplay))
        {
            if (destroyOnCompletion && ghostObj != null && !ghostObj.CompareTag("Player")) Object.Destroy(ghostObj);
            yield break;
        }

        StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
    }

    private void UpdateReplay()
    {
        if (_currentReplay == null || _ghostObj == null) return;

        var pose = _currentReplay.EvaluatePoint(_replaySmoothedTime);
        _ghostObj.transform.SetPositionAndRotation(pose.position, pose.rotation);

        if (_ghostAnimator != null)
        {
            foreach (var p in _ghostAnimator.parameters)
            {
                if (p.type == AnimatorControllerParameterType.Float && _currentReplay.TryEvaluateFloatParam(p.name, _replaySmoothedTime, out float fv))
                    _ghostAnimator.SetFloat(p.name, fv);
                else if (p.type == AnimatorControllerParameterType.Bool && _currentReplay.TryEvaluateBoolParam(p.name, _replaySmoothedTime, out bool bv))
                    _ghostAnimator.SetBool(p.name, bv);
            }
        }

        var trail = _ghostObj.GetComponent<TrailRenderer>();
        if(trail != null) trail.emitting = _currentReplay.GetTrailActiveAt(_replaySmoothedTime);

        if (_replaySmoothedTime > _currentReplay.Duration)
        {
            _currentReplay = null;
            if (_destroyOnComplete && _ghostObj != null && !_ghostObj.CompareTag("Player")) Object.Destroy(_ghostObj);
            _ghostObj = null;
            _ghostAnimator = null;
        }
    }

    public void StopReplay()
    {
        if (_ghostObj != null && !_ghostObj.CompareTag("Player")) Object.Destroy(_ghostObj);
        _ghostObj = null;
        _currentReplay = null;
    }

    #endregion
}