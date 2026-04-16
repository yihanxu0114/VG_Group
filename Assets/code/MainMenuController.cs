using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameSceneName = "sample";

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject informationPanel;

    [Header("Information Sub Panels")]
    public GameObject controlsPanel;
    public GameObject objectivePanel;
    public GameObject itemsPanel;
    public GameObject resourcesPanel;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (informationPanel != null)
            informationPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
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

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
    }

    public void CloseInformation()
    {
        if (informationPanel != null)
            informationPanel.SetActive(false);
    }

    public void ShowControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
    }

    public void ShowObjective()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
    }

    public void ShowItems()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(true);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
    }

    public void ShowResources()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);

        if (resourcesPanel != null)
            resourcesPanel.SetActive(true);
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }

    public void CloseObjective()
    {
        if (objectivePanel != null)
            objectivePanel.SetActive(false);
    }

    public void CloseItems()
    {
        if (itemsPanel != null)
            itemsPanel.SetActive(false);
    }

    public void CloseResources()
    {
        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QuitGame called");
    }
}