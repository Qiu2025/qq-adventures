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
        if(Input.GetKeyDown(KeyCode.N))
        {
            _system.StartRun(_recordTarget, _captureEveryNFrames);
            _system.PlayRecording(RecordingType.Best, Instantiate(_ghostPrefab));
        } else if(Input.GetKeyDown(KeyCode.M))
        {
            _system.FinishRun();
            _system.StopReplay();
        }
    }
}