using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
