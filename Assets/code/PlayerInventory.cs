using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class ShopItemData
{
    public string itemName;
    public int count = 0;
    public int price;
    public GameObject inventorySlotObj;
    public TextMeshProUGUI uiText;
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Money & Materials")]
    public int money = 0;
    public int goldFragments = 0;
    public int diamondFragments = 0;
    public int rubyFragments = 0;
    public int emeraldFragments = 0;

    [Header("Sell Prices")]
    public int goldPrice = 50;
    public int emeraldPrice = 100;
    public int rubyPrice = 150;
    public int diamondPrice = 200;

    public TextMeshProUGUI moneyText;

    [Header("Shop & Inventory")]
    public List<ShopItemData> myItems = new List<ShopItemData>();

    [Header("Win Settings")]
    public GameObject winPanel;
    public AudioSource sfxSource;
    public AudioClip winClip;
    public string mainMenuSceneName = "MainMenu";

    [Header("Optional Win Buttons")]
    public Button restartButton;
    public Button mainMenuButton;

    private bool hasWon = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        UpdateUI();
    }

    public void CollectItem(ValuableBlock.Type type)
    {
        if (type == ValuableBlock.Type.Gold)
        {
            goldFragments++;
        }
        else if (type == ValuableBlock.Type.Diamond)
        {
            diamondFragments++;
        }
        else if (type == ValuableBlock.Type.Ruby)
        {
            rubyFragments++;
        }
        else if (type == ValuableBlock.Type.Emerald)
        {
            emeraldFragments++;
        }
    }

    public void SellOneItem(ValuableBlock.Type type)
    {
        if (type == ValuableBlock.Type.Gold && goldFragments > 0)
        {
            goldFragments--;
            money += goldPrice;
        }
        else if (type == ValuableBlock.Type.Diamond && diamondFragments > 0)
        {
            diamondFragments--;
            money += diamondPrice;
        }
        else if (type == ValuableBlock.Type.Ruby && rubyFragments > 0)
        {
            rubyFragments--;
            money += rubyPrice;
        }
        else if (type == ValuableBlock.Type.Emerald && emeraldFragments > 0)
        {
            emeraldFragments--;
            money += emeraldPrice;
        }

        UpdateUI();
    }

    public void SellAllItems()
    {
        int earnings = (goldFragments * goldPrice) +
                       (diamondFragments * diamondPrice) +
                       (rubyFragments * rubyPrice) +
                       (emeraldFragments * emeraldPrice);

        if (earnings > 0)
        {
            money += earnings;
            goldFragments = 0;
            diamondFragments = 0;
            rubyFragments = 0;
            emeraldFragments = 0;
            UpdateUI();
        }
    }

    public bool BuyItem(string targetItemName)
    {
        foreach (var item in myItems)
        {
            if (item.itemName == targetItemName)
            {
                if (money >= item.price)
                {
                    money -= item.price;
                    item.count++;
                    UpdateUI();
                    Debug.Log($"Buy {targetItemName} successfully! current: {item.count}");
                    return true;
                }
                else
                {
                    Debug.Log($"Not enough money to buy {targetItemName}!");
                    return false;
                }
            }
        }

        Debug.LogError($"Error: Inventory system does not contain {targetItemName}!");
        return false;
    }

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + money.ToString();
        }

        foreach (var item in myItems)
        {
            if (item.inventorySlotObj != null)
            {
                if (item.count > 0)
                {
                    item.inventorySlotObj.SetActive(true);

                    if (item.uiText != null)
                    {
                        item.uiText.text = "x" + item.count.ToString();
                    }
                }
                else
                {
                    item.inventorySlotObj.SetActive(false);
                }
            }
        }
    }

    public void WinGame()
    {
        if (hasWon)
        {
            return;
        }

        hasWon = true;

        DisableAllGameplayInput();

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (sfxSource != null && winClip != null)
        {
            sfxSource.PlayOneShot(winClip);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (EventSystem.current != null)
        {
            if (mainMenuButton != null)
            {
                EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
            }
            else if (restartButton != null)
            {
                EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
            }
        }

        Time.timeScale = 0f;
    }

    void DisableAllGameplayInput()
    {
        MonoBehaviour[] playerScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in playerScripts)
        {
            if (script != null && script != this)
            {
                script.enabled = false;
            }
        }

        DisableScriptsOnObject("Canvas-Main");
        DisableScriptsOnObject("Canvas-Shop");
        DisableScriptsOnObject("PortalSystem");
        DisableScriptsOnObject("Shop");
        DisableScriptsOnObject("PauseMenuController");
    }

    void DisableScriptsOnObject(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null) return;

        MonoBehaviour[] scripts = obj.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Restart clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Main Menu clicked");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}