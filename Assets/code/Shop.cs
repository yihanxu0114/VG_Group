using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [Header("UI References")]
    public GameObject shopUI;

    public GameObject goldItemObj;
    public GameObject diamondItemObj;
    public GameObject rubyItemObj;
    public GameObject emeraldItemObj;
    public TextMeshProUGUI goldCountText;
    public TextMeshProUGUI diamondCountText;
    public TextMeshProUGUI rubyCountText;
    public TextMeshProUGUI emeraldCountText;

    private bool isPlayerInRange = false;
    private PlayerInventory currentPlayer;

    void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);
    }

    void Update()
    {
        if (PauseMenuController.IsPaused)
            return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (shopUI != null)
            {
                bool isActive = shopUI.activeSelf;
                shopUI.SetActive(!isActive);

                if (!isActive && currentPlayer != null)
                {
                    UpdateShopDisplay(currentPlayer);
                }

                Time.timeScale = isActive ? 1f : 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inv = other.GetComponent<PlayerInventory>();
        if (inv != null)
        {
            isPlayerInRange = true;
            currentPlayer = inv;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (shopUI == null) return;
        if (other == null || other.gameObject == null) return;

        if (other.GetComponent<PlayerInventory>() != null)
        {
            isPlayerInRange = false;
            currentPlayer = null;
            shopUI.SetActive(false);

            if (!PauseMenuController.IsPaused)
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void OnSellButtonClick()
    {
        if (PauseMenuController.IsPaused)
            return;

        if (currentPlayer != null)
        {
            currentPlayer.SellAllItems();
        }
    }

    public void CloseShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }

        if (!PauseMenuController.IsPaused)
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateShopDisplay(PlayerInventory playerInv)
    {
        if (playerInv == null) return;

        playerInv.UpdateUI();

        if (playerInv.goldFragments > 0)
        {
            if (goldItemObj != null) goldItemObj.SetActive(true);
            if (goldCountText != null) goldCountText.text = "x" + playerInv.goldFragments;
        }
        else
        {
            if (goldItemObj != null) goldItemObj.SetActive(false);
        }

        if (playerInv.diamondFragments > 0)
        {
            if (diamondItemObj != null) diamondItemObj.SetActive(true);
            if (diamondCountText != null) diamondCountText.text = "x" + playerInv.diamondFragments;
        }
        else
        {
            if (diamondItemObj != null) diamondItemObj.SetActive(false);
        }

        if (playerInv.rubyFragments > 0)
        {
            if (rubyItemObj != null) rubyItemObj.SetActive(true);
            if (rubyCountText != null) rubyCountText.text = "x" + playerInv.rubyFragments;
        }
        else
        {
            if (rubyItemObj != null) rubyItemObj.SetActive(false);
        }

        if (playerInv.emeraldFragments > 0)
        {
            if (emeraldItemObj != null) emeraldItemObj.SetActive(true);
            if (emeraldCountText != null) emeraldCountText.text = "x" + playerInv.emeraldFragments;
        }
        else
        {
            if (emeraldItemObj != null) emeraldItemObj.SetActive(false);
        }
    }

    public void OnBuyItemClick(string itemName)
    {
        if (PauseMenuController.IsPaused)
            return;

        if (currentPlayer != null)
        {
            bool success = currentPlayer.BuyItem(itemName);
            if (success)
            {
                Debug.Log($"Store Sell Successfully {itemName}");
            }
        }
    }
}