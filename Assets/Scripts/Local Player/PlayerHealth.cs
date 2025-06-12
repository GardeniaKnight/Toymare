// PlayerHealth.cs
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
        // 先初始化，无论单/多模式都要加载
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
        // 仅本地玩家处理屏幕闪红
        if (photonView.IsMine)
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
    /// 扣血：本地玩家执行，并在死亡时同步给远程客户端
    /// </summary>
    [PunRPC]
    public void TakeDamage(int amount)
    {
        if (!photonView.IsMine) return;

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
            photonView.RPC("RPC_SyncDeath", RpcTarget.OthersBuffered);
        }
    }

    /// <summary>
    /// 恢复血量：供 Pickup 脚本调用
    /// </summary>
    public void AddHealth(int amount)
    {
        // 多人模式下，只在本地玩家实例生效
        if (photonView != null && !photonView.IsMine) return;

        currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

        if (healthSliderForeground != null)
            healthSliderForeground.value = currentHealth;
        if (healthSliderBackground != null)
            healthSliderBackground.value = currentHealth;
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
        if (playerMovement != null)  playerMovement.enabled = false;
        if (playerShooting != null)  playerShooting.enabled = false;
        anim?.SetTrigger("Die");
        playerAudio.clip = deathClip;
        playerAudio.Play();
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
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
        float elapsed = 0f, duration = 0.5f;
        float startVal = healthSliderBackground.value;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthSliderBackground.value = Mathf.Lerp(startVal, currentHealth, elapsed / duration);
            yield return null;
        }
        healthSliderBackground.value = currentHealth;
    }
}

