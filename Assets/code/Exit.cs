using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused = false;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button exitButton;

    [Header("Optional Main Player Script")]
    public MonoBehaviour playerController;

    private bool isPaused = false;
    private int selectedIndex = 0;
    private Button[] menuButtons;

    void Start()
    {
        menuButtons = new Button[] { resumeButton, exitButton };

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        isPaused = false;
        IsPaused = false;
        selectedIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }

        if (isPaused)
        {
            HandleKeyboardNavigation();
        }
    }

    void HandleKeyboardNavigation()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = (selectedIndex - 1 + menuButtons.Length) % menuButtons.Length;
            SelectButton(selectedIndex);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = (selectedIndex + 1) % menuButtons.Length;
            SelectButton(selectedIndex);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (menuButtons[selectedIndex] != null)
                menuButtons[selectedIndex].onClick.Invoke();
        }
    }

    void SelectButton(int index)
    {
        if (menuButtons == null || index < 0 || index >= menuButtons.Length) return;

        Button btn = menuButtons[index];
        if (btn == null || EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }

    public void PauseGame()
    {
        isPaused = true;
        IsPaused = true;

        if (playerController != null)
            playerController.enabled = false;

        Time.timeScale = 0f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        selectedIndex = 0;
        StartCoroutine(SelectNextFrame());
    }

    public void ResumeGame()
    {
        isPaused = false;
        IsPaused = false;

        Time.timeScale = 1f;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        StartCoroutine(EnablePlayerNextFrame());
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame clicked");

        isPaused = false;
        IsPaused = false;
        Time.timeScale = 1f;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator SelectNextFrame()
    {
        yield return null;
        SelectButton(selectedIndex);
    }

    IEnumerator EnablePlayerNextFrame()
    {
        yield return null;

        if (playerController != null)
            playerController.enabled = true;
    }
}