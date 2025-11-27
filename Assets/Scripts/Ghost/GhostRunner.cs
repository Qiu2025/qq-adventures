using UnityEngine;

public class GhostRunner : MonoBehaviour
{
    [SerializeField] private Transform _recordTarget;
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField, Range(1, 10)] private int _captureEveryNFrames = 1;

    private ReplaySystem _system;
    private void Awake() => _system = new ReplaySystem(this);

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            _system.StartRun(_recordTarget, _captureEveryNFrames);
            Debug.Log("Recording");
        } else if(Input.GetKeyDown(KeyCode.B))
        {
            _system.FinishRun();
            Debug.Log("Record finished");
        } else if (Input.GetKeyDown(KeyCode.N))
        {
            _system.PlayRecording(RecordingType.Last, Instantiate(_ghostPrefab));
            Debug.Log("Playing record");
        } else if(Input.GetKeyDown(KeyCode.M))
        {
            _system.StopReplay();
            Debug.Log("Record stopped manually");
        }
    }
}