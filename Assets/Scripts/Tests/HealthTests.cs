using NUnit.Framework;
using UnityEngine;

public class HealthTests
{
    [Test]
    public void GetAmount_ReturnsCorrectValue()
    {
        var expectedAmount = 100f;
        var health = CreateHealth(expectedAmount);
        var actualAmount = health.GetAmount();

        Assert.AreEqual(expectedAmount, actualAmount);
    }

    [Test]
    public void SetNewMaxHealth_SetsMaxHealth()
    {
        var startingAmount = 100f;
        var newMaxHealth = 200f;
        var health = CreateHealth(startingAmount);

        health.SetNewMaxHealth(newMaxHealth);

        Assert.AreEqual(newMaxHealth, health.GetAmount());
    }

    [Test]
    public void Heal_IncreasesAmount()
    {
        var maxAmount = 100f;
        var healAmount = 20f;
        var startAmount = 50f;
        var health = CreateHealth(maxAmount);
        health.ChangeAmount(50f);
        
        health.Heal(healAmount);

        Assert.AreEqual(50 + healAmount, health.GetAmount());
    }

    [Test]
    public void Heal_DoesNotExceedMaxHealth()
    {
        var startingAmount = 80f;
        var maxHealth = 100f;
        var healAmount = 30f;
        var health = CreateHealth(startingAmount);
        health.SetNewMaxHealth(maxHealth);

        health.Heal(healAmount);

        Assert.AreEqual(maxHealth, health.GetAmount());
    }

    private Health CreateHealth(float startingAmount)
    {
        var gameObject = new GameObject();
        var health = gameObject.AddComponent<Health>();
        health.SetNewMaxHealth(startingAmount);

        return health;
    }
}