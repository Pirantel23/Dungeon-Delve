using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private Image slot1;
    [SerializeField] private Image slot2;
    private PlayerController player;
    public Sprite activeSlot;
    public Sprite offSlot;
    private bool changingToSlot1;
    private bool changingToSlot2;
    private bool scrolling;
    private int currentSlot = 1;
    public WeaponContainer weapon1;
    public WeaponContainer weapon2;
    public bool changingWeapon;
    private float weaponChangeCooldown = 0.5f;
    
    private void Start()
    {
        weapon1 = slot1.GetComponent<WeaponContainer>();
        weapon2 = slot2.GetComponent<WeaponContainer>();
        player = FindObjectOfType<PlayerController>();
        Debug.Log(player.weapon);
    }
    
    private void Update()
    {
        UpdateSlot();
        GetInput();
        if (changingToSlot1) currentSlot = 1;
        if (changingToSlot2) currentSlot = 2;
        if (scrolling) currentSlot = currentSlot == 2 ? 1 : 2;
    }

    private void GetInput()
    {
        changingToSlot1 = Input.GetKeyDown(KeyCode.Alpha1);
        changingToSlot2 = Input.GetKeyDown(KeyCode.Alpha2);
        scrolling = Input.mouseScrollDelta.y != 0;
    }

    private void UpdateSlot()
    {
        if (currentSlot == 1)
        {
            slot1.sprite = activeSlot;
            slot2.sprite = offSlot;
            player.weapon = weapon1.weapon;
        }
        else
        {
            slot2.sprite = activeSlot;
            slot1.sprite = offSlot;
            player.weapon = weapon2.weapon;
        }
    }

    public void TryChangeWeapon(Weapon weapon)
    {
        if (changingWeapon) return;
        StartCoroutine(ChangeWeapon(weapon));
    }
    
    public IEnumerator ChangeWeapon(Weapon weapon)
    {
        Debug.Log($"changing to {weapon} from {weapon1.weapon} and {changingWeapon}");
        changingWeapon = true;
        if (currentSlot == 1)
        {
            if (weapon1.weapon is not null)
            {
                var a = Instantiate(weapon1.weapon.prefab, player.transform.position, Quaternion.identity)
                    .GetComponent<Weapon>();
                a.inUI = false;
                a.sprite.enabled = true;
            }
            weapon1.weapon = weapon;
            weapon.sprite.enabled = false;
            weapon.inUI = true;
        }
        else
        {
            if (weapon2.weapon is not null)
            {
                var a = Instantiate(weapon2.weapon.prefab, player.transform.position, Quaternion.identity)
                    .GetComponent<Weapon>();
                a.inUI = false;
                a.sprite.enabled = true;
            }
            weapon2.weapon = weapon;
            weapon.sprite.enabled = false;
            weapon.inUI = true;
        }

        yield return new WaitForSeconds(weaponChangeCooldown);
        changingWeapon = false;
    }
}
