using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Request Panel")]
    public GameObject requestPanel;
    public Image portraitImage;
    public TMP_Text distanceText;
    public TMP_Text rewardText;
    public Button acceptButton;
    public Button declineButton;

    [Header("Timer / Money / Stars")]
    public TMP_Text timerText;
    public TMP_Text moneyText;
    public Image[] starImages;
    public Sprite emptyStarSprite;
    public Sprite filledStarSprite;
    public GameObject moneyIcon;

    [Header("End Panels")]
    public GameObject successPanel;
    public TMP_Text successMoneyText;
    public TMP_Text successStarsText;

    [Header("Passenger In Car UI")]
    public GameObject inCarPanel;
    public Image inCarPortraitImage;
    public Slider inCarPatienceSlider;
    private bool passengerLostPatience = false;
    private float currentPatience = 1f;
    public float CurrentPatience => currentPatience;
    private Coroutine patienceRoutine;
    
    [Header("Pause Menu")]
    public GameObject pausePanel;
    private bool isPaused = false;
    
    private Coroutine flashRoutine;
    private Color defaultTimerColor;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // assign button listeners
        acceptButton.onClick.AddListener(OnAcceptClicked);
        declineButton.onClick.AddListener(OnDeclineClicked);
        
        defaultTimerColor = timerText.color;
    }

    private void Start()
    {
        HidePassengerRequest();
        UpdateMoney(0);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }
    
    // ====== Pause System ======

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void ShowPassengerRequest(PassengerManager.PassengerRequest req)
    {
        if (req == null) return;
        requestPanel.SetActive(true);
        portraitImage.sprite = req.passengerSO.portrait;
        distanceText.text = $"{Mathf.RoundToInt(req.distance)}m";
        rewardText.text = $"${req.reward}";
    }

    public void HidePassengerRequest()
    {
        requestPanel.SetActive(false);
    }

    private void OnAcceptClicked()
    {
        PassengerManager.Instance.AcceptRequest();
    }

    private void OnDeclineClicked()
    {
        PassengerManager.Instance.DeclineRequest();
    }

    public void UpdateTimer(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"{mins:00}:{secs:00}";
            
        // Turn red if 10s or less
        if (seconds <= 10f)
        {
            timerText.color = Color.red;
        }
        else if (flashRoutine == null) // only reset to default if not flashing
        {
            timerText.color = defaultTimerColor;
        }
    }
    
    // Called by GameManager when time is added
    public void FlashTimerGreen()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashGreenRoutine());
    }

    private IEnumerator FlashGreenRoutine()
    {
        timerText.color = Color.green;
        yield return new WaitForSeconds(2f); // flash duration
        timerText.color = defaultTimerColor;
        flashRoutine = null;
    }

    public void UpdateMoney(int money)
    {
        moneyText.text = $"${money}";
    }

    public void UpdateStars(int currentStars)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < currentStars)
                starImages[i].sprite = filledStarSprite;
            else
                starImages[i].sprite = emptyStarSprite;
        }
    }

    public void ShowSuccessPanel(int money, int stars)
    {
        successPanel.SetActive(true);
        successMoneyText.text = $"${money}";
        successStarsText.text = $"{stars}/5";
    }

    public void HideInGameUI()
    {
        requestPanel.SetActive(false);
        
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].gameObject.SetActive(false);
        }
        
        moneyText.enabled = false;
        timerText.enabled = false;
        moneyIcon.SetActive(false);
        inCarPanel.SetActive(false);
    }

    // Called by PassengerManager when player picks passenger up or passenger in car state changes
    public void SetPassengerInCar(bool inCar, PassengerSO passenger)
    {
        inCarPanel.SetActive(inCar);

        if (inCar)
        {
            // Only start coroutine if none is running
            if (patienceRoutine == null && !passengerLostPatience)
            {
                passengerLostPatience = false;
                currentPatience = 1f;

                inCarPortraitImage.sprite = passenger.portrait;
                inCarPatienceSlider.minValue = 0f;
                inCarPatienceSlider.maxValue = 1f;
                inCarPatienceSlider.value = currentPatience;

                patienceRoutine = StartCoroutine(PatienceDecayRoutine());
            }
        }
        else
        {
            // Stop the routine if passenger leaves
            if (patienceRoutine != null)
            {
                StopCoroutine(patienceRoutine);
                patienceRoutine = null;
            }
            inCarPanel.SetActive(false);
        }
    }

    private System.Collections.IEnumerator PatienceDecayRoutine()
    {
        currentPatience = 1f;
        inCarPatienceSlider.value = currentPatience;
        float decayRate = 0.02f; // patience per second

        while (currentPatience > 0f)
        {
            currentPatience -= decayRate * Time.deltaTime; 
            currentPatience = Mathf.Clamp01(currentPatience);
            inCarPatienceSlider.value = currentPatience;

            yield return null;
        }

        if (!passengerLostPatience)
        {
            passengerLostPatience = true;
            GameManager.Instance.LoseStar(1);
        }
        
        patienceRoutine = null;
    }
}
