using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BackgroundFollowTests
{
    private GameObject cameraObject;
    private GameObject backgroundObject;
    private BackgroundFollow backgroundFollow;
    private readonly List<Object> toDestroy = new();

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        cameraObject = new GameObject("MainCamera");
        toDestroy.Add(cameraObject);
        cameraObject.tag = "MainCamera";
        var cam = cameraObject.AddComponent<Camera>();
        cam.transform.position = Vector3.zero;

        backgroundObject = new GameObject("Background");
        toDestroy.Add(backgroundObject);
        backgroundFollow = backgroundObject.AddComponent<BackgroundFollow>();
        backgroundFollow.GetType().GetField("parallaxX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(backgroundFollow, 0.5f);
        backgroundFollow.GetType().GetField("parallaxY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(backgroundFollow, 0.5f);
        backgroundFollow.GetType().GetField("cameraTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(backgroundFollow, cameraObject.transform);

        backgroundObject.transform.position = new Vector3(0f, 0f, 10f);

        yield return null; // asegura Start()
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();
        yield return null;
    }

    [UnityTest]
    public IEnumerator Background_moves_half_camera_distance_with_parallax_05()
    {
        Vector3 initialBackgroundPos = backgroundObject.transform.position;
        Vector3 initialCameraPos = cameraObject.transform.position;

        cameraObject.transform.position = new Vector3(2f, 2f, 0f);
        yield return null; // deja que Unity ejecute LateUpdate()

        Vector3 expectedPos = initialBackgroundPos + (cameraObject.transform.position - initialCameraPos) * 0.5f;
        Assert.AreEqual(expectedPos.x, backgroundObject.transform.position.x, 0.001f, "El fondo debe moverse la mitad de la distancia de la cámara en X");
        Assert.AreEqual(expectedPos.y, backgroundObject.transform.position.y, 0.001f, "El fondo debe moverse la mitad de la distancia de la cámara en Y");
    }

    [UnityTest]
    public IEnumerator Background_stays_still_when_parallax_is_zero()
    {
        backgroundFollow.GetType().GetField("parallaxX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(backgroundFollow, 0f);
        backgroundFollow.GetType().GetField("parallaxY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(backgroundFollow, 0f);

        Vector3 initialBackgroundPos = backgroundObject.transform.position;

        cameraObject.transform.position = new Vector3(5f, 3f, 0f);
        yield return null;

        Assert.AreEqual(initialBackgroundPos, backgroundObject.transform.position, "El fondo no debe moverse cuando el parallax es 0");
    }
}
