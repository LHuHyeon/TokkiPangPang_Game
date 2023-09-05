using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeaponPopup : UI_Popup
{
    enum Gameobjects
    {
        SlotGrid,
    }

    List<UI_WeaponSlotItem> weaponSlots;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        weaponSlots = new List<UI_WeaponSlotItem>();

        BindObject(typeof(Gameobjects));

        SetInfo();

        return true;
    }

    void SetInfo()
    {
        foreach(Transform child in GetObject((int)Gameobjects.SlotGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=1; i<=Managers.Data.Weapon.Count; i++)
        {
            UI_WeaponSlotItem weaponSlot = Managers.UI.MakeSubItem<UI_WeaponSlotItem>(GetObject((int)Gameobjects.SlotGrid).transform);
            weaponSlot.SetInfo(Managers.Data.Weapon[i]);

            weaponSlots.Add(weaponSlot);
        }
    }

    public void PickWeaponCheck()
    {
        foreach(UI_WeaponSlotItem slot in weaponSlots)
        {
            if (slot._weaponItem.isEquip == false)
                slot.ClosePickWeapon();
        }
    }
}
