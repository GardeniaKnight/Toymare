using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerHealth : MonoBehaviourPun
{
    [Header("数值设置")]
    public int   startingHealth      = 100;
    public float invulnerabilityTime = 1f;
    public float timeAfterLastDamage = 1f;

    [Header("UI 引用")]
    public Slider healthSliderForeground;
    public Slider healthSliderBackground;
    public Image  damageImage;

    [Header("音效 / 动画")]
    public AudioClip deathClip;
    public float     flashSpeed      = 5f;
    public Color     flashColor      = new Color(1f, 0f, 0f, 0.35f);

    [HideInInspector] public int currentHealth;

    Animator       anim;
    AudioSource    playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;

    bool   isDead;
    bool   damaged;
    float  invulnerableTimer;
    float  backgroundLerpTimer;

    void Awake()
    {
        // 初始化引用
        anim            = GetComponent<Animator>();
        playerAudio     = GetComponent<AudioSource>();
        playerMovement  = GetComponent<PlayerMovement>();
        playerShooting  = GetComponent<PlayerShooting>();

        invulnerableTimer = invulnerabilityTime;
        currentHealth     = startingHealth;

        AutoFindUIRefs();
    }

    void Start()
    {
        // 初始化血条
        if (healthSliderForeground != null)
        {
            healthSliderForeground.maxValue = startingHealth;
            healthSliderForeground.value    = startingHealth;
        }
        if (healthSliderBackground != null)
        {
            healthSliderBackground.maxValue = startingHealth;
            healthSliderBackground.value    = startingHealth;
        }
        if (damageImage != null)
            damageImage.color = Color.clear;
    }

    void Update()
    {
        // 仅本地玩家实例才做屏幕闪红等效果
        // （在单人模式里，PhotonNetwork.InRoom==false，所以 this.photonView.IsMine 也不管用，
        //  但我们仍然希望单人里走这块逻辑）
        if (!PhotonNetwork.InRoom || photonView.IsMine)
        {
            if (damageImage != null)
                damageImage.color = damaged
                    ? flashColor
                    : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            damaged = false;

            invulnerableTimer   += Time.deltaTime;
            backgroundLerpTimer += Time.deltaTime;

            if (healthSliderBackground != null && backgroundLerpTimer >= timeAfterLastDamage)
            {
                healthSliderBackground.value = Mathf.Lerp(
                    healthSliderBackground.value,
                    healthSliderForeground.value,
                    2f * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// 扣血：单人模式下始终生效；多人模式下仅本地玩家实例生效
    /// </summary>
    [PunRPC]
    public void TakeDamage(int amount)
    {
        // 如果是在多人房间，且又不是本地玩家，就跳过
        if (PhotonNetwork.InRoom && !photonView.IsMine)
            return;

        // 无敌帧逻辑
        if (invulnerableTimer < invulnerabilityTime)
            return;

        invulnerableTimer   = 0f;
        backgroundLerpTimer = 0f;
        damaged             = true;
        currentHealth       = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

        if (healthSliderForeground != null)
            healthSliderForeground.value = currentHealth;
        if (healthSliderBackground != null)
            StartCoroutine(SmoothBackground());

        playerAudio?.Play();

        if (currentHealth <= 0 && !isDead)
        {
            Death();
            // 同步远程死亡效果
            if (PhotonNetwork.InRoom)
                photonView.RPC("RPC_SyncDeath", RpcTarget.OthersBuffered);
        }
    }

    /// <summary>
    /// 恢复血量：单人模式下始终生效；多人模式下仅本地玩家实例生效
    /// </summary>
    public void AddHealth(int amount)
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

        if (healthSliderForeground != null)
            healthSliderForeground.value = currentHealth;
        if (healthSliderBackground != null)
            StartCoroutine(SmoothBackground());
    }

    /// <summary>
    /// 同步远程死亡动画与碰撞禁用
    /// </summary>
    [PunRPC]
    void RPC_SyncDeath()
    {
        if (isDead) return;
        isDead = true;
        anim?.SetTrigger("Die");
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void Death()
    {
        isDead = true;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerShooting != null) playerShooting.enabled = false;

        anim?.SetTrigger("Die");
        playerAudio.clip = deathClip;
        playerAudio.Play();

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 单人模式下，GameOverManager 会在检测到 playerHealth.IsAlive()==false 后触发失败
    }

    /// <summary>
    /// 检查是否仍存活
    /// </summary>
    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }

    void AutoFindUIRefs()
    {
        if ((healthSliderForeground == null || healthSliderBackground == null)
            && FindObjectOfType<PlayerHUD>() is PlayerHUD hud)
        {
            if (healthSliderForeground == null)
                healthSliderForeground = hud.foreground;
            if (healthSliderBackground == null)
                healthSliderBackground = hud.background;
        }
        if (damageImage == null && GameObject.Find("DamageImage") is GameObject imgObj)
            damageImage = imgObj.GetComponent<Image>();
    }

    IEnumerator SmoothBackground()
    {
        float elapsed  = 0f, total = 0.5f;
        float startVal = healthSliderBackground.value;
        while (elapsed < total)
        {
            elapsed += Time.deltaTime;
            healthSliderBackground.value
                = Mathf.Lerp(startVal, currentHealth, elapsed / total);
            yield return null;
        }
        healthSliderBackground.value = currentHealth;
    }
}
