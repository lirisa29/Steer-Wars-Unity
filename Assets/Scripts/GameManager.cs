using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Timer")]
    public float startingTime = 90f;
    public float currentTime;

    [Header("Money & Rating")]
    public int money = 0;
    [Range(0,5)] public int stars = 5;

    [Header("Star decay/gain")]
    public int payPenaltyPerStarLost = 10; // reduce pay by this per star missing

    // internal
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        Debug.Log("Starting stars: " + stars);
        currentTime = startingTime;
        UpdateUI();
    }

    private void Update()
    {
        // tick timer
        if (currentTime > 0f && stars > 0)
        {
            currentTime -= Time.deltaTime;
            UIManager.Instance.UpdateTimer(currentTime);
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                OnTimeExpired();
            }
        }
    }

    public void AddMoney(int amount)
    {
        money += Mathf.Max(0, amount);
        UIManager.Instance.UpdateMoney(money);
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
        UIManager.Instance.UpdateTimer(currentTime);
    }

    public void LoseStar(int amount = 1)
    {
        if (amount <= 0) return;
        stars = Mathf.Max(0, stars - amount);
        UIManager.Instance.UpdateStars(stars);
        if (stars == 0) 
        {
            OnAllStarsLost();
        }
    }

    public void GainStar(int amount = 1)
    {
        if (amount <= 0) return;
        stars = Mathf.Min(5, stars + amount);
        UIManager.Instance.UpdateStars(stars);
    }

    public int GetEffectiveFare(int baseFare)
    {
        // pay reduced by missing stars
        int missing = 5 - stars;
        return Mathf.Max(0, baseFare - missing * payPenaltyPerStarLost);
    }

    private void OnAllStarsLost()
    {
        // Game over
        UIManager.Instance.ShowFailPanel();
    }

    private void OnTimeExpired()
    {
        // Success screen with money and rating
        UIManager.Instance.ShowSuccessPanel(money, stars);
        UIManager.Instance.HideInGameUI();
        Time.timeScale = 0f;
    }

    public void UpdateUI()
    {
        UIManager.Instance.UpdateMoney(money);
        UIManager.Instance.UpdateStars(stars);
        UIManager.Instance.UpdateTimer(currentTime);
    }
}
