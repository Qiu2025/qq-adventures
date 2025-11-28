using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GhostRunnerTests
{
    private readonly List<Object> toDestroy = new();
    private GhostRunner ghostRunner;
    private GameObject recordTarget;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        var runnerGO = new GameObject("GhostRunner");
        toDestroy.Add(runnerGO);
        ghostRunner = runnerGO.AddComponent<GhostRunner>();

        recordTarget = new GameObject("RecordTarget");
        toDestroy.Add(recordTarget);

        var ghostPrefab = new GameObject("GhostPrefab");
        toDestroy.Add(ghostPrefab);
        ghostPrefab.AddComponent<TrailRenderer>();

        SetPrivateField(ghostRunner, "_ghostPrefab", ghostPrefab);
        SetPrivateField(ghostRunner, "_recordTarget", recordTarget.transform);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var obj in toDestroy)
        {
            if (obj != null) Object.Destroy(obj);
        }
        toDestroy.Clear();

        var instanceField = typeof(GhostRunner).GetField("<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);
        instanceField?.SetValue(null, null);

        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayRunByName_instancia_el_prefab_cuando_existe_run()
    {
        var system = (ReplaySystem)GetPrivateField(ghostRunner, "_system");
        system.StartRun(recordTarget.transform, 1, 1f, null, null);

        for (int i = 0; i < 3; i++)
        {
            recordTarget.transform.position += new Vector3(1f, 0f, 0f);
            yield return new WaitForFixedUpdate();
        }

        bool saved = system.FinishRunAndSave("run1", saveToFile: false);
        Assert.IsTrue(saved, "La run debe guardarse en memoria para poder reproducirse.");

        bool result = ghostRunner.PlayRunByName("run1");
        Assert.IsTrue(result, "PlayRunByName debe devolver true cuando encuentra la run.");

        yield return null;

        var spawnedGhost = GameObject.Find("GhostPrefab(Clone)");
        toDestroy.Add(spawnedGhost);
        Assert.IsNotNull(spawnedGhost, "El prefab del ghost debe instanciarse al reproducir la run.");
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(target, value);
    }

    private static object GetPrivateField(object target, string fieldName)
    {
        return target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(target);
    }
}

