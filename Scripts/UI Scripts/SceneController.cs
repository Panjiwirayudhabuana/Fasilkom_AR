using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // HOME SCENE
    public void GoToAR()
    {
        SceneManager.LoadScene("AR Scene");
    }


    public void ExitApp()
    {
        Application.Quit();
    }

    // AR SCENE
    public void GoToHome()
    {
        SceneManager.LoadScene("Home Scene");
    }

}
