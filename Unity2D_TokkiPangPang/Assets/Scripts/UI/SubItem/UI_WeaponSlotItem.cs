using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WeaponSlotItem : UI_Base
{
    enum Images
    {
        WeaponImage,
        PickItemImage,
    }

    public WeaponItem _weaponItem;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));

        GetImage((int)Images.PickItemImage).gameObject.SetActive(false);

        return true;
    }

    public void SetInfo(WeaponItem weaponItem)
    {
        Init();
        _weaponItem = weaponItem;

        if (_weaponItem.isEquip == true)
            OnPickWeapon();

        GetImage((int)Images.WeaponImage).sprite = _weaponItem.itemIcon;

        // 슬롯 클릭 시 장착 확인
        gameObject.BindEvent(()=>
        {
            if (_weaponItem.isEquip == true)
                return;

            Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
            {
                OnPickWeapon();
            }, "무기를 장착하겠습니까?");
        });
    }

    public void OnPickWeapon()
    {
        Managers.Game.Weapon.isEquip = false;

        _weaponItem.isEquip = true;
        Managers.Game.Weapon = _weaponItem;
        GetImage((int)Images.PickItemImage).gameObject.SetActive(true);

        Managers.Game._titlePopup.weaponPopup.PickWeaponCheck();
    }

    public void ClosePickWeapon()
    {
        _weaponItem.isEquip = false;
        GetImage((int)Images.PickItemImage).gameObject.SetActive(false);
    }
}
