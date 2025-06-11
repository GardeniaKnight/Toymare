using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerHealth : MonoBehaviourPun
{
    [Header("数值设置")]
    public int startingHealth = 100;
    public float invulnerabilityTime = 1f;
    public float timeAfterWeLastTookDamage = 1f;

    [Header("UI 引用")]
    public Slider healthSliderForeground;
    public Slider healthSliderBackground;
    public Image damageImage;

    [Header("音效 / 动画")]
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f,0f,0f,0.35f);

    [HideInInspector] public int currentHealth;
    private Animator anim;
    private AudioSource playerAudio;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    private bool isDead;
    private bool damaged;
    private float invulnerableTimer;
    private float backgroundLerpTimer;

    /* ---------- 生命周期 ---------- */
    // void Awake()
    // {
    //     anim           = GetComponent<Animator>();
    //     playerAudio    = GetComponent<AudioSource>();
    //     playerMovement = GetComponent<PlayerMovement>();
    //     playerShooting = GetComponentInChildren<PlayerShooting>();

    //     invulnerableTimer = invulnerabilityTime;     // 第一击可生效
    //     currentHealth     = startingHealth;

    //     AutoFindUIRefs();                            // 自动抓 UI
    // }

    // void Start()
    // {
    //     // 初始化 Slider 最大值与初始值
    //     if (healthSliderForeground)
    //     {
    //         healthSliderForeground.maxValue = startingHealth;
    //         healthSliderForeground.value    = startingHealth;
    //     }
    //     if (healthSliderBackground)
    //     {
    //         healthSliderBackground.maxValue = startingHealth;
    //         healthSliderBackground.value    = startingHealth;
    //     }

    //     // 确保闪光起始透明
    //     if (damageImage) damageImage.color = Color.clear;
    // }

    // void Update()
    // {
    //     /* 闪光淡出 */
    //     if (damageImage)
    //     {
    //         damageImage.color = damaged
    //             ? flashColour
    //             : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
    //     }
    //     damaged = false;

    //     /* 计时与背景条缓跟 */
    //     invulnerableTimer  += Time.deltaTime;
    //     backgroundLerpTimer += Time.deltaTime;

    //     if (healthSliderBackground &&
    //         backgroundLerpTimer >= timeAfterWeLastTookDamage)
    //     {
    //         healthSliderBackground.value = Mathf.Lerp(
    //             healthSliderBackground.value,
    //             healthSliderForeground.value,
    //             2f * Time.deltaTime);
    //     }
    // }

    // /* ---------- 受伤 / 加血 / 死亡 ---------- */
    // public void TakeDamage(int amount)
    // {
    //     // 若想保留无敌帧，把下一行的注释去掉
    //     // if (invulnerableTimer < invulnerabilityTime) return;

    //     invulnerableTimer   = 0f;
    //     backgroundLerpTimer = 0f;
    //     damaged             = true;

    //     currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

    //     if (healthSliderForeground) healthSliderForeground.value = currentHealth;
    //     if (healthSliderBackground) StartCoroutine(SmoothBackground());

    //     playerAudio.Play();

    //     if (currentHealth <= 0 && !isDead) Death();
    // }

    void Awake()
    {
        // 仅本地实例执行
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();

        invulnerableTimer = invulnerabilityTime;
        currentHealth = startingHealth;

        // 找到 HUD
        AutoFindUIRefs();
    }

    void Start()
    {
        if (healthSliderForeground)
        {
            healthSliderForeground.maxValue = startingHealth;
            healthSliderForeground.value = startingHealth;
        }
        if (healthSliderBackground)
        {
            healthSliderBackground.maxValue = startingHealth;
            healthSliderBackground.value = startingHealth;
        }
        if (damageImage) damageImage.color = Color.clear;
    }

    void Update()
    {
        // 闪红
        if (damageImage)
            damageImage.color = damaged ? flashColour : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        damaged = false;

        invulnerableTimer += Time.deltaTime;
        backgroundLerpTimer += Time.deltaTime;

        if (healthSliderBackground && backgroundLerpTimer >= timeAfterWeLastTookDamage)
            healthSliderBackground.value = Mathf.Lerp(
                healthSliderBackground.value,
                healthSliderForeground.value,
                2f * Time.deltaTime
            );
    }

    public void TakeDamage(int amount)
    {
        if (!photonView.IsMine) return;

        invulnerableTimer = 0f;
        backgroundLerpTimer = 0f;
        damaged = true;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

        if (healthSliderForeground) healthSliderForeground.value = currentHealth;
        if (healthSliderBackground) StartCoroutine(SmoothBackground());

        playerAudio.Play();

        if (currentHealth <= 0 && !isDead) Death();
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

        if (healthSliderForeground) healthSliderForeground.value = currentHealth;
        if (healthSliderBackground) healthSliderBackground.value = currentHealth;
    }

    void Death()
    {
        isDead = true;
        if (playerShooting) playerShooting.DisableEffects();
        if (anim)          anim.SetTrigger("Die");

        playerAudio.clip = deathClip;
        playerAudio.Play();

        if (playerMovement) playerMovement.enabled = false;
        if (playerShooting) playerShooting.enabled = false;
    }

    public bool IsAlive() => currentHealth > 0;

    /* ---------- 辅助 ---------- */
    void AutoFindUIRefs()
    {
        // Slider
        if (!healthSliderForeground || !healthSliderBackground)
        {
            var hud = FindObjectOfType<PlayerHUD>();
            if (hud)
            {
                if (!healthSliderForeground) healthSliderForeground = hud.foreground;
                if (!healthSliderBackground) healthSliderBackground = hud.background;
            }
        }
        // 红闪 Image
        if (!damageImage)
        {
            var imgObj = GameObject.Find("DamageImage");
            if (imgObj) damageImage = imgObj.GetComponent<Image>();
        }
    }

    IEnumerator SmoothBackground()
    {
        float t        = 0f;
        const float dur = 0.5f;
        float startVal = healthSliderBackground.value;

        while (t < dur)
        {
            t += Time.deltaTime;
            healthSliderBackground.value = Mathf.Lerp(startVal, currentHealth, t / dur);
            yield return null;
        }
        healthSliderBackground.value = currentHealth;
    }
}
