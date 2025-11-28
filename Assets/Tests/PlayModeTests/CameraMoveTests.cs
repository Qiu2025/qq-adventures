using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraMoveTests
{
    private readonly List<Object> toDestroy = new();

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
    public IEnumerator Update_moves_camera_to_the_right_based_on_speed()
    {
        var go = new GameObject("CameraMover");
        toDestroy.Add(go);

        var mover = go.AddComponent<cameraMove>();
        mover.speed = 3f;

        Vector3 initialPosition = go.transform.position;
        yield return null; // deja que Unity ejecute Update una vez

        float expectedDelta = mover.speed * Time.deltaTime;
        float realDelta = go.transform.position.x - initialPosition.x;

        Assert.AreEqual(expectedDelta, realDelta, 0.01f,
            "La cámara debe desplazarse a la derecha en función de la velocidad y deltaTime.");
    }

    [UnityTest]
    public IEnumerator Update_responds_to_speed_changes_en_tiempo_de_ejecucion()
    {
        var go = new GameObject("CameraMover");
        toDestroy.Add(go);

        var mover = go.AddComponent<cameraMove>();
        mover.speed = 1f;

        yield return null;
        float delta1 = go.transform.position.x;

        mover.speed = 5f;
        yield return null;
        float delta2 = go.transform.position.x - delta1;

        Assert.Greater(delta2, delta1,
            "Al aumentar la velocidad en runtime, el desplazamiento por frame debe ser mayor.");
    }
}

