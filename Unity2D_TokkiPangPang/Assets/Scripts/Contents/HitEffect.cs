using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [SerializeField]
    float closeTime = 0.8f;

    void OnEnable()
    {
        StartCoroutine(CloseEffect());
    }

    IEnumerator CloseEffect()
    {
        yield return new WaitForSeconds(closeTime);

        Managers.Resource.Destroy(gameObject);
    }
}
