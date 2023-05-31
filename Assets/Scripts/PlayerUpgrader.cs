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

    private void UpgradeParameter(string parameterName, int parameterCost, TextMeshProUGUI textObject)
    {
        var t = PlayerPrefs.GetInt($"{parameterName}_T");
        if (hayCount < parameterCost || t > 5) return;
        PlayerPrefs.SetFloat(parameterName, PlayerPrefs.GetFloat(parameterName) * upgradeMultiplier);
        PlayerPrefs.SetInt($"{parameterName}_T", t + 1);
        hayCount -= parameterCost;
        parameterCost = (int)(parameterCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(textObject, parameterCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }

    public void UpgradeStrength() =>
        UpgradeParameter("STRENGTH_T", strengthCost, strengthIcon);


    public void UpgradeSpeed() =>
        UpgradeParameter("SPEED_T", speedCost, speedIcon);

    public void UpgradeDash() =>
        UpgradeParameter("DASH_T", dashCost, dashIcon);

    public void UpgradeHeal() =>
        UpgradeParameter("HEAL_T", healCost, healIcon);

    public void UpgradeHealth() =>
        UpgradeParameter("HEALTH_T", healthCost, healthIcon);

    public void UpgradeAgility() =>
        UpgradeParameter("AGILITY_T", agilityCost, agilityIcon);

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

        PlayerPrefs.SetInt("GOLD", 0);
        PlayerPrefs.SetInt("HAY", 100);

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
    }

    private void CloseUI()
    {
        upgradeUI.gameObject.SetActive(false);
        regularUI.gameObject.SetActive(true);
    }
}