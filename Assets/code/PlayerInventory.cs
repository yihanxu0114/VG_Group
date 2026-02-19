using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    public int money = 0;
    public int goldFragments = 0;
    public int diamondFragments = 0;

    public int goldPrice = 50;
    public int diamondPrice = 100;

    public TextMeshProUGUI moneyText; // MoneyText

    public int winMoney = 150;         
    public GameObject winPanel;         
    public AudioSource sfxSource;       
    public AudioClip winClip;           
    public string mainMenuSceneName = "MainMenu"; 

    private bool hasWon = false;

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        UpdateUI();
    }

    public void CollectItem(ValuableBlock.Type type)
    {
        if (type == ValuableBlock.Type.Gold) goldFragments++;
        else if (type == ValuableBlock.Type.Diamond) diamondFragments++;

    }
    public void SellAllItems()
    {
        ebug.Log("[SellAllItems] called");
        int earnings = (goldFragments * goldPrice) + (diamondFragments * diamondPrice);

        if (earnings > 0)
        {
            money += earnings;
            goldFragments = 0;
            diamondFragments = 0;
            UpdateUI();
            CheckWin();
        }
    }

    void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$ " + money.ToString();
        }
    }

    void CheckWin()
    {
        if (hasWon) return;
        if (money < winMoney) return;

        hasWon = true;
        Debug.Log("YOU WIN!");

        if (winPanel != null) winPanel.SetActive(true);

        if (sfxSource != null && winClip != null)
            sfxSource.PlayOneShot(winClip);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}