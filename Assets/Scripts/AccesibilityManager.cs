using System;
using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance { get; private set; }

    public event Action OnChanged;
    public event Action OnChangedSlowMo;
    public event Action OnChangedAutopilot;
    [Header("Jump max")]
    [SerializeField] private int jumpsNormal = 2;
    [SerializeField] private int jumpsAccesibilidad = 4;
    public bool MasSaltos { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        MasSaltos = PlayerPrefs.GetInt("max_saltos", 0) == 1;
    }

    public int GetMaxJumps()
    {
        return MasSaltos ? jumpsAccesibilidad : jumpsNormal;
    }

    public void SetMasSaltos(bool activo)
    {
        Debug.Log("TOGGLE MasSaltos -> " + activo);
        MasSaltos = activo;
        PlayerPrefs.SetInt("max_saltos", activo ? 1 : 0);

        Debug.Log("Lanzando evento OnChanged");
        OnChanged?.Invoke();
    }

    public void SetCamaraLenta(bool activo)
    {
        OnChangedSlowMo?.Invoke();
    }

    public void SetAutopilot(bool activo)
    {
        OnChangedAutopilot?.Invoke();
    }
}
