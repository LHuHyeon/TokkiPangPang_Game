using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using static Define;

public class Utils
{
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    // 숫자 단위 붙이기
    public static string GetNumberUnitText(int number)
    {
        if (number.ToString().Length <= 4)
            return (number == 0) ? "0" : GetCommaText(number);

        // 숫자 구성 단위
        string[] unit = new string[] { "", "K", "M", "G", "T", "P", "E", "Z"};

        // 3칸씩 숫자 자리 지정 
        string num = string.Format("{0:# ### ### ### ### ### ### ### ###}", number).TrimStart().Replace(" ", ",");
        string[] str = num.Split(',');

        int cnt = str.Length - 1;
        int strNum = Convert.ToInt32(str[0]);

        string result = "";
        // 두자리 수까진 소수점 붙이기
        if (strNum.ToString().Length <= 2 && cnt > 0)
            result = strNum + "." + str[1].Substring(0, 1) + unit[cnt];
        else
            result = strNum + unit[cnt];

        return result;
    }

    // 콤마(,) 붙이기
    public static string GetCommaText(int number)
    {
        return string.Format("{0:#,###}", number);
    }
}
