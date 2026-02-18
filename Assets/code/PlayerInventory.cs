using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public int money = 0;
    public int goldFragments = 0;
    public int diamondFragments = 0;

    public int goldPrice = 50;
    public int diamondPrice = 100;

    public TextMeshProUGUI moneyText; // MoneyText

    void Start()
    {
        UpdateUI();
    }

    public void CollectItem(ValuableBlock.Type type)
    {
        if (type == ValuableBlock.Type.Gold) goldFragments++;
        else if (type == ValuableBlock.Type.Diamond) diamondFragments++;

    }
    public void SellAllItems()
    {
        int earnings = (goldFragments * goldPrice) + (diamondFragments * diamondPrice);

        if (earnings > 0)
        {
            money += earnings;
            goldFragments = 0;
            diamondFragments = 0;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + money.ToString();
        }
    }
}