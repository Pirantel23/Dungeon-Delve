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

    public void UpgradeStrength()
    {
        var t = PlayerPrefs.GetInt("STRENGTH_T");
        if (hayCount < strengthCost || t > 5) return;
        PlayerPrefs.SetFloat("STRENGTH", PlayerPrefs.GetFloat("STRENGTH") * upgradeMultiplier);
        PlayerPrefs.SetInt("STRENGTH_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(strengthCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(strengthIcon, strengthCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }
    
    public void UpgradeSpeed()
    {
        var t = PlayerPrefs.GetInt("SPEED_T");
        if (hayCount < speedCost || t > 5) return;
        PlayerPrefs.SetFloat("SPEED", PlayerPrefs.GetFloat("SPEED") * upgradeMultiplier);
        PlayerPrefs.SetInt("SPEED_T", t + 1);
        hayCount -= speedCost;
        speedCost = (int)(speedCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(speedIcon, speedCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }
    
    public void UpgradeDash()
    {
        var t = PlayerPrefs.GetInt("DASH_T");
        if (hayCount < dashCost || t > 5) return;
        PlayerPrefs.SetFloat("DASH", PlayerPrefs.GetFloat("DASH") + 1);
        PlayerPrefs.SetInt("DASH_T", t + 1);
        hayCount -= dashCost;
        dashCost = (int)(dashCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(dashIcon, dashCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }
    
    public void UpgradeHeal()
    {
        var t = PlayerPrefs.GetInt("HEAL_T");
        if (hayCount < healCost || t > 5) return;
        PlayerPrefs.SetFloat("HEAL", PlayerPrefs.GetFloat("HEAL") * upgradeMultiplier);
        PlayerPrefs.SetInt("HEAL_T", t + 1);
        hayCount -= healCost;
        healCost = (int)(healCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(healIcon, healCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }
    
    public void UpgradeHealth()
    {
        var t = PlayerPrefs.GetInt("HEALTH_T");
        if (hayCount < healthCost || t > 5) return;
        PlayerPrefs.SetFloat("HEALTH", PlayerPrefs.GetFloat("HEALTH") * upgradeMultiplier);
        PlayerPrefs.SetInt("HEALTH_T", t + 1);
        hayCount -= healthCost;
        healthCost = (int)(healthCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(healthIcon, healthCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }
    
    public void UpgradeAgility()
    {
        var t = PlayerPrefs.GetInt("AGILITY_T");
        if (hayCount < agilityCost || t > 5) return;
        PlayerPrefs.SetFloat("AGILITY", PlayerPrefs.GetFloat("AGILITY") * upgradeMultiplier);
        PlayerPrefs.SetInt("AGILITY_T", t + 1);
        hayCount -= agilityCost;
        agilityCost = (int)(agilityCost * costMultiplier);
        player.InitUpgrades();
        ChangeText(agilityIcon, agilityCost.ToString());
        PlayerPrefs.SetInt("HAY", hayCount);
    }

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
        {
            OpenUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CloseUI();
        }
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
