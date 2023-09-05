using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Define
{
	public enum State
	{
		Idle,
		JumpReady,
		Jump,
		Shield,
		Attack,
		Skill,
		Hit,
		Death,
	}

    public enum UIEvent
	{
		Click,
		Pressed,
		PointerDown,
		PointerUp,
	}

    public enum Scene
	{
		Unknown,
		Dev,
		Game,
	}

	public enum Sound
	{
		Bgm,
		Effect,
		Speech,
		Max,
	}

	public const float Ground = -3.4f;
}
