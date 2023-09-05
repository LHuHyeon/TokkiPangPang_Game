using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override bool Init()
    {
		if (base.Init() == false)
			return false;

        SceneType = Define.Scene.Game;
		
		Managers.Game._titlePopup = Managers.UI.ShowPopupUI<UI_TitlePopup>();

		Debug.Log("GameScene : Init()");
		return true;
	}
}
