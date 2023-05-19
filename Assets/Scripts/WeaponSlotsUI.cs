using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotsUI : MonoBehaviour
{
    public Image[] slots;

    public void ChangeSlot(int slot, Sprite newSlot)
    {
        slots[slot].sprite = newSlot;
    }
}
