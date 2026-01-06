using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgm;
    [SerializeField] private AudioSource sfx;
    
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip die;
    // [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip powerUp;
    [SerializeField] private AudioClip dash;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayJumpSound()
    {
        sfx.PlayOneShot(jump);
    }

    public void PlayDieSound()
    {
        sfx.PlayOneShot(die);
    }
    
    // public void PlayWalkSound()
    // {
    //     sfx.PlayOneShot(walk);
    // }

    public void PlayCoinSound()
    {
        sfx.PlayOneShot(coin);
    }

    public void PlayPowerUpSound()
    {
        sfx.PlayOneShot(powerUp);
    }

    public void PlayDashSound()
    {
        sfx.PlayOneShot(dash);
    }
}
