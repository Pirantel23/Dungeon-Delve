using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PlayerUpgrader : MonoBehaviour
{
    [SerializeField] private Canvas upgradeUI;
    [SerializeField] private Canvas regularUI;

    [SerializeField] private float baseStrength;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float baseDash;
    [SerializeField] private float baseHeal;
    [SerializeField] private float baseHealth;
    [SerializeField] private float baseAgility;

    [SerializeField] private int strengthCost;
    [SerializeField] private int speedCost;
    [SerializeField] private int dashCost;
    [SerializeField] private int healCost;
    [SerializeField] private int healthCost;
    [SerializeField] private int agilityCost;

    [SerializeField] private float costMultiplier;
    [SerializeField] private float upgradeMultiplier;

    [SerializeField] private TextMeshProUGUI strengthIcon;
    [SerializeField] private TextMeshProUGUI healthIcon;
    [SerializeField] private TextMeshProUGUI healIcon;
    [SerializeField] private TextMeshProUGUI agilityIcon;
    [SerializeField] private TextMeshProUGUI dashIcon;
    [SerializeField] private TextMeshProUGUI speedIcon;

    [SerializeField] private TextMeshProUGUI hayIcon;

    [SerializeField] private GameObject sprite;

    private int hayCount;
    private PlayerController player;

    private void Awake()
    {
        hayCount = PlayerPrefs.GetInt("HAY");
        CheckForFirstLaunch();
        player = FindObjectOfType<PlayerController>();
        ChangeText(strengthIcon, strengthCost.ToString());
        ChangeText(agilityIcon, agilityCost.ToString());
        ChangeText(healIcon, healCost.ToString());
        ChangeText(healthIcon, healthCost.ToString());
        ChangeText(speedIcon, speedCost.ToString());
        ChangeText(dashIcon, dashCost.ToString());
    }

    private void Update()
    {
        hayIcon.text = hayCount.ToString();
    }

    private void ChangeText(TextMeshProUGUI text, string newValue) => text.text = newValue;

    private void UpgradeParameter(string parameterName, int parameterCost, TextMeshProUGUI textObject, bool linearIncrement)
    {
        var t = PlayerPrefs.GetInt($"{parameterName}_T");
        if (hayCount < parameterCost || t > 4) return;
        PlayerPrefs.SetFloat(parameterName,
            linearIncrement
                ? PlayerPrefs.GetFloat(parameterName) + 1
                : PlayerPrefs.GetFloat(parameterName) * upgradeMultiplier);
        PlayerPrefs.SetInt($"{parameterName}_T", t + 1);
        hayCount -= parameterCost;
        parameterCost = (int)(parameterCost * costMultiplier);
        switch (parameterName)
        {
            case "STRENGTH":
                strengthCost = parameterCost;
                break;
            case "SPEED":
                speedCost = parameterCost;
                break;
            case "DASH":
                dashCost = parameterCost;
                break;
            case "HEAL":
                healCost = parameterCost;
                break;
            case "HEALTH":
                healthCost = parameterCost;
                break;
            case "AGILITY":
                agilityCost = parameterCost;
                break;
        }
        player.InitUpgrades();
        ChangeText(textObject, parameterCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }

    public void UpgradeStrength() =>
        UpgradeParameter("STRENGTH", strengthCost, strengthIcon, false);


    public void UpgradeSpeed() =>
        UpgradeParameter("SPEED", speedCost, speedIcon, false);

    public void UpgradeDash() =>
        UpgradeParameter("DASH", dashCost, dashIcon, true);

    public void UpgradeHeal() =>
        UpgradeParameter("HEAL", healCost, healIcon, false);

    public void UpgradeHealth() =>
        UpgradeParameter("HEALTH", healthCost, healthIcon, false);

    public void UpgradeAgility() =>
        UpgradeParameter("AGILITY", agilityCost, agilityIcon, false);

    private void CheckForFirstLaunch()
    {
        var hasPlayed = PlayerPrefs.GetInt("PLAYED");
        if (hasPlayed != 0) return;
        PlayerPrefs.SetFloat("STRENGTH", baseStrength);
        PlayerPrefs.SetFloat("SPEED", baseSpeed);
        PlayerPrefs.SetFloat("DASH", baseDash);
        PlayerPrefs.SetFloat("HEAL", baseHeal);
        PlayerPrefs.SetFloat("HEALTH", baseHealth);
        PlayerPrefs.SetFloat("AGILITY", baseAgility);

        PlayerPrefs.SetInt("STRENGTH_T", 0);
        PlayerPrefs.SetInt("SPEED_T", 0);
        PlayerPrefs.SetInt("DASH_T", 0);
        PlayerPrefs.SetInt("HEAL_T", 0);
        PlayerPrefs.SetInt("HEALTH_T", 0);
        PlayerPrefs.SetInt("AGILITY_T", 0);

        PlayerPrefs.SetInt("HAY", 10000);

        PlayerPrefs.SetInt("PLAYED", 1);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.E))
            OpenUI();
        else if (other.CompareTag("Player")) sprite.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CloseUI();
        sprite.SetActive(false);
    }

    private void OpenUI()
    {
        upgradeUI.gameObject.SetActive(true);
        regularUI.gameObject.SetActive(false);
        hayCount = PlayerPrefs.GetInt("HAY");
        agilityCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("AGILITY_T")+1);
        dashCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("DASH_T")+1);
        healCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("HEAL_T")+1);
        healthCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("HEALTH_T")+1);
        speedCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("SPEED_T")+1);
        strengthCost = (int)Mathf.Pow(2, PlayerPrefs.GetInt("STRENGTH_T")+1);
        
        ChangeText(agilityIcon, agilityCost.ToString());
        ChangeText(dashIcon, dashCost.ToString());
        ChangeText(healIcon, healCost.ToString());
        ChangeText(healthIcon, healthCost.ToString());
        ChangeText(speedIcon, speedCost.ToString());
        ChangeText(strengthIcon, strengthCost.ToString());
    }

    private void CloseUI()
    {
        upgradeUI.gameObject.SetActive(false);
        regularUI.gameObject.SetActive(true);
    }
}