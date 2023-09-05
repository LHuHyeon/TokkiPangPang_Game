using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class GameData
{
    public int Hp;
    public int MaxHp;
    public int Attack;

	public int MaxJumpGauge; // 최대 점프 게이지
	public int Score;

	public WeaponItem Weapon;
}

public class GameManagerEx
{
    GameData _gameData = new GameData();

	public PlayerController player;

	public BlockController currentBlock;

	public UI_PlayPopup _playPopup;
	public UI_TitlePopup _titlePopup;

	HashSet<GameObject> _blocks = new HashSet<GameObject>();
	public Action<int> OnSpawnEvent;

    public int Hp
	{
		get { return _gameData.Hp; }
		set 
		{ 
			_gameData.Hp = Mathf.Clamp(value, 0, MaxHp);

			if (_playPopup != null)
				_playPopup.SetHp();
		}
	}

    public int MaxHp
	{
		get { return _gameData.MaxHp; }
		set { _gameData.MaxHp = value; }
	}

    public int Attack
	{
		get { return _gameData.Attack; }
		set { _gameData.Attack = value; }
	}

    public int Score
	{
		get { return _gameData.Score; }
		set { _gameData.Score = value; }
	}

	public int MaxJumpGauge
	{
		get { return _gameData.MaxJumpGauge; }
		set { _gameData.MaxJumpGauge = value; }
	}

	public WeaponItem Weapon
	{
		get { return _gameData.Weapon; }
		set { _gameData.Weapon = value; }
	}

    public void Init()
    {
		Debug.Log("GameManager : Init()");
		MaxHp = 3;
		Hp = MaxHp;
		Attack = 5;
		Score = 0;
		MaxJumpGauge = 30;
    }

	// 블럭과 거리가 가까운지 체크
	public float BlockDistance()
	{
		if (currentBlock == null)
			return 100;

		return (currentBlock.transform.position - Managers.Game.player.transform.position).magnitude;
	}

	// Block 소환
    public GameObject Spawn(GameObject obj, Transform parent = null)
    {
		GameObject go = Managers.Resource.Instantiate(obj, parent);

		_blocks.Add(go);

		if (OnSpawnEvent != null)
			OnSpawnEvent.Invoke(1);

        return go;
    }

    // Block 삭제
    public void Despawn(GameObject go)
    {
		if (_blocks.Contains(go))	// 존재 여부 확인
		{
			_blocks.Remove(go);
			if (OnSpawnEvent != null)
				OnSpawnEvent.Invoke(-1);
		}

        Managers.Resource.Destroy(go);
    }
}
