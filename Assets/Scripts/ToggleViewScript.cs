using UnityEngine;
using UnityEngine.UI;

public class MenuAccesibilidadUI : MonoBehaviour
{
    [SerializeField] private Toggle toggleMasSalto;

    void OnEnable()  
    {
        toggleMasSalto.isOn = AccessibilityManager.Instance.MasSaltos;
    }
}

