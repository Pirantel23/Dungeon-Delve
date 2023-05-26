using System;
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
    private int hayCount;
    private PlayerController player;
    
    private void Awake()
    {
        CheckForFirstLaunch();
        player = FindObjectOfType<PlayerController>();
    }

    public void UpgradeStrength()
    {
        var t = PlayerPrefs.GetInt("STRENGTH_T");
        if (hayCount < strengthCost || t > 5) return;
        PlayerPrefs.SetFloat("STRENGTH", PlayerPrefs.GetFloat("STRENGTH") + baseStrength);
        PlayerPrefs.SetInt("STRENGTH_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(strengthCost * costMultiplier);
        player.InitUpgrades();
    }
    
    public void UpgradeSpeed()
    {
        var t = PlayerPrefs.GetInt("SPEED_T");
        if (hayCount < speedCost || t > 5) return;
        PlayerPrefs.SetFloat("SPEED", PlayerPrefs.GetFloat("SPEED") + baseSpeed);
        PlayerPrefs.SetInt("SPEED_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(speedCost * costMultiplier);
        player.InitUpgrades();
    }
    
    public void UpgradeDash()
    {
        var t = PlayerPrefs.GetInt("DASH_T");
        if (hayCount < dashCost || t > 5) return;
        PlayerPrefs.SetFloat("DASH", PlayerPrefs.GetFloat("DASH") + baseDash);
        PlayerPrefs.SetInt("DASH_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(dashCost * costMultiplier);
        player.InitUpgrades();
    }
    
    public void UpgradeHeal()
    {
        var t = PlayerPrefs.GetInt("HEAL_T");
        if (hayCount < healCost || t > 5) return;
        PlayerPrefs.SetFloat("HEAL", PlayerPrefs.GetFloat("HEAL") + baseHeal);
        PlayerPrefs.SetInt("HEAL_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(healCost * costMultiplier);
        player.InitUpgrades();
    }
    
    public void UpgradeHealth()
    {
        var t = PlayerPrefs.GetInt("HEALTH_T");
        if (hayCount < healthCost || t > 5) return;
        PlayerPrefs.SetFloat("HEALTH", PlayerPrefs.GetFloat("HEALTH") + baseHealth);
        PlayerPrefs.SetInt("HEALTH_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(healthCost * costMultiplier);
        player.InitUpgrades();
    }
    
    public void UpgradeAgility()
    {
        var t = PlayerPrefs.GetInt("AGILITY_T");
        if (hayCount < agilityCost || t > 5) return;
        PlayerPrefs.SetFloat("AGILITY", PlayerPrefs.GetFloat("AGILITY") + baseAgility);
        PlayerPrefs.SetInt("AGILITY_T", t + 1);
        hayCount -= strengthCost;
        strengthCost = (int)(agilityCost * costMultiplier);
        player.InitUpgrades();
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
        upgradeUI.enabled = true;
        regularUI.enabled = false;
        hayCount = PlayerPrefs.GetInt("HAY");
    }

    private void CloseUI()
    {
        upgradeUI.enabled = false;
        regularUI.enabled = true;
    }
}
