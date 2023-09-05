using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int blockScore;  // 점수

    [SerializeField]
    GameObject hitEffect;

    void FixedUpdate()
    {
        OnAttack();
    }

    [SerializeField] float hitDistance = 0.5f;
    void OnAttack()
    {
        Debug.DrawRay(transform.position, Vector3.up * hitDistance, Color.red, 1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, hitDistance, 1 << 7); // 7 : Player

        if (hit == true)
            Managers.Game.player.OnAttacked();
    }

    // 피격 받기
    public void OnAttacked()
    {
        hp -= Managers.Game.Attack;

        if (hp <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Managers.Game.Score += blockScore;
        Managers.Resource.Instantiate(hitEffect).transform.position = this.transform.position;
        Managers.Game.Despawn(gameObject);
    }
}
