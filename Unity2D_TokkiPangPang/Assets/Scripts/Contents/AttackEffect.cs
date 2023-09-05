using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField]
    float closeTime = 1f;

    void OnEnable()
    {
        StartCoroutine(CloseEffect());
    }

    IEnumerator CloseEffect()
    {
        yield return new WaitForSeconds(closeTime);

        Managers.Resource.Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            other.GetComponent<BlockController>().OnAttacked();
        }
    }
}
