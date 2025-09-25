using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSounds();
        }
        
        SceneManager.LoadScene(sceneIndex);
    }
}
