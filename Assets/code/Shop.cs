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

    private bool isPlayerInRange = false; // player is in the shop area or not
    private PlayerInventory currentPlayer; // reference to the player's inventory, so we can call sell functions

    void Start()
    {
        // when the game starts, make sure the shop UI is hidden
        if (shopUI != null) shopUI.SetActive(false);
    }

    void Update()
    {
        // only allow opening the shop if the player is in range and presses E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // if the shop UI is already active, this will close it; if it's closed, this will open it
            if (shopUI != null)
            {
                bool isActive = shopUI.activeSelf;
                shopUI.SetActive(!isActive); // switch the active state of the shop UI

                if (!isActive && currentPlayer != null)
                {
                    UpdateShopDisplay(currentPlayer);
                }

                Time.timeScale = isActive ? 1 : 0;
            }
        }
    }

    // player enters the shop area
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
        }
    }

    public void OnSellButtonClick()
    {
        if (currentPlayer != null)
        {
            currentPlayer.SellAllItems();

            // CloseShop(); 
        }
    }

    public void CloseShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false); 
        }

        Time.timeScale = 1;
    }

    public void UpdateShopDisplay(PlayerInventory playerInv)
    {
        if (playerInv == null) return;

        if (playerInv.goldFragments > 0)
        {
            goldItemObj.SetActive(true);
            goldCountText.text = "x" + playerInv.goldFragments;
        }
        else goldItemObj.SetActive(false);

        if (playerInv.diamondFragments > 0)
        {
            diamondItemObj.SetActive(true);
            diamondCountText.text = "x" + playerInv.diamondFragments;
        }
        else diamondItemObj.SetActive(false);

        if (playerInv.rubyFragments > 0)
        {
            if (rubyItemObj != null) rubyItemObj.SetActive(true);
            if (rubyCountText != null) rubyCountText.text = "x" + playerInv.rubyFragments;
        }
        else if (rubyItemObj != null) rubyItemObj.SetActive(false);

        if (playerInv.emeraldFragments > 0)
        {
            if (emeraldItemObj != null) emeraldItemObj.SetActive(true);
            if (emeraldCountText != null) emeraldCountText.text = "x" + playerInv.emeraldFragments;
        }
        else if (emeraldItemObj != null) emeraldItemObj.SetActive(false);
    }

    public void OnBuyItemClick(string itemName)
    {
        if (currentPlayer != null)
        {
            bool success = currentPlayer.BuyItem(itemName);
            if (success)
            {
                Debug.Log($"Store£ºSell Successfully {itemName}£¡");
            }
        }
    }
}
