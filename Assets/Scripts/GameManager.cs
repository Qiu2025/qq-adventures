using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --------------------------------------------- //
    private static bool gameOver = false;
    private static bool slowmo = false;
    
    // --- PARA PUNTUACION POR LOS COLLECTIBLES --- //
    [HideInInspector] public static int score = 0;
    
    // -------- PARA RESPAWN EN CHECKPOINT -------- //
    private static Vector2 lastCheckpointPos = new Vector2(-11.75f, 6.4f); 
    private static GameObject player;
    
    // -------- PARA BOCADILLO ------------------- //
    private static GameObject chat; 

    // --------------------------------------------- //

    [Header("Selección de mecánicas")]
    public bool allowDoubleJump = false;
    public bool allowDash = false;
    public bool allowGlide = false;

    // --------------------------------------------- //

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        chat =  GameObject.FindGameObjectWithTag("Chat");
        Application.targetFrameRate = 144;
        Instance = this;
    }

    void Update()
    {
        // Para volver al ultimo checkpoint
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }

        // Slow motion, para debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            slowmo = !slowmo;
            Time.timeScale = slowmo ? 0.2f : 1f;
        }
    }
    
    public static void SetCheckpoint(Vector2 position)
    {
        lastCheckpointPos = position;
        Debug.Log("Checkpoint guardado en: " + position);
    }

    public static void RespawnPlayer()
    {
        Collider2D col = player.GetComponent<BoxCollider2D>();
        col.enabled = false;

        player.transform.position = lastCheckpointPos;

        col.enabled = true;

        Debug.Log("Jugador reaparecido en checkpoint");
        gameOver = false;
        Time.timeScale = 1f;
    }

    public static void SetGameOver(bool cond) {
        gameOver = cond;
        Time.timeScale = gameOver ? 0f: 1f;
    }
    
    
    public static void ShowChat(Sprite sprite, float fadeIn, float hold, float fadeOut, 
                                float scale = 1f, float offsetX = 0f, float offsetY = 0f)
    {
        if (chat == null || sprite == null) return;

        Instance.StartCoroutine(Instance.ShowChatRoutine(sprite, fadeIn, hold, fadeOut, scale, offsetX, offsetY));
    }

    private IEnumerator ShowChatRoutine(Sprite sprite, float fadeIn, float hold, float fadeOut,
                                        float scale, float offsetX, float offsetY)
    {
        // Hijo que contiene el SpriteRenderer del chat
        SpriteRenderer imgRenderer = chat.transform.Find("ImagenMostrar").GetComponent<SpriteRenderer>();
        if (imgRenderer == null) yield break;

        // Asignar el sprite
        imgRenderer.sprite = sprite;

        // Ajustar escala
        imgRenderer.transform.localScale = Vector3.one * scale;

        // Ajustar posición relativa
        imgRenderer.transform.localPosition = new Vector3(offsetX, offsetY, imgRenderer.transform.localPosition.z);

        // Activar chat
        chat.SetActive(true);

        // Poner alfa 0 a todos los SpriteRenderers del chat
        SpriteRenderer[] renderers = chat.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = 0f;
            r.color = c;
        }

        // Fade in
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float a = t / fadeIn;
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = a;
                r.color = c;
            }
            yield return null;
        }

        // Mantener visible
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = 1f;
            r.color = c;
        }
        yield return new WaitForSeconds(hold);

        // Fade out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float a = 1f - t / fadeOut;
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = a;
                r.color = c;
            }
            yield return null;
        }

        // Ocultar chat
        chat.SetActive(false);
    }

}
