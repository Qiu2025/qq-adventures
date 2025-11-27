using UnityEngine;

/// <summary>
/// Controla grabación (V/B) y reproducción (N/M) de runs.
/// - NO hace auto-load: para reproducir, llamar a PlayRunByName.
/// - Distinción DeveloperMode: grabación solo permitida en modo desarrollador/editor.
/// </summary>
public class GhostRunner : MonoBehaviour
{
    // --------------------------------------------- //

    [Header("Recording")]
    [SerializeField] private Transform _recordTarget;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;
    [SerializeField] private string _runName = "run1"; // nombre para guardar/cargar la grabación

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

    // Variables reales y booleanos del animator para ser grabado
    string[] floatParams = new string[] { };
    string[] boolParams = new string[] { "isWalking", "isFalling", "isJumping", "grounded", "isDoubleJumping", "isDoingSecondJump", "isFacingRight" };

    // --------------------------------------------- //

    private void Awake()
    {
        _system = new ReplaySystem(this);
    }

    void Update()
    {
        if (!_developerMode)
        {
            return; // No se permite el acceso a la funcionalidad ghost si no se está accediendo desde unity
        }

        // Controles de desarrollo / prueba
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Grabar un run
            _system.StartRun(_recordTarget, _captureEveryNFrames, 60f, floatParams, boolParams);
            Debug.Log("Recording started: " + _runName);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            // Terminar grabacion y guardar
            var saved = _system.FinishRunAndSave(_runName);
            Debug.Log($"Record finished. Saved: {saved} -> {Application.persistentDataPath}/{_runName}.ghost");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // Reproducir un run
            PlayRunByName(_runName);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            // Parar el run manualmente
            _system.StopReplay();
            Debug.Log("Record stopped manually");
        }
    }

    /// <summary>
    /// Reproduce una run concreta
    /// Si no existe, devuelve false y destruye el prefab instanciado.
    /// </summary>
    public bool PlayRunByName(string runName)
    {
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