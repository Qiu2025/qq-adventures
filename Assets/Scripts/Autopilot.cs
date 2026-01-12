using UnityEngine;

public class Autopilot : MonoBehaviour
{
    private bool defaultBoolean = false;

    public void AutoPilotChanged()
    {
        gameObject.SetActive(!defaultBoolean);
    }
}
