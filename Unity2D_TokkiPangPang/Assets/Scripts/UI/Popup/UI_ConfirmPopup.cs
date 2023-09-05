using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ConfirmPopup : UI_Popup
{
    enum Buttons
    {
        YesButton,
        NoButton,
    }

    [SerializeField]
    TextMeshProUGUI _Messagetext;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }
    
    Action _onClickYesButton;
    public void SetInfo(Action onClickYesButton, string text)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        
        _onClickYesButton = onClickYesButton;
        _Messagetext.text = text;
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);
        if (_onClickYesButton != null)
            _onClickYesButton.Invoke();
    }

    void OnClickNoButton()
    {
        Time.timeScale = 1; // 일시 정지 되어 있을 수도 있기 때문에
        Managers.UI.ClosePopupUI(this);
    }
}
