using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class CreditsMusic : MonoBehaviour
{
    public float delayBeforeMusic = 8f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        StartCoroutine(PlayAfterDelay());
    }

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeMusic);
        audioSource.Play();
    }

    void OnDisable()
    {
        audioSource.Stop();
    }
}
