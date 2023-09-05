using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer weaponRenderer;

    [SerializeField]
    float jumpGaugeSpeed;               // 점프 게이지 충전 속도
    public float jumpGauge;             // 점프 게이지

    public float jumpSkillValue = 50f;  // 점프 스킬 힘 (날라가는 힘)

    [SerializeField] float attackCooldown = 0.1f;    // 공격 쿨타임
    [SerializeField] float attackDistance = 2f;      // 공격 사거리

    bool isAttack = false;              // 공격 중인가?

    float groundValue = -3.35f;         // 땅 높이

    public GameObject jumpSkillEffect;
    public Rigidbody2D rigid;

    [SerializeField]
    Animator anim;

    [SerializeField]
    private Define.State state = Define.State.Idle;
    public Define.State State
    {
        get { return state; }
        set
        {
            state = value;

            switch (state)
            {
                case Define.State.Idle:
                    anim.CrossFade("Idle", 0.4f);
                    break;
                case Define.State.JumpReady:
                    anim.CrossFade("JumpReady", 0.1f);
                    break;
                case Define.State.Shield:
                    anim.CrossFade("Shield", 0.1f, -1, 0);
                    break;
                case Define.State.Attack:
                    anim.CrossFade("Attack", 0.1f, -1, 0);
                    break;
                case Define.State.Hit:
                    anim.CrossFade("Hit", 0.1f, -1, 0);
                    break;
                case Define.State.Death:
                    anim.CrossFade("Death", 0.1f, -1, 0);
                    break;
            }
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        weaponRenderer.sprite = Managers.Game.Weapon.itemIcon;
    }

    void Update()
    {
        switch (State)
        {
            case Define.State.Idle:        // 가만히 있기
                UpdateIdle();
                break;
            case Define.State.JumpReady:   // 점프 준비
                UpdateJumpReady();
                break;
            case Define.State.Jump:        // 점프
                UpdateJump();
                break;
            case Define.State.Shield:      // 방어
                UpdateShield();
                break;  
            case Define.State.Skill:       // 스킬
                UpdateSkill();
                break;
            case Define.State.Death:       // 죽음
                UpdateDeath();
                break;
        }
    }

#region State

    void UpdateIdle() 
    {
        // 지면과 멀어져 있으면
        if (IsGround() == false)
            State = Define.State.Jump;
    }
    
    // 점프 준비
    void UpdateJumpReady()
    {
        // 시간으로 점프 게이지 충전
        if (jumpGauge > Managers.Game.MaxJumpGauge)
            jumpGauge = Managers.Game.MaxJumpGauge;
        else
            jumpGauge += Time.deltaTime * jumpGaugeSpeed;
    }

    // 점프 중
    void UpdateJump()
    {
        // 지면과 가까우면
        if (IsGround() == true)
            State = Define.State.Idle;
    }

    // 방어
    float shieldTime = 0;
    float maxShieldTime = 2f;
    void UpdateShield()
    {
        shieldTime += Time.deltaTime;

        if (shieldTime >= maxShieldTime)
        {
            State = Define.State.Idle;
            shieldTime = 0;
        }
    }

    // 점프 스킬 전용
    void UpdateSkill()
    {
        // 사정거리 안에 있다면 공격
        if (attackDistance >= Managers.Game.BlockDistance())
            Managers.Game.currentBlock.OnAttacked();
    }

    void UpdateDeath() {}

#endregion

    // 공격 받으면
    [SerializeField] float shieldValue = 5f;    // 튕겨내기 힘
    public void OnAttacked()
    {
        if (state == Define.State.Death)
            return;

        // 방어 중이라면
        if (state == Define.State.Shield)
        {
            Rigidbody2D blockRigid = Managers.Game.currentBlock.GetComponent<Rigidbody2D>();
            blockRigid.velocity = Vector2.zero;
            blockRigid.AddForce(Vector2.up * shieldValue, ForceMode2D.Impulse);

            // 쉴드 쿨타임
            Managers.Game._playPopup.OnShieldCooldown();
            State = Define.State.Idle;
        }
        else
        {
            // 지면과 가까울 때 피격 가능
            if (isHit == false && IsGround() == true)
                StartCoroutine(AttackedCoroutine());
        }
    }

    bool isHit = false;
    [SerializeField] float hitCooldown = 1.5f;
    IEnumerator AttackedCoroutine()
    {
        isHit = true;
        Managers.Game.Hp -= 1;

        if (Managers.Game.Hp <= 0)
        {
            StopAllCoroutines();
            State = Define.State.Death;
        }

        yield return new WaitForSeconds(hitCooldown);

        isHit = false;
    }

    // 공격 시작
    public void OnAttack()
    {
        if (state == Define.State.Death)
            return;

        if (isAttack == true || state == Define.State.JumpReady)
            return;

        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        State = Define.State.Attack;
        isAttack = true;

        yield return new WaitForSeconds(attackCooldown - 0.15f);

        // 사정거리 안에 있다면 공격
        if (attackDistance >= Managers.Game.BlockDistance())
        {
            Managers.Game.currentBlock.OnAttacked();
            Managers.Game._playPopup.AttackSkillGauge();
        }

        yield return new WaitForSeconds(attackCooldown + 0.15f);

        State = Define.State.Idle;
        isAttack = false;
    }

    // 점프 스킬 시작
    public void OnJumpSkill()
    {
        State = Define.State.Skill;

        rigid.AddForce(Vector2.up * jumpSkillValue, ForceMode2D.Impulse);
        StartCoroutine(JumpSkillCoroutine());
    }

    [SerializeField] float jumpSkillTime = 0.8f;
    IEnumerator JumpSkillCoroutine()
    {
        Managers.Resource.Instantiate(jumpSkillEffect, transform);

        yield return new WaitForSeconds(jumpSkillTime);

        State = Define.State.Jump;
    }

    // 공격 스킬 시작
    public void OnAttackSkill()
    {
        State = Define.State.Attack;

        // 현재 착용 중인 무기에 따라 스킬 소환 높이가 다름
        switch (Managers.Game.Weapon.itemId)
        {
            case 1:
                OnSkill(2.5f);
                break;
            case 2:
                OnSkill(1.5f);
                break;
            case 3:
                OnSkill(0.5f);
                break;
            case 4:
                OnSkill(0.5f);
                break;
        }
    }

    void OnSkill(float upValue)
    {
        WeaponItem weaponItem = Managers.Game.Weapon;
        GameObject effect = Managers.Resource.Instantiate(weaponItem.effect, transform);
        effect.transform.position += Vector3.up * upValue;
    }

    void FixedUpdate()
    {
        if (state == Define.State.Death)
            return;

        BlockRayCheck();
    }

    // 제일 가까운 블럭 Ray 체크
    [SerializeField] float maxDistance = 7f;
    void BlockRayCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, maxDistance, 1 << 6); // 6 : Block
        
        if (hit == true)
            Managers.Game.currentBlock = hit.collider.GetComponent<BlockController>();
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (state == Define.State.Death)
            return;

        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            State = Define.State.Idle;
        }
    }

    bool IsGround()
    {
        if (transform.position.y <= groundValue)
            return true;

        return false;
    }
}
