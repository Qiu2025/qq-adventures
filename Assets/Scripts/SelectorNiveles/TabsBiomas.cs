using UnityEngine;

public class TabsBiomas : MonoBehaviour
{
    [Header("Fondos/Biomas")]
    public GameObject mapDesierto;
    public GameObject mapBosque;
    public GameObject mapPlaya;
    public GameObject mapArtico;

    [Header("UI")]
    public GameObject levelIconsDesierto; // el contenedor de los niveles del desierto
    public GameObject lockedPanel;        // pingüino NO DISPONIBLE

    private void Start()
    {
        ShowDesierto(); // por defecto
    }

    private void HideAllMaps()
    {
        mapDesierto.SetActive(false);
        mapBosque.SetActive(false);
        mapPlaya.SetActive(false);
        mapArtico.SetActive(false);
    }

    public void ShowDesierto()
    {
        HideAllMaps();

        mapDesierto.SetActive(true);

        if (levelIconsDesierto != null) levelIconsDesierto.SetActive(true);
        if (lockedPanel != null) lockedPanel.SetActive(false);
    }

    public void ShowBosque()
    {
        HideAllMaps();

        mapBosque.SetActive(true);

        if (levelIconsDesierto != null) levelIconsDesierto.SetActive(false);
        if (lockedPanel != null) lockedPanel.SetActive(true);
    }

    public void ShowPlaya()
    {
        HideAllMaps();

        mapPlaya.SetActive(true);

        if (levelIconsDesierto != null) levelIconsDesierto.SetActive(false);
        if (lockedPanel != null) lockedPanel.SetActive(true);
    }

    public void ShowArtico()
    {
        HideAllMaps();

        mapArtico.SetActive(true);

        if (levelIconsDesierto != null) levelIconsDesierto.SetActive(false);
        if (lockedPanel != null) lockedPanel.SetActive(true);
    }
}
