using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameSceneName = "sample";

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject informationPanel;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (informationPanel != null)
            informationPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OpenInformation()
    {
        if (informationPanel != null)
            informationPanel.SetActive(true);
    }

    public void CloseInformation()
    {
        if (informationPanel != null)
            informationPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QuitGame called");
    }
}