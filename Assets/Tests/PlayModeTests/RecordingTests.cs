using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RecordingTests
{
    [UnityTest]
    public IEnumerator AddSnapshot_captura_posicion_y_duracion()
    {
        var go = new GameObject("Target");
        go.transform.position = new Vector3(1f, 2f, 0f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, 45f);

        var recording = new Recording(go.transform);
        recording.AddSnapshot(0.5f, false);

        Pose pose = recording.EvaluatePoint(0.5f);
        Assert.AreEqual(go.transform.position, pose.position, "La posición grabada debe coincidir con la del objetivo.");
        Assert.AreEqual(45f, pose.rotation.eulerAngles.z, 0.001f, "La rotación grabada debe coincidir con la del objetivo.");
        Assert.AreEqual(0.5f, recording.Duration, 0.0001f, "La duración debe actualizarse con el último snapshot.");

        Object.Destroy(go);
        yield break;
    }

    [UnityTest]
    public IEnumerator Serialize_y_deserialize_preservan_los_datos()
    {
        var go = new GameObject("Target");
        var recording = new Recording(go.transform);

        go.transform.position = new Vector3(2f, 3f, 0f);
        recording.AddSnapshot(0.2f, true);

        go.transform.position = new Vector3(4f, 6f, 0f);
        recording.AddSnapshot(0.4f, false);

        string data = recording.Serialize();
        var loaded = new Recording(data);

        var pose = loaded.EvaluatePoint(0.2f);
        Assert.AreEqual(2f, pose.position.x, 0.0001f);
        Assert.IsTrue(loaded.GetTrailActiveAt(0.2f), "El estado del TrailRenderer debe preservarse al serializar.");

        pose = loaded.EvaluatePoint(0.4f);
        Assert.AreEqual(6f, pose.position.y, 0.0001f);
        Assert.IsFalse(loaded.GetTrailActiveAt(0.4f), "El estado del TrailRenderer debe coincidir tras deserializar.");

        Object.Destroy(go);
        yield break;
    }
}

