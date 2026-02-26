using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameSceneName = "sample"; 

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        // 在编辑器里不会退出是正常的，Build 后才会退出
        Debug.Log("QuitGame called");
    }
}