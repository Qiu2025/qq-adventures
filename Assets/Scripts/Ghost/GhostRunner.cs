using UnityEngine;

public class GhostRunner : MonoBehaviour
{
    public static GhostRunner Instance { get; private set; }

    // --------------------------------------------- //

    [Header("Recording")]
    [SerializeField] private Transform _recordTarget;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;
    [SerializeField] private string _runName = "run1";

    // --------------------------------------------- //

    [Header("Playback")]
    [SerializeField] private GameObject _ghostPrefab;

    // --------------------------------------------- //

    [Header("Mode")]
#if UNITY_EDITOR
    [SerializeField] private bool _developerMode = true;
#else
    [SerializeField] private bool _developerMode = false;
#endif

    // --------------------------------------------- //

    private ReplaySystem _system;

    string[] floatParams = new string[] { };
    string[] boolParams = new string[] { "isWalking", "isFalling", "isJumping", "grounded", "isDoubleJumping", "isDoingSecondJump", "isFacingRight" };

    // --------------------------------------------- //

    private void Awake()
    {
        _system = new ReplaySystem(this);
        Instance = this;
    }

    void Update()
    {
        if (!_developerMode) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
            _system.StartRun(_recordTarget, _captureEveryNFrames, 60f, floatParams, boolParams);
            Debug.Log("Recording started: " + _runName);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            var saved = _system.FinishRunAndSave(_runName);
            Debug.Log($"Record finished. Saved: {saved}");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            PlayRunByName(_runName);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            StopReplayManual();
        }
    }

    public GameObject PlayRunByName(string runName)
    {
        var ghost = Instantiate(_ghostPrefab);
        if (!_system.PlayRecording(runName, ghost))
        {
            Destroy(ghost);
            return null;
        }
        return ghost;
    }

    public bool PlayRunOnExistingTarget(string runName, GameObject target)
    {
        return _system.PlayRecording(runName, target, false);
    }

    public void StopReplayManual()
    {
        _system.StopReplay();
    }

    public float GetRunDuration(string runName)
    {
        if (_system.GetRun(runName, out var run)) return run.Duration;
        return 0f;
    }
}