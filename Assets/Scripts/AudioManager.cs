using UnityEngine;

// Script que contiene los audio clips y metodos para reproducir dichos clips
// Metido en un empty de cada nivel
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource bgm;
    [SerializeField] private AudioSource sfx;
    
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip powerUp;
    [SerializeField] private AudioClip dash;
    [SerializeField] private AudioClip checkpoint;
    [SerializeField] private AudioClip key;
    [SerializeField] private AudioClip door;
    
    
    public float volumen; 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayJumpSound()
    {
        sfx.PlayOneShot(jump,volumen);
    }

    public void PlayDieSound()
    {
        sfx.PlayOneShot(die,volumen);
    }

    public void PlayCoinSound()
    {
        sfx.PlayOneShot(coin,volumen);
    }

    public void PlayPowerUpSound()
    {
        sfx.PlayOneShot(powerUp,volumen);
    }

    public void PlayDashSound()
    {
        sfx.PlayOneShot(dash,volumen);
    }
    
    public void PlayCheckpointSound()
    {
        sfx.PlayOneShot(checkpoint, volumen);
    }
    
    public void PlayKeySound()
    {
        sfx.PlayOneShot(key, volumen);
    }
    
    public void PlayDoorSound()
    {
        sfx.PlayOneShot(door, volumen);
    }

    

}
