using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Home");
    }
    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    
    public void QuitGame() // make another script that does not show the quit option.
    {
        #if UNITY_EDITOR
        if(EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        #endif
        Application.Quit(); // for Desktop/PC
    }
}
