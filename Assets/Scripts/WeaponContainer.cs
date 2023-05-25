using UnityEngine;
using UnityEngine.UI;

public class WeaponContainer : MonoBehaviour
{
    [SerializeField] private Image icon;
    public Weapon weapon;

    private void Update()
    {
        if (weapon is null) return;
        icon.sprite = weapon.icon;
    }
}
