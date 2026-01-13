using UnityEngine;

// Script que controla la grabacion y reproduccion de runs
// - Para debug, V/B para grabar y N/M para reproducir
// - Dichos controles solo estan permitidos en modo developer, es decir, en Unity Editor
// - Para reproducir llama a PlayRunByName, o sea que es necesario tener un archivo 
//   x.ghost siendo x el nombre del run para reproducir
public class GhostRunner : MonoBehaviour
{
    public static GhostRunner Instance { get; private set; }

    // --------------------------------------------- //

    [Header("Recording")]
    [SerializeField] private Transform _recordTarget;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 2;
    [SerializeField] private string _runName = "run1"; // nombre para guardar/cargar la grabación

    // --------------------------------------------- //

    [Header("Playback")]
    [SerializeField] private GameObject _ghostPrefab;

    // --------------------------------------------- //

    private ReplaySystem _system;

    // Variables reales y booleanos del animator para ser grabado
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
#if UNITY_EDITOR
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
            StopReplayManual();
            Debug.Log("Record stopped manually");
        }
#endif
    }

    // Reproduce una run concreta
    // Si no existe, devuelve null y destruye el prefab instanciado.
    public GameObject PlayRunByName(string runName)
    {
        var ghost = Instantiate(_ghostPrefab);
        bool ok = _system.PlayRecording(runName, ghost);

        if (!ok)
        {
            Debug.LogWarning("No recording found with name: " + runName);
            Destroy(ghost);
            return null;
        }

        Debug.Log("Playing record: " + runName);
        return ghost;
    }

    //  Metodos para autopiloto
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