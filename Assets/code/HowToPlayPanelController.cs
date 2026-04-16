using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject howToPlayPanel;
    public GameObject controlsPanel;
    public GameObject objectivePanel;
    public GameObject itemsPanel;

    void Start()
    {
        ShowControls();
    }

    public void OpenPanel()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }

        ShowControls();
    }

    public void ClosePanel()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    public void ShowControls()
    {
        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(false);
    }

    public void ShowObjective()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(true);
        if (itemsPanel != null) itemsPanel.SetActive(false);
    }

    public void ShowItems()
    {
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (itemsPanel != null) itemsPanel.SetActive(true);
    }
}
