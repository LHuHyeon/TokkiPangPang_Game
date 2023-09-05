using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitlePopup : UI_Popup
{
    enum Gameobjects
    {
        MiddleLine,
        WeaponButton,
        HomeButton,
        ExitButton,
    }

    enum Images
    {
        WeaponImage,
    }

    public UI_WeaponPopup weaponPopup;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));

        SetInfo();

        return true;
    }

    void FixedUpdate()
    {
        if (GetImage((int)Images.WeaponImage).sprite != Managers.Game.Weapon.itemIcon)
            GetImage((int)Images.WeaponImage).sprite = Managers.Game.Weapon.itemIcon;
    }

    void SetInfo()
    {
        // 가운데 화면 누를 시 게임 시작
        GetObject((int)Gameobjects.MiddleLine).BindEvent(()=>
        {
            Managers.UI.ClosePopupUI(this);
            Managers.Game._playPopup = Managers.UI.ShowPopupUI<UI_PlayPopup>();
        });

        // 홈 버튼은 무기창 닫기 용도
        GetObject((int)Gameobjects.HomeButton).BindEvent(()=>
        {
            if (weaponPopup == null)
                return;

            Managers.UI.ClosePopupUI(weaponPopup);
        });

        // 무기창 띄우기
        GetObject((int)Gameobjects.WeaponButton).BindEvent(()=>
        {
            if (weaponPopup != null)
            {
                Managers.UI.ClosePopupUI(weaponPopup);
                return;
            }

            weaponPopup = Managers.UI.ShowPopupUI<UI_WeaponPopup>();
        });

        // 게임 나가기
        GetObject((int)Gameobjects.ExitButton).BindEvent(()=>
        {
            Application.Quit();
        });
    }
}
