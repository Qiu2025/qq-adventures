using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// Sistema ligero para grabar múltiples runs, guardarlas/leerlas desde disco por nombre
/// y reproducir una grabación instanciando un prefab visual (ghostObj).
/// Esta versión intenta auto-cargar desde persistentDataPath si la run no está en memoria,
/// y si no existe intenta cargar asíncronamente desde StreamingAssets ("ghosts/<runName>.ghost").
/// </summary>
public class ReplaySystem
{
    private readonly WaitForFixedUpdate _wait = new WaitForFixedUpdate();
    private readonly MonoBehaviour _runner;

    public ReplaySystem(MonoBehaviour runner)
    {
        _runner = runner;
        // arrancamos coroutines para muestreo y reproducción
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
            // en FixedUpdate incrementamos con fixedDeltaTime
            _elapsedRecordingTime += Time.fixedDeltaTime;
        }
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            yield return null;
            // para playback usamos deltaTime
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

    /// <summary>
    /// Comienza una grabación para ese transform
    /// </summary>
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

    /// <summary>
    /// Finaliza la grabación actual y la guarda en memoria bajo el nombre runName.
    /// Si quieres también la guarda en disco (persistentDataPath) cuando saveToFile == true.
    /// </summary>
    public bool FinishRunAndSave(string runName, bool saveToFile = true)
    {
        if (_currentRun == null) return false;

        _runs[runName] = _currentRun;
        if (saveToFile) SaveRunToFile(runName);
        _currentRun = null;
        return true;
    }

    /// <summary>
    /// Guarda la grabación con nombre runName a Application.persistentDataPath/runName.ghost
    /// </summary>
    public bool SaveRunToFile(string runName)
    {
        if (!_runs.TryGetValue(runName, out var run)) return false;
        try
        {
            string path = Path.Combine(Application.persistentDataPath, runName + ".ghost");
            File.WriteAllText(path, run.Serialize());
            Debug.Log($"Saved run '{runName}' -> {path}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save run: " + ex);
            return false;
        }
    }

    /// <summary>
    /// Carga un archivo .ghost desde StreamingAssets (o desde persistentDataPath si prefieres).
    /// - relativePathInStreamingAssets: ejemplo "ghosts/mi_grabacion.ghost"
    /// - runName: nombre con el que quedará registrado en _runs (si null, se usa el filename sin extensión)
    /// - onComplete: callback opcional (bool success)
    /// </summary>
    public void LoadRunFromStreamingAssets(string relativePathInStreamingAssets, string runName = null, System.Action<bool> onComplete = null)
    {
        // arrancamos la coroutine desde el MonoBehaviour que se pasó al constructor
        _runner.StartCoroutine(LoadRunCoroutine(relativePathInStreamingAssets, runName, onComplete));
    }

    private IEnumerator LoadRunCoroutine(string relativePathInStreamingAssets, string runName, System.Action<bool> onComplete)
    {
        // Nombre final que usaremos en el diccionario
        string fileName = System.IO.Path.GetFileName(relativePathInStreamingAssets);
        string keyName = string.IsNullOrEmpty(runName) ? System.IO.Path.GetFileNameWithoutExtension(fileName) : runName;

        // Construimos la ruta dentro de streaming assets
        string streamingPath = System.IO.Path.Combine(Application.streamingAssetsPath, relativePathInStreamingAssets);

#if UNITY_WEBGL && !UNITY_EDITOR
        // En WebGL streamingPath será una URL; usamos UnityWebRequest
        using (UnityWebRequest uwr = UnityWebRequest.Get(streamingPath))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string data = uwr.downloadHandler.text;
                try
                {
                    var run = new Recording(data);
                    _runs[keyName] = run;
                    Debug.Log($"Loaded run '{keyName}' from StreamingAssets (WebGL).");
                    onComplete?.Invoke(true);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Failed to parse run data: " + ex);
                    onComplete?.Invoke(false);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to load streaming asset (WebGL): {streamingPath}  error: {uwr.error}");
                onComplete?.Invoke(false);
            }
        }
#else
        // En Editor / Standalone podemos leer directamente con File.ReadAllText
        if (System.IO.File.Exists(streamingPath))
        {
            string data = System.IO.File.ReadAllText(streamingPath);
            try
            {
                var run = new Recording(data);
                _runs[keyName] = run;
                Debug.Log($"Loaded run '{keyName}' from StreamingAssets (path: {streamingPath}).");
                onComplete?.Invoke(true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to parse run data: " + ex);
                onComplete?.Invoke(false);
            }
        }
        else
        {
            Debug.LogWarning("Streaming asset not found at: " + streamingPath);
            onComplete?.Invoke(false);
        }
        yield return null;
#endif
    }

