using UnityEngine;

/// <summary>
/// Controla grabación (V/B) y reproducción (N/M) de runs.
/// - NO hace auto-load: la reproducción solo ocurre cuando se la pides (PlayRun / PlayRunByName o Trigger).
/// - Distinción DeveloperMode: grabación solo permitida en modo desarrollador/editor.
/// </summary>
public class GhostRunner : MonoBehaviour
{
    [Header("Recording")]
    [SerializeField] private Transform _recordTarget;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 1;
    [SerializeField] private string _runName = "run1"; // nombre para guardar/cargar la grabación

    [Header("Playback")]
    [SerializeField] private GameObject _ghostPrefab;

    [Header("Mode")]
    // true por defecto en Editor, false por defecto en builds
#if UNITY_EDITOR
    [SerializeField] private bool developerMode = true;
#else
    [SerializeField] private bool developerMode = false;
#endif

    private ReplaySystem _system;

    private void Awake()
    {
        // ReplaySystem necesita un MonoBehaviour para iniciar coroutines
        _system = new ReplaySystem(this);
    }

    void Update()
    {
        // Controles de desarrollo / prueba:
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!developerMode)
            {
                Debug.LogWarning("Grabación deshabilitada en este build. Activa developerMode para permitir grabar.");
            }
            else
            {
                if (_recordTarget == null)
                {
                    Debug.LogError("GhostRunner: _recordTarget no asignado.");
                }
                else
                {
                    string[] floatParams = new string[] { };
                    string[] boolParams = new string[] { "isWalking", "isFalling", "isJumping", "grounded", "isDoubleJumping", "isDoingSecondJump" };
                    _system.StartRun(_recordTarget, _captureEveryNFrames, 60f, floatParams, boolParams);
                    Debug.Log("Recording started: " + _runName);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (!developerMode)
            {
                Debug.LogWarning("Guardar grabación deshabilitado en este build.");
            }
            else
            {
                var saved = _system.FinishRunAndSave(_runName);
                Debug.Log($"Record finished. Saved: {saved} -> {Application.persistentDataPath}/{_runName}.ghost");
            }
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            PlayRunByName(_runName);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            _system.StopReplay();
            Debug.Log("Record stopped manually");
        }
    }

    /// <summary>
    /// Reproduce una run concreta (si existe en memoria).
    /// Si no está cargada, devuelve false y destruye el prefab instanciado.
    /// </summary>
    public bool PlayRunByName(string runName)
    {
        if (_ghostPrefab == null)
        {
            Debug.LogError("GhostRunner: ghostPrefab no asignado.");
            return false;
        }

        var ghost = Instantiate(_ghostPrefab);
        bool ok = _system.PlayRecording(runName, ghost);
        if (!ok)
        {
            Debug.LogWarning("No recording found with name: " + runName);
            Destroy(ghost);
            return false;
        }

        Debug.Log("Playing record: " + runName);
        return true;
    }
}
