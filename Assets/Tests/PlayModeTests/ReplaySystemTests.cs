using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReplaySystemTests
{
    private readonly List<Object> toDestroy = new();
    private ReplaySystem system;
    private GameObject target;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        var runnerGO = new GameObject("ReplayRunner");
        toDestroy.Add(runnerGO);
        var runner = runnerGO.AddComponent<ReplayRunner>();
        runner.Init();
        system = runner.System;

        target = new GameObject("Target");
        toDestroy.Add(target);

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
        yield return null;
    }

    [UnityTest]
    public IEnumerator FinishRunAndSave_registra_la_run_en_memoria()
    {
        system.StartRun(target.transform, 1, 1f, null, null);

        for (int i = 0; i < 3; i++)
        {
            target.transform.position += Vector3.right;
            yield return new WaitForFixedUpdate();
        }

        bool saved = system.FinishRunAndSave("test_run", saveToFile: false);
        Assert.IsTrue(saved, "FinishRunAndSave debe devolver true cuando había una grabación en curso.");

        bool found = system.GetRun("test_run", out var run);
        Assert.IsTrue(found, "La run debe almacenarse en memoria.");
        Assert.Greater(run.Duration, 0f, "La duración debe actualizarse en función de los snapshots añadidos.");
    }

    [UnityTest]
    public IEnumerator PlayRecording_reproduce_y_destruye_el_ghost_al_finalizar()
    {
        system.StartRun(target.transform, 1, 1f, null, null);

        for (int i = 0; i < 2; i++)
        {
            target.transform.position += new Vector3(0.5f, 0f, 0f);
            yield return new WaitForFixedUpdate();
        }

        system.FinishRunAndSave("play_run", saveToFile: false);
        system.GetRun("play_run", out var run);

        var ghost = new GameObject("Ghost");
        ghost.AddComponent<TrailRenderer>();
        system.PlayRecording("play_run", ghost);

        yield return new WaitForSeconds(run.Duration + 0.2f);
        Assert.IsTrue(ghost == null, "Al finalizar la reproducción el ghost debe destruirse.");
    }

    private class ReplayRunner : MonoBehaviour
    {
        public ReplaySystem System { get; private set; }
        public void Init()
        {
            System = new ReplaySystem(this);
        }
    }
}

