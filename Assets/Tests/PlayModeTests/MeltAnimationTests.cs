using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class MeltAnimationTests
{
    private GameObject go;
    private MeltAnimation melt;
    private SpriteRenderer sr;
    private readonly List<Object> toDestroy = new();

    private Sprite MakeSprite(int w, int h)
    {
        var tex = new Texture2D(w, h);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
    }

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        go = new GameObject("MeltObject");
        toDestroy.Add(go);
        sr = go.AddComponent<SpriteRenderer>();
        go.AddComponent<BoxCollider2D>();

        melt = go.AddComponent<MeltAnimation>();
        melt.target = sr;
        melt.frames = new[] { MakeSprite(2, 2), MakeSprite(3, 3), MakeSprite(4, 4) };
        melt.meltDuration = 0.15f;

        yield return null; // OnEnable -> Play
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        foreach (var o in toDestroy) if (o) Object.Destroy(o);
        toDestroy.Clear();
        yield return null;
    }

    private IEnumerator SimulateForSeconds(float seconds)
    {
        var steps = Mathf.CeilToInt(seconds / Time.fixedDeltaTime);
        for (int i = 0; i < steps; i++) yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator Play_sets_first_frame_immediately()
    {
        // Se testea que al iniciar fija el primer frame
        Assert.AreEqual(melt.frames[0], sr.sprite);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Frames_advance_and_finish_disables_sprite_and_collider()
    {
        // Se testea que avanza frames y al terminar desactiva render y collider
        var first = sr.sprite;
        yield return SimulateForSeconds(melt.meltDuration * 0.6f);
        Assert.AreNotEqual(first, sr.sprite);

        yield return SimulateForSeconds(melt.meltDuration); // excede duración
        yield return null;

        Assert.IsFalse(sr.enabled);
        var col = go.GetComponent<Collider2D>();
        Assert.IsFalse(col.enabled);
    }
}