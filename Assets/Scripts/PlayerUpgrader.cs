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
    
    private void Awake()
    {
        CheckForFirstLaunch();
    }

    public void UpgradeStrength()
    {
        if (hayCount < strengthCost) return;
        PlayerPrefs.SetFloat("STRENGTH", PlayerPrefs.GetFloat("STRENGTH") + baseStrength);
        hayCount -= strengthCost;
        strengthCost = (int)(strengthCost * costMultiplier);
    }
    
    public void UpgradeSpeed()
    {
        if (hayCount < speedCost) return;
        PlayerPrefs.SetFloat("SPEED", PlayerPrefs.GetFloat("SPEED") + baseSpeed);
        hayCount -= strengthCost;
        strengthCost = (int)(speedCost * costMultiplier);
    }
    
    public void UpgradeDash()
    {
        if (hayCount < dashCost) return;
        PlayerPrefs.SetFloat("DASH", PlayerPrefs.GetFloat("DASH") + baseDash);
        hayCount -= strengthCost;
        strengthCost = (int)(dashCost * costMultiplier);
    }
    
    public void UpgradeHeal()
    {
        if (hayCount < healCost) return;
        PlayerPrefs.SetFloat("HEAL", PlayerPrefs.GetFloat("HEAL") + baseHeal);
        hayCount -= strengthCost;
        strengthCost = (int)(healCost * costMultiplier);
    }
    
    public void UpgradeHealth()
    {
        if (hayCount < healthCost) return;
        PlayerPrefs.SetFloat("HEALTH", PlayerPrefs.GetFloat("HEALTH") + baseHealth);
        hayCount -= strengthCost;
        strengthCost = (int)(healthCost * costMultiplier);
    }
    
    public void UpgradeAgility()
    {
        if (hayCount < agilityCost) return;
        PlayerPrefs.SetFloat("AGILITY", PlayerPrefs.GetFloat("AGILITY") + baseAgility);
        hayCount -= strengthCost;
        strengthCost = (int)(agilityCost * costMultiplier);
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
