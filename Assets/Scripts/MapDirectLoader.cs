using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapDirectLoader : MonoBehaviour
{
    [Header("Configuración")]
    public string sceneName;      
    public bool isUnlocked;      

    [Header("Referencias")]
    public GameObject lockIcon;   

    private void Start()
    {
        ActualizarVisuales();
    }
    
    private void ActualizarVisuales()
    {
      
        if (lockIcon != null)
        {
         
            lockIcon.SetActive(!isUnlocked);
        }
        
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = isUnlocked; 
        }
    }
    
    public void LoadGameScene()
    {
        if (isUnlocked)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogError("¡Se te olvidó escribir el nombre de la escena en el Inspector!");
            }
        }
    }
}