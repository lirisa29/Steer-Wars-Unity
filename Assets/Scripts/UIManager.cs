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

    [Header("End Panels")]
    public GameObject successPanel;
    public TMP_Text successMoneyText;
    public TMP_Text successStarsText;

    [Header("Passenger In Car UI")]
    public GameObject inCarPanel;
    public Image inCarPortraitImage;
    public Image inCarPatienceSlider;
    private bool passengerLostPatience = false;
    private float currentPatience = 1f;
    public float CurrentPatience => currentPatience;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        // assign button listeners
        acceptButton.onClick.AddListener(OnAcceptClicked);
        declineButton.onClick.AddListener(OnDeclineClicked);
    }

    private void Start()
    {
        HidePassengerRequest();
        UpdateMoney(0);
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
    }

    // Called by PassengerManager when player picks passenger up or passenger in car state changes
    public void SetPassengerInCar(bool inCar, PassengerSO passenger)
    {
        inCarPanel.SetActive(inCar);
        if (inCar)
        {
            passengerLostPatience = false;
            inCarPortraitImage.sprite = passenger.portrait;
            inCarPatienceSlider.fillAmount = currentPatience;
            StartCoroutine(PatienceDecayRoutine(passenger));
        }
        else
        {
            StopAllCoroutines();
            inCarPanel.SetActive(false);
        }
    }

    private System.Collections.IEnumerator PatienceDecayRoutine(PassengerSO passenger)
    {
        currentPatience = 1f; // reset on new passenger
        float decayRate = 0.02f; 

        while (true)
        {
            currentPatience -= decayRate * Time.deltaTime;
            inCarPatienceSlider.fillAmount = currentPatience;

            if (currentPatience <= 0f && !passengerLostPatience)
            {
                passengerLostPatience = true;
                GameManager.Instance.LoseStar(1);
                yield break;
            }
            yield return null;
        }
    }
}
