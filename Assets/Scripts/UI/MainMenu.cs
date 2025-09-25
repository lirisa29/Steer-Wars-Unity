using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Main Buttons")]
    public GameObject[] mainButtons; // Exit, Options, HowToPlay, Shop

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject howToPlayPanel;
    public GameObject shopPanel;

    [Header("Start Prompt")]
    public TextMeshProUGUI startPrompt; // UI Text with "Press Enter to Start"
    public float pulseSpeed = 2f;

    private bool panelOpen = false;
    private Color originalColor;

    void Start()
    {
        // Store original color for pulsing
        if (startPrompt != null)
            originalColor = startPrompt.color;

        // Ensure panels start closed
        optionsPanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    void Update()
    {
        // Animate start prompt (only if no panel is open)
        if (!panelOpen && startPrompt != null)
        {
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            startPrompt.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
        else if (panelOpen && startPrompt != null)
        {
            startPrompt.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        // Check Enter key only if no panel is open
        if (!panelOpen && Input.GetKeyDown(KeyCode.Return))
        {
            LoadGameplay();
        }
    }

    // ===== Button Events =====

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        HideMainButtons();
    }

    public void OpenHowToPlay()
    {
        howToPlayPanel.SetActive(true);
        HideMainButtons();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        HideMainButtons();
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        ShowMainButtons();
    }

    public void ExitGame()
    {
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ===== Helpers =====

    private void HideMainButtons()
    {
        foreach (var btn in mainButtons)
            btn.SetActive(false);

        panelOpen = true;
    }

    private void ShowMainButtons()
    {
        foreach (var btn in mainButtons)
            btn.SetActive(true);

        panelOpen = false;
    }

    private void LoadGameplay()
    {
        SceneManager.LoadScene("Gameplay"); // replace with your scene name
    }
}