    /// <summary>
    /// Carga sincrona desde una ruta absoluta o (si no es absoluta) desde Application.persistentDataPath.
    /// Registra la run en _runs con runName (o filename sin ext) y devuelve true si tuvo éxito.
    /// </summary>
    public bool LoadRunFromFile(string filePathOrName, string runName = null)
    {
        string path = filePathOrName;
        if (!Path.IsPathRooted(path)) path = Path.Combine(Application.persistentDataPath, filePathOrName);

        if (!File.Exists(path))
        {
            // no existe en disco
            return false;
        }

        try
        {
            string data = File.ReadAllText(path);
            var run = new Recording(data);
            string key = string.IsNullOrEmpty(runName) ? Path.GetFileNameWithoutExtension(path) : runName;
            _runs[key] = run;
            Debug.Log($"Loaded run '{key}' from file: {path}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load run from file: " + ex);
            return false;
        }
    }

    /// <summary>
    /// Recupera una run en memoria
    /// </summary>
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


    /// <summary>
    /// Reproduce la run con nombre runName. Devuelve true si la reproducción comenzó correctamente.
    /// Debes pasar el GameObject visual ya instanciado (prefab instanciado).
    /// Si la run no está en memoria intentará:
    ///  - cargar sincrónicamente desde persistentDataPath/runName.ghost
    ///  - si falla, lanzará una carga asíncrona desde StreamingAssets/ghosts/runName.ghost y reproducirá cuando termine.
    /// </summary>
    public bool PlayRecording(string runName, GameObject ghostObj, bool destroyOnCompletion = true)
    {
        if (_ghostObj != null) Object.Destroy(_ghostObj);

        // 1) Si ya está en memoria, reproducir inmediatamente
        if (GetRun(runName, out _currentReplay))
        {
            StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
            return true;
        }

        // 2) Intentar cargar sincronamente desde persistentDataPath (runName.ghost)
        string candidateFileName = runName + ".ghost";
        if (LoadRunFromFile(candidateFileName, runName))
        {
            if (GetRun(runName, out _currentReplay))
            {
                StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
                return true;
            }
        }

        // 3) Intentar cargar asíncronamente desde StreamingAssets ("ghosts/<runName>.ghost")
        // Iniciamos una coroutine que cargará y, cuando termine, pondrá a reproducir o destruirá el ghost si falla.
        _runner.StartCoroutine(LoadFromStreamingAndPlayCoroutine(runName, ghostObj, destroyOnCompletion));
        // devolvemos true porque hemos iniciado la operación asíncrona;
        // si falla más tarde el coroutine destruye el ghost.
        return true;
    }


    private void StartPlaybackWithGhost(GameObject ghostObj, bool destroyOnCompletion)
    {
        _replaySmoothedTime = 0f;
        _destroyOnComplete = destroyOnCompletion;
        _ghostObj = ghostObj;

        // Poner el ghost en la primera posición para evitar saltos
        var pose = _currentReplay.EvaluatePoint(0f);
        _ghostObj.transform.SetPositionAndRotation(pose.position, pose.rotation);

        // Limpiar TrailRenderer si lo hay (evitar residuos)
        var trail = _ghostObj.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            // Clear existe en versiones modernas de Unity
            trail.Clear();
            // asegurar que emite
            // trail.emitting = true;
        }

        // Aseguramos que el ghost no interfiera físicamente
        var col = _ghostObj.GetComponent<Collider>();
        if (col) col.enabled = false;
        var col2 = _ghostObj.GetComponent<Collider2D>();
        if (col2) col2.enabled = false;
        var rb = _ghostObj.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
        var rb2 = _ghostObj.GetComponent<Rigidbody2D>();
        if (rb2) rb2.bodyType = RigidbodyType2D.Kinematic;

        // Intentar obtener Animator del ghost (para aplicar parámetros en UpdateReplay)
        _ghostAnimator = _ghostObj.GetComponent<Animator>();
    }


    private IEnumerator LoadFromStreamingAndPlayCoroutine(string runName, GameObject ghostObj, bool destroyOnCompletion)
    {
        // Intentaremos "ghosts/<runName>.ghost" dentro de StreamingAssets
        string relative = Path.Combine("ghosts", runName + ".ghost");
        bool completed = false;
        bool success = false;

        // Llamamos al loader existente que usa una coroutine interna y callback
        LoadRunFromStreamingAssets(relative, runName, (ok) => { success = ok; completed = true; });

        // Esperamos a que termine la carga
        yield return new WaitUntil(() => completed);

        if (!success)
        {
            Debug.LogWarning($"PlayRecording: run '{runName}' not found in memory, persistentDataPath or StreamingAssets.");
            if (destroyOnCompletion && ghostObj != null) Object.Destroy(ghostObj);
            yield break;
        }

        // Si la carga tuvo éxito, la run ya está registrada en _runs
        if (!GetRun(runName, out _currentReplay))
        {
            Debug.LogWarning($"PlayRecording: loaded run '{runName}' but failed to retrieve it from memory.");
            if (destroyOnCompletion && ghostObj != null) Object.Destroy(ghostObj);
            yield break;
        }

        // Iniciamos la reproducción usando el mismo ghostObj
        StartPlaybackWithGhost(ghostObj, destroyOnCompletion);
    }
    private void UpdateReplay()
    {
        if (_currentReplay == null || _ghostObj == null) return;

        var pose = _currentReplay.EvaluatePoint(_replaySmoothedTime);
        _ghostObj.transform.SetPositionAndRotation(pose.position, pose.rotation);

        // Aplicar parámetros del Animator (si existen)
        if (_ghostAnimator != null)
        {
            // Aplicar floats
            // Los nombres de parámetros disponibles están dentro de _currentReplay (si se grabaron)
            // Para simplicidad, iteramos sobre las curvas almacenadas por reflection-like (Recording no expone listas),
            // pero como Recording no expone sus nombres, añadimos dos métodos TryEvaluate... que ya están implementadas.
            // Como no tenemos la lista de nombres aquí, podemos mantener una copia en memoria:
            // -> solución: cuando se cargue la run (en LoadRunFromFile o LoadRunFromStreamingAssets),
            //    guardamos la parameter lists asociadas en un diccionario. Para no complicar, aprovechamos que Recording
            //    mantiene internamente las curvas y hemos expuesto TryEvaluate* APIs que necesitan el nombre.
            // -> Por simplicidad práctica aquí, asumimos que el ghost Animator tiene los mismos parámetros y
            //    que el usuario conoce qué parámetros quiere reproducir: fallback: si Recording contiene curves,
            //    necesitamos exponer las names. Para no romper muchas cosas, vamos a intentar aplicar los parámetros
            //    guardados iterando sobre a lista de parámetros que existan en el Animator itself:
            var paramsInfo = _ghostAnimator.parameters;
            foreach (var p in paramsInfo)
            {
                string paramName = p.name;
                if (p.type == AnimatorControllerParameterType.Float)
                {
                    if (_currentReplay.TryEvaluateFloatParam(paramName, _replaySmoothedTime, out float fv))
                    {
                        _ghostAnimator.SetFloat(paramName, fv);
                    }
                }
                else if (p.type == AnimatorControllerParameterType.Bool)
                {
                    if (_currentReplay.TryEvaluateBoolParam(paramName, _replaySmoothedTime, out bool bv))
                    {
                        _ghostAnimator.SetBool(paramName, bv);
                    }
                }
                // Triggers not handled here (would need edge detection)
            }
        }

        // Actualizar TrailRenderer
        var trail = _ghostObj.GetComponent<TrailRenderer>();
        if(trail != null)
            trail.emitting = _currentReplay.GetTrailActiveAt(_replaySmoothedTime);

        if (_replaySmoothedTime > _currentReplay.Duration)
        {
            _currentReplay = null;
            if (_destroyOnComplete && _ghostObj != null) Object.Destroy(_ghostObj);
            _ghostObj = null;
            _ghostAnimator = null;
        }
    }


    /// <summary>
    /// Detiene la reproducción actual y destruye el objeto visual (si existe).
    /// </summary>
    public void StopReplay()
    {
        if (_ghostObj != null) Object.Destroy(_ghostObj);
        _ghostObj = null;
        _currentReplay = null;
    }

    #endregion
}