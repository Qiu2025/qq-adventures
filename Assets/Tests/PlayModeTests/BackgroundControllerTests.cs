using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class BackgroundControllerTests
{
    private GameObject bgObject;
    private BackgroundController controller;
    private GameObject camObject;
    private SpriteRenderer renderer;
    private readonly System.Collections.Generic.List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Cámara fake
        camObject = new GameObject("Camera");
        toDestroy.Add(camObject);

        // Fondo con sprite
        bgObject = new GameObject("Background");
        toDestroy.Add(bgObject);
        renderer = bgObject.AddComponent<SpriteRenderer>();
        
        // Hacemos un sprite
        Texture2D tex = new Texture2D(100, 1);
        renderer.sprite = Sprite.Create(tex, new Rect(0, 0, 100, 1), new Vector2(0.5f, 0.5f));

        controller = bgObject.AddComponent<BackgroundController>();
        controller.cam = camObject;
        controller.parallaxEffect = 0.5f;

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (var o in toDestroy)
            if (o) Object.Destroy(o);
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Parallax_moves_background_correctly()
    {
        float start = bgObject.transform.position.x;

        camObject.transform.position = new Vector3(10f, 0f, 0f);
        yield return new WaitForFixedUpdate();

        float expectedX = start + 10f * 0.5f; // parallaxEffect = 0.5f

        Assert.AreEqual(expectedX, bgObject.transform.position.x, 0.001f,
            "El fondo debe moverse según la fórmula de parallax.");
    }

    [UnityTest]
    public IEnumerator Background_wraps_forward_when_camera_passes_length()
    {
        float start = bgObject.transform.position.x;
        float len = renderer.bounds.size.x; // = 100

        // Cámara avanza lo suficiente para provocar wrap
        camObject.transform.position = new Vector3(start + len + 5f, 0f, 0f);
        yield return new WaitForFixedUpdate();

        var field = typeof(BackgroundController).GetField("startPos",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float newStart = (float)field.GetValue(controller);

        Assert.AreEqual(start + len, newStart, 0.001f,
            "startPos debe aumentar en +length cuando la cámara supera el límite.");
    }

    [UnityTest]
    public IEnumerator Background_wraps_backward_when_camera_moves_left()
    {
        float start = bgObject.transform.position.x;
        float len = renderer.bounds.size.x; // = 100

        // Cámara retrocede más allá del límite
        camObject.transform.position = new Vector3(start - len - 5f, 0f, 0f);
        yield return new WaitForFixedUpdate();

        var field = typeof(BackgroundController).GetField("startPos",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float newStart = (float)field.GetValue(controller);

        Assert.AreEqual(start - len, newStart, 0.001f,
            "startPos debe disminuir en -length cuando la cámara retrocede demasiado.");
    }
}
