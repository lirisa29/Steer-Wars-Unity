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
    [Range(0,5)] public int stars = 0;

    [Header("Stars & Tips")]
    public int maxStars = 5;
    public int tipBonusPerStar = 20;

    // internal
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        currentTime = startingTime;
        UpdateUI();
    }

    private void Update()
    {
        // tick timer
        if (currentTime > 0f)
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
    }

    public void GainStar(int amount = 1)
    {
        if (amount <= 0) return;
        int oldStars = stars;
        stars = Mathf.Min(maxStars, stars + amount);
        if (stars > oldStars)
        {
            // reward tip for gaining a star
            AddMoney(tipBonusPerStar);
        }
        UIManager.Instance.UpdateStars(stars);
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
