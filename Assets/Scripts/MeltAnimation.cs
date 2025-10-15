using UnityEngine;

public class MeltAnimation : MonoBehaviour
{
    public SpriteRenderer target;
    public Sprite[] frames;
    public float meltDuration = 10f;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    float t;
    bool playing;
    int lastIdx = -1;

    void Awake()
    {
        var anim = GetComponent<Animator>();
        if (anim) anim.enabled = false;
    }

    void OnEnable() { Play(); }

    public void Play()
    {
        t = 0f;
        lastIdx = -1;
        playing = true;
        if (frames != null && frames.Length > 0) target.sprite = frames[0];
    }

    void Update()
    {
        if (!playing || frames == null || frames.Length == 0 || target == null) return;

        t += Time.deltaTime;
        float norm = Mathf.Clamp01(t / Mathf.Max(0.0001f, meltDuration));
        norm = curve.Evaluate(norm);

        int idx = Mathf.Clamp(Mathf.FloorToInt(norm * (frames.Length - 1)), 0, frames.Length - 1);

        if (idx != lastIdx) // solo cambia cuando toca
        {
            target.sprite = frames[idx];
            lastIdx = idx;
        }

        if (t >= meltDuration)
        {
            playing = false;
            target.enabled = false; // oculta sprite
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = false; // desactiva el collider
        }
    }
}
