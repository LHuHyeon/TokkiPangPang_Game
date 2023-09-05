using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponItem
{
    public int itemId;
    public string itemName;
    public Sprite itemIcon;
    public Sprite skillIcon;
    public GameObject effect;

    public bool isEquip = false;
}

public class WeaponItemData
{
    public Dictionary<int, WeaponItem> WeaponItemRequest()
    {
        Dictionary<int, WeaponItem> weaponDic = new Dictionary<int, WeaponItem>();

        TextAsset weaponDataFile = Resources.Load<TextAsset>("Data/WeaponData");
        string[] lines = weaponDataFile.text.Split("\n");

        for(int y=1; y<lines.Length; y++)
        {
            string[] row = lines[y].Replace("\r", "").Split(',');
            if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

            WeaponItem weaponItem = new WeaponItem()
            {
                itemId = int.Parse(row[0]),
                itemName = row[1],
                itemIcon = Managers.Resource.Load<Sprite>("Art/UI/ItemIcon/"+row[2]),
                skillIcon = Managers.Resource.Load<Sprite>("Art/UI/SkillIcon/"+row[3]),
                effect = Managers.Resource.Load<GameObject>("Prefabs/Effect/Skill/"+row[4]),
            };

            weaponDic.Add(weaponItem.itemId, weaponItem);
        }

        Managers.Game.Weapon = weaponDic[1];
        Managers.Game.Weapon.isEquip = true;

        return weaponDic;
    }
}