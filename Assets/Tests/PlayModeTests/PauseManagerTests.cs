using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PauseManagerTests
{
    private GameObject pauseManagerGO;
    private PauseManager pauseManager;

    private GameObject pauseMenu;
    private GameObject panelMenu;
    private GameObject panelOpciones;
    private GameObject panelSonido;
    private GameObject panelControles;
    private GameObject panelAccesibilidad;

    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Time.timeScale = 1f;

        // ---------------- Panels ----------------
        pauseMenu = new GameObject("PauseMenu");
        toDestroy.Add(pauseMenu);

        panelMenu = new GameObject("PanelMenu");
        toDestroy.Add(panelMenu);
        panelMenu.transform.SetParent(pauseMenu.transform, false);

        panelOpciones = new GameObject("PanelOpciones");
        toDestroy.Add(panelOpciones);
        panelOpciones.transform.SetParent(pauseMenu.transform, false);

        panelSonido = new GameObject("PanelSonido");
        toDestroy.Add(panelSonido);
        panelSonido.transform.SetParent(pauseMenu.transform, false);

        panelControles = new GameObject("PanelControles");
        toDestroy.Add(panelControles);
        panelControles.transform.SetParent(pauseMenu.transform, false);

        panelAccesibilidad = new GameObject("PanelAccesibilidad");
        toDestroy.Add(panelAccesibilidad);
        panelAccesibilidad.transform.SetParent(pauseMenu.transform, false);

        // Estado inicial controlado (no depender del editor)
        pauseMenu.SetActive(true);
        panelMenu.SetActive(false);
        panelOpciones.SetActive(false);
        panelSonido.SetActive(false);
        panelControles.SetActive(false);
        panelAccesibilidad.SetActive(false);

        // ---------------- PauseManager ----------------
        pauseManagerGO = new GameObject("PauseManager");
        toDestroy.Add(pauseManagerGO);

        // Clave: desactivar para evitar Start/Update antes de inyectar refs
        pauseManagerGO.SetActive(false);

        pauseManager = pauseManagerGO.AddComponent<PauseManager>();

        // Asignar referencias directamente (son public, no hace falta reflection)
        pauseManager.pauseMenu = pauseMenu;
        pauseManager.panelMenu = panelMenu;
        pauseManager.panelOpciones = panelOpciones;
        pauseManager.panelSonido = panelSonido;
        pauseManager.panelControles = panelControles;
        pauseManager.panelAccesibilidad = panelAccesibilidad;

        // Activar y dejar que Start corra
        pauseManagerGO.SetActive(true);
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        Time.timeScale = 1f;

        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();

        yield return null;
    }

    [UnityTest]
    public IEnumerator PauseMenu_starts_inactive()
    {
        // Start() hace pauseMenu.SetActive(false)
        Assert.IsFalse(pauseMenu.activeSelf,
            "El pauseMenu debe empezar inactivo tras Start().");

        yield break;
    }

    [UnityTest]
    public IEnumerator Pause_sets_timescale_to_zero_and_activates_menu()
    {
        pauseManager.Pause();
        yield return null;

        Assert.That(Time.timeScale, Is.EqualTo(0f).Within(1e-4f),
            "Pause() debe poner Time.timeScale a 0.");

        Assert.IsTrue(pauseMenu.activeSelf,
            "Pause() debe activar pauseMenu.");

        Assert.IsTrue(panelMenu.activeSelf,
            "Pause() debe activar panelMenu.");

        Assert.IsFalse(panelOpciones.activeSelf,
            "Pause() debe desactivar panelOpciones.");

        Assert.IsFalse(panelSonido.activeSelf,
            "Pause() debe desactivar panelSonido.");
        Assert.IsFalse(panelControles.activeSelf,
            "Pause() debe desactivar panelControles.");
        Assert.IsFalse(panelAccesibilidad.activeSelf,
            "Pause() debe desactivar panelAccesibilidad.");
    }

    [UnityTest]
    public IEnumerator Resume_sets_timescale_to_one_and_deactivates_menu()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.Resume();
        yield return null;

        Assert.That(Time.timeScale, Is.EqualTo(1f).Within(1e-4f),
            "Resume() debe restaurar Time.timeScale a 1.");

        Assert.IsFalse(pauseMenu.activeSelf,
            "Resume() debe desactivar pauseMenu.");
    }

    [UnityTest]
    public IEnumerator OpenOptions_shows_options_panel()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        Assert.IsFalse(panelMenu.activeSelf,
            "OpenOptions() debe desactivar panelMenu.");

        Assert.IsTrue(panelOpciones.activeSelf,
            "OpenOptions() debe activar panelOpciones.");
    }

    [UnityTest]
    public IEnumerator OpenSoundOptions_shows_sound_panel()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenSoundOptions();
        yield return null;

        Assert.IsFalse(panelOpciones.activeSelf,
            "OpenSoundOptions() debe desactivar panelOpciones.");

        Assert.IsTrue(panelSonido.activeSelf,
            "OpenSoundOptions() debe activar panelSonido.");
    }

    [UnityTest]
    public IEnumerator OpenControls_shows_controls_panel()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenControls();
        yield return null;

        Assert.IsFalse(panelOpciones.activeSelf,
            "OpenControls() debe desactivar panelOpciones.");

        Assert.IsTrue(panelControles.activeSelf,
            "OpenControls() debe activar panelControles.");
    }

    [UnityTest]
    public IEnumerator OpenAccesibilityOptions_shows_accessibility_panel()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenAccesibilityOptions();
        yield return null;

        Assert.IsFalse(panelOpciones.activeSelf,
            "OpenAccesibilityOptions() debe desactivar panelOpciones.");

        Assert.IsTrue(panelAccesibilidad.activeSelf,
            "OpenAccesibilityOptions() debe activar panelAccesibilidad.");
    }

    [UnityTest]
    public IEnumerator BackToMenu_returns_to_main_menu_panel()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.BackToMenu();
        yield return null;

        Assert.IsFalse(panelOpciones.activeSelf,
            "BackToMenu() debe desactivar panelOpciones.");

        Assert.IsTrue(panelMenu.activeSelf,
            "BackToMenu() debe activar panelMenu.");
    }

    [UnityTest]
    public IEnumerator BackToOptionsMenu_returns_from_sound_to_options()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenSoundOptions();
        yield return null;

        pauseManager.BackToOptionsMenu();
        yield return null;

        Assert.IsFalse(panelSonido.activeSelf,
            "BackToOptionsMenu() debe desactivar panelSonido.");

        Assert.IsTrue(panelOpciones.activeSelf,
            "BackToOptionsMenu() debe activar panelOpciones.");
    }

    [UnityTest]
    public IEnumerator BackToOptionsMenuFromControls_returns_from_controls_to_options()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenControls();
        yield return null;

        pauseManager.BackToOptionsMenuFromControls();
        yield return null;

        Assert.IsFalse(panelControles.activeSelf,
            "BackToOptionsMenuFromControls() debe desactivar panelControles.");

        Assert.IsTrue(panelOpciones.activeSelf,
            "BackToOptionsMenuFromControls() debe activar panelOpciones.");
    }

    [UnityTest]
    public IEnumerator BackToOptionsMenuFromAccesibilityMenu_returns_from_accessibility_to_options()
    {
        pauseManager.Pause();
        yield return null;

        pauseManager.OpenOptions();
        yield return null;

        pauseManager.OpenAccesibilityOptions();
        yield return null;

        pauseManager.BackToOptionsMenuFromAccesibilityMenu();
        yield return null;

        Assert.IsFalse(panelAccesibilidad.activeSelf,
            "BackToOptionsMenuFromAccesibilityMenu() debe desactivar panelAccesibilidad.");

        Assert.IsTrue(panelOpciones.activeSelf,
            "BackToOptionsMenuFromAccesibilityMenu() debe activar panelOpciones.");
    }
}
