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
    Debug.Log("OpenInformation on object: " + gameObject.name);

    if (informationPanel != null)
        informationPanel.SetActive(true);

    if (controlsPanel != null)
    {
        controlsPanel.SetActive(false);
        Debug.Log("controlsPanel after false = " + controlsPanel.activeSelf);
    }

    if (objectivePanel != null)
        objectivePanel.SetActive(false);

    if (itemsPanel != null)
        itemsPanel.SetActive(false);
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
    }

    public void ShowObjective()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(true);

        if (itemsPanel != null)
            itemsPanel.SetActive(false);
    }

    public void ShowItems()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (itemsPanel != null)
            itemsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QuitGame called");
    }
}