using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PlayPopup : UI_Popup
{
    enum Gameobjects
    {
        HpBar,
    }

    enum Buttons
    {
        JumpButton,
        ShieldButton,
        AttackButton,
        PauseButton,
        JumpSkillButton,
        AttackSkillButton,
    }

    enum Sliders
    {
        BlockHpBar,
        JumpSkillSlider,
        AttackSkillSlider,
    }

    enum Images
    {
        AttackImage,
        ShieldCooldownImage,
        JumpSkillImage,
        AttackSkillButton,
        AttackSkillImage,
    }

    enum Texts
    {
        ScoreText,
        BlockHpText,
    }

    GameObject hpIcon;
    GameObject spawning;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindSlider(typeof(Sliders));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        
        SetInfo();

        Managers.Game.player = Managers.Resource.Instantiate("Player").GetComponent<PlayerController>();
        spawning = Managers.Resource.Instantiate("Spawning");

        Debug.Log("UI_PlayPopup : Init()");

        return true;
    }

    void Update()
    {
        RefreshUI();
        ShieldCooldown();
        TestKey();
    }

#region 기본 버튼

    // 마우스 누르면 점프 준비
    void OnPointerDownJumpButton()
    {
        // 가만히 있다면
        if (Managers.Game.player.State == Define.State.Idle)
        {
            Managers.Game.player.jumpGauge = 0;
            Managers.Game.player.State = Define.State.JumpReady;
        }
    }

    // 마우스 때면 점프 시작
    void OnPointerUpJumpButton()
    {
        // 점프 준비 중이라면
        if (Managers.Game.player.State == Define.State.JumpReady)
        {
            // 플레이어 위로 날리기
            Debug.Log("jumpGauge : " + Managers.Game.player.jumpGauge);
            Managers.Game.player.rigid.AddForce(Vector2.up * Managers.Game.player.jumpGauge, ForceMode2D.Impulse);
            
            // 점프 스킬 게이지 증가
            JumpSkillGauge(Managers.Game.player.jumpGauge);
            Managers.Game.player.State = Define.State.Jump;
        }
    }

    // 방어 버튼
    void OnClickShieldButton()
    {
        if (isCooldown == false)
            Managers.Game.player.State = Define.State.Shield;
    }

    // 공격 버튼
    void OnClickAttackButton()
    {
        Managers.Game.player.OnAttack();
    }

    // 일시 정지 버튼
    bool isPause = false;
    void OnClickPauseButton()
    {
        Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
        {
            Time.timeScale = 1;
            isPause = false;
            Managers.Game._titlePopup = Managers.UI.ShowPopupUI<UI_TitlePopup>();
            Clear();
        }, "로비로 이동하겠습니까?");

        // 일시 정지 활성화
        if (isPause == false)
        {
            Time.timeScale = 0;
            isPause = true;
            return;
        }
    }

#endregion

#region 공격 스킬

    // 공격 스킬 게이지 증가
    public void AttackSkillGauge()
    {
        Image attackSkillImage = GetImage((int)Images.AttackSkillImage);

        // Block 피격 시 스킬 0.1씩 게이지 증가
        attackSkillImage.fillAmount += 0.1f;
        if (attackSkillImage.fillAmount >= 1)
        {
            attackSkillImage.fillAmount = 0;
            GetSlider((int)Sliders.AttackSkillSlider).gameObject.SetActive(true);
        }
    }

    // 공격 스킬 슬라이더 체크
    void OnChangeValueAttackSkill(float value)
    {
        Slider slider = GetSlider((int)Sliders.AttackSkillSlider);

        // 슬라이더 끝까지 땡기면 점프 스킬 발동
        if (slider.value == 1f)
        {
            Managers.Game.player.OnAttackSkill();

            slider.gameObject.SetActive(false);
            GetImage((int)Images.AttackSkillImage).fillAmount = 0;
        }

        slider.value = 0;
    }

    // 공격 스킬 버튼 (일반 공격)
    void OnClickAttackSkillButton()
    {
        if (GetSlider((int)Sliders.AttackSkillSlider).value == 0)
            OnClickAttackButton();
    }

#endregion

#region 점프 스킬

    // 점프 스킬 게이지 증가
    void JumpSkillGauge(float jumpGauge)
    {
        Image jumpSkilImage = GetImage((int)Images.JumpSkillImage);

        // 점프 횟수가 일정 수치 도달하면 점프 스킬 활성화
        float gauge = jumpGauge / 100;
        jumpSkilImage.fillAmount += Mathf.Clamp(gauge, 0, 1);

        if (jumpSkilImage.fillAmount >= 1)
        {
            jumpSkilImage.fillAmount = 0;
            GetSlider((int)Sliders.JumpSkillSlider).gameObject.SetActive(true);
        }
    }

    // 점프 스킬 슬라이더 체크
    bool isClickJumpSkill = false;    // 점프 스킬 체크
    void OnChangeValueJumpSkill(float value)
    {
        if (isClickJumpSkill == true)
            return;

        Slider slider = GetSlider((int)Sliders.JumpSkillSlider);

        // 슬라이더 끝까지 땡기면 점프 스킬 발동
        if (slider.value == 1f)
        {
            Managers.Game.player.OnJumpSkill();

            slider.gameObject.SetActive(false);
            GetImage((int)Images.JumpSkillImage).fillAmount = 0;
        }

        slider.value = 0;
    }

    // 점프 스킬 클릭 시 점프 준비
    void OnSliderDownJumpSkill()
    {
        isClickJumpSkill = true;
        OnPointerDownJumpButton();
    }

    // 점프 스킬 클릭 때면
    void OnSliderUpJumpSkill()
    {
        isClickJumpSkill = false;

        // 슬라이더가 움직이지 않았다면 일반 점프
        if (GetSlider((int)Sliders.JumpSkillSlider).value == 0)
            OnPointerUpJumpButton();
    }

#endregion

    // 쉴드 시작
    bool isCooldown = false;
    public void OnShieldCooldown()
    {
        isCooldown = true;
    }

    float cooldownValue = 2.5f;
    void ShieldCooldown()
    {
        if (isCooldown == true)
        {
            Image cooldownImage = GetImage((int)Images.ShieldCooldownImage);

            if (cooldownImage.gameObject.activeSelf == false)
            {
                cooldownImage.gameObject.SetActive(true);
            }

            cooldownImage.fillAmount -= 1 * Time.smoothDeltaTime / cooldownValue;
            
            if (cooldownImage.fillAmount <= 0)
            {
                isCooldown = false;
                cooldownImage.fillAmount = 1;
                cooldownImage.gameObject.SetActive(false);
            }
        }
    }

    // 체력에 따른 하트 아이콘 개수 적용
    public void SetHp()
    {
        if (Managers.Game.Hp < 0)
            return;

        if (hpIcon == null)
            hpIcon = Managers.Resource.Instantiate("UI/Icon/HpIcon", GetObject((int)Gameobjects.HpBar).transform);

        foreach(Transform child in GetObject((int)Gameobjects.HpBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<Managers.Game.Hp; i++)
            Managers.Resource.Instantiate("UI/Icon/HpIcon", GetObject((int)Gameobjects.HpBar).transform);
    }

    // 컴퓨터로 조작하는 키
    void TestKey()
    {
        if (Input.GetKeyDown(KeyCode.D))
            OnPointerDownJumpButton();
        if (Input.GetKeyUp(KeyCode.D))
            OnPointerUpJumpButton();
        
        if (Input.GetKeyDown(KeyCode.G))
            OnClickShieldButton();
        if (Input.GetKeyDown(KeyCode.J))
            OnClickAttackButton();
    }

    void SetInfo()
    {
        GetButton((int)Buttons.ShieldButton).gameObject.BindEvent(OnClickShieldButton);
        GetButton((int)Buttons.AttackButton).gameObject.BindEvent(OnClickAttackButton);
        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(OnClickPauseButton);

        GetButton((int)Buttons.JumpButton).gameObject.BindEvent(OnPointerDownJumpButton, Define.UIEvent.PointerDown);
        GetButton((int)Buttons.JumpButton).gameObject.BindEvent(OnPointerUpJumpButton, Define.UIEvent.PointerUp);

        GetButton((int)Buttons.JumpSkillButton).gameObject.BindEvent(OnSliderDownJumpSkill, Define.UIEvent.PointerDown);
        GetButton((int)Buttons.JumpSkillButton).gameObject.BindEvent(OnSliderUpJumpSkill, Define.UIEvent.PointerUp);

        GetButton((int)Buttons.AttackSkillButton).gameObject.BindEvent(OnClickAttackSkillButton);

        GetButton((int)Buttons.PauseButton).gameObject.BindEvent(OnClickPauseButton);

        GetSlider((int)Sliders.JumpSkillSlider).onValueChanged.AddListener(OnChangeValueJumpSkill);
        GetSlider((int)Sliders.AttackSkillSlider).onValueChanged.AddListener(OnChangeValueAttackSkill);

        GetImage((int)Images.AttackSkillButton).sprite = Managers.Game.Weapon.skillIcon;
        GetImage((int)Images.AttackSkillImage).sprite = Managers.Game.Weapon.skillIcon;
        GetImage((int)Images.AttackImage).sprite = Managers.Game.Weapon.itemIcon;

        foreach(Transform child in GetObject((int)Gameobjects.HpBar).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<Managers.Game.Hp; i++)
            Managers.Resource.Instantiate("UI/Icon/HpIcon", GetObject((int)Gameobjects.HpBar).transform);

        GetImage((int)Images.ShieldCooldownImage).gameObject.SetActive(false);
        GetSlider((int)Sliders.JumpSkillSlider).gameObject.SetActive(false);
        GetSlider((int)Sliders.AttackSkillSlider).gameObject.SetActive(false);
    }

    public void RefreshUI()
    {
        // 가까운 블럭이 있다면 체력 호출
        if (Managers.Game.currentBlock != null)
        {
            GetSlider((int)Sliders.BlockHpBar).gameObject.SetActive(true);
            SetRatio(GetSlider((int)Sliders.BlockHpBar), (float)Managers.Game.currentBlock.hp / Managers.Game.currentBlock.maxHp);
            GetText((int)Texts.BlockHpText).text = Managers.Game.currentBlock.hp + " / " + Managers.Game.currentBlock.maxHp;
        }
        else
            GetSlider((int)Sliders.BlockHpBar).gameObject.SetActive(false);

        GetText((int)Texts.ScoreText).text = Utils.GetCommaText(Managers.Game.Score);
    }

    // 슬라이더 값 NaN 방지
    public void SetRatio(Slider slider, float ratio)
    {
        if (float.IsNaN(ratio) == true)
            slider.value = 0;
        else
            slider.value = ratio;
    }

    // 투명도 설정
    void SetColor(Image image, float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    void Clear()
    {
        Managers.Resource.Destroy(spawning);
        Managers.Resource.Destroy(Managers.Game.player.gameObject);
        Camera.main.transform.position = new Vector3(0, 0, -10f);
        Managers.UI.ClosePopupUI(this);
    }
}
