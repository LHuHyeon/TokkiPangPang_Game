using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public Dictionary<int, WeaponItem> Weapon { get; private set; }

    public void Init()
    {
        Weapon = new WeaponItemData().WeaponItemRequest();
    }
}