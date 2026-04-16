using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject howToPlayPanel;
    public GameObject controlsPanel;
    public GameObject objectivePanel;
    public GameObject itemsPanel;
    public GameObject resourcesPanel;

    void Start()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (resourcesPanel != null) resourcesPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);

        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (resourcesPanel != null) resourcesPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

    public void ShowControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (resourcesPanel != null) resourcesPanel.SetActive(false);
    }

    public void ShowObjective()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(true);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (resourcesPanel != null) resourcesPanel.SetActive(false);
    }

    public void ShowItems()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(true);
        if (resourcesPanel != null) resourcesPanel.SetActive(false);
    }

    public void ShowResources()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
        if (resourcesPanel != null) resourcesPanel.SetActive(true);
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
}