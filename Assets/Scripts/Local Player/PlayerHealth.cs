// // using UnityEngine;
// // using UnityEngine.UI;
// // using Photon.Pun;
// // using System.Collections;

// // public class PlayerHealth : MonoBehaviourPun
// // {
// //     [Header("数值设置")]
// //     public int startingHealth = 100;
// //     public float invulnerabilityTime = 1f;
// //     public float timeAfterWeLastTookDamage = 1f;

// //     [Header("UI 引用")]
// //     public Slider healthSliderForeground;
// //     public Slider healthSliderBackground;
// //     public Image damageImage;

// //     [Header("音效 / 动画")]
// //     public AudioClip deathClip;
// //     public float flashSpeed = 5f;
// //     public Color flashColour = new Color(1f,0f,0f,0.35f);

// //     [HideInInspector] public int currentHealth;
// //     private Animator anim;
// //     private AudioSource playerAudio;
// //     private PlayerMovement playerMovement;
// //     private PlayerShooting playerShooting;

// //     private bool isDead;
// //     private bool damaged;
// //     private float invulnerableTimer;
// //     private float backgroundLerpTimer;

// //     /* ---------- 生命周期 ---------- */
// //     // void Awake()
// //     // {
// //     //     anim           = GetComponent<Animator>();
// //     //     playerAudio    = GetComponent<AudioSource>();
// //     //     playerMovement = GetComponent<PlayerMovement>();
// //     //     playerShooting = GetComponentInChildren<PlayerShooting>();

// //     //     invulnerableTimer = invulnerabilityTime;     // 第一击可生效
// //     //     currentHealth     = startingHealth;

// //     //     AutoFindUIRefs();                            // 自动抓 UI
// //     // }

// //     // void Start()
// //     // {
// //     //     // 初始化 Slider 最大值与初始值
// //     //     if (healthSliderForeground)
// //     //     {
// //     //         healthSliderForeground.maxValue = startingHealth;
// //     //         healthSliderForeground.value    = startingHealth;
// //     //     }
// //     //     if (healthSliderBackground)
// //     //     {
// //     //         healthSliderBackground.maxValue = startingHealth;
// //     //         healthSliderBackground.value    = startingHealth;
// //     //     }

// //     //     // 确保闪光起始透明
// //     //     if (damageImage) damageImage.color = Color.clear;
// //     // }

// //     // void Update()
// //     // {
// //     //     /* 闪光淡出 */
// //     //     if (damageImage)
// //     //     {
// //     //         damageImage.color = damaged
// //     //             ? flashColour
// //     //             : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
// //     //     }
// //     //     damaged = false;

// //     //     /* 计时与背景条缓跟 */
// //     //     invulnerableTimer  += Time.deltaTime;
// //     //     backgroundLerpTimer += Time.deltaTime;

// //     //     if (healthSliderBackground &&
// //     //         backgroundLerpTimer >= timeAfterWeLastTookDamage)
// //     //     {
// //     //         healthSliderBackground.value = Mathf.Lerp(
// //     //             healthSliderBackground.value,
// //     //             healthSliderForeground.value,
// //     //             2f * Time.deltaTime);
// //     //     }
// //     // }

// //     // /* ---------- 受伤 / 加血 / 死亡 ---------- */
// //     // public void TakeDamage(int amount)
// //     // {
// //     //     // 若想保留无敌帧，把下一行的注释去掉
// //     //     // if (invulnerableTimer < invulnerabilityTime) return;

// //     //     invulnerableTimer   = 0f;
// //     //     backgroundLerpTimer = 0f;
// //     //     damaged             = true;

// //     //     currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

// //     //     if (healthSliderForeground) healthSliderForeground.value = currentHealth;
// //     //     if (healthSliderBackground) StartCoroutine(SmoothBackground());

// //     //     playerAudio.Play();

// //     //     if (currentHealth <= 0 && !isDead) Death();
// //     // }

// //     void Awake()
// //     {
// //         // 仅本地实例执行
// //         if (photonView != null && !photonView.IsMine)
// //         {
// //             enabled = false;
// //             return;
// //         }

// //         anim = GetComponent<Animator>();
// //         playerAudio = GetComponent<AudioSource>();
// //         playerMovement = GetComponent<PlayerMovement>();
// //         playerShooting = GetComponent<PlayerShooting>();

// //         invulnerableTimer = invulnerabilityTime;
// //         currentHealth = startingHealth;

// //         // 找到 HUD
// //         AutoFindUIRefs();
// //     }

// //     void Start()
// //     {
// //         if (healthSliderForeground)
// //         {
// //             healthSliderForeground.maxValue = startingHealth;
// //             healthSliderForeground.value = startingHealth;
// //         }
// //         if (healthSliderBackground)
// //         {
// //             healthSliderBackground.maxValue = startingHealth;
// //             healthSliderBackground.value = startingHealth;
// //         }
// //         if (damageImage) damageImage.color = Color.clear;
// //     }

// //     void Update()
// //     {
// //         // 闪红
// //         if (damageImage)
// //             damageImage.color = damaged ? flashColour : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
// //         damaged = false;

// //         invulnerableTimer += Time.deltaTime;
// //         backgroundLerpTimer += Time.deltaTime;

// //         if (healthSliderBackground && backgroundLerpTimer >= timeAfterWeLastTookDamage)
// //             healthSliderBackground.value = Mathf.Lerp(
// //                 healthSliderBackground.value,
// //                 healthSliderForeground.value,
// //                 2f * Time.deltaTime
// //             );
// //     }

// //     public void TakeDamage(int amount)
// //     {
// //         if (!photonView.IsMine) return;

// //         invulnerableTimer = 0f;
// //         backgroundLerpTimer = 0f;
// //         damaged = true;

// //         currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

// //         if (healthSliderForeground) healthSliderForeground.value = currentHealth;
// //         if (healthSliderBackground) StartCoroutine(SmoothBackground());

// //         playerAudio.Play();

// //         if (currentHealth <= 0 && !isDead) Death();
// //     }

// //     public void AddHealth(int amount)
// //     {
// //         currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

// //         if (healthSliderForeground) healthSliderForeground.value = currentHealth;
// //         if (healthSliderBackground) healthSliderBackground.value = currentHealth;
// //     }

// //     void Death()
// //     {
// //         isDead = true;
// //         if (playerShooting) playerShooting.DisableEffects();
// //         if (anim)          anim.SetTrigger("Die");

// //         playerAudio.clip = deathClip;
// //         playerAudio.Play();

// //         if (playerMovement) playerMovement.enabled = false;
// //         if (playerShooting) playerShooting.enabled = false;
// //     }

// //     public bool IsAlive() => currentHealth > 0;

// //     /* ---------- 辅助 ---------- */
// //     void AutoFindUIRefs()
// //     {
// //         // Slider
// //         if (!healthSliderForeground || !healthSliderBackground)
// //         {
// //             var hud = FindObjectOfType<PlayerHUD>();
// //             if (hud)
// //             {
// //                 if (!healthSliderForeground) healthSliderForeground = hud.foreground;
// //                 if (!healthSliderBackground) healthSliderBackground = hud.background;
// //             }
// //         }
// //         // 红闪 Image
// //         if (!damageImage)
// //         {
// //             var imgObj = GameObject.Find("DamageImage");
// //             if (imgObj) damageImage = imgObj.GetComponent<Image>();
// //         }
// //     }

// //     IEnumerator SmoothBackground()
// //     {
// //         float t        = 0f;
// //         const float dur = 0.5f;
// //         float startVal = healthSliderBackground.value;

// //         while (t < dur)
// //         {
// //             t += Time.deltaTime;
// //             healthSliderBackground.value = Mathf.Lerp(startVal, currentHealth, t / dur);
// //             yield return null;
// //         }
// //         healthSliderBackground.value = currentHealth;
// //     }
// // }

// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;
// using System.Collections;

// public class PlayerHealth : MonoBehaviourPun
// {
//     [Header("数值设置")]
//     public int   startingHealth           = 100;
//     public float invulnerabilityTime      = 1f;
//     public float timeAfterLastDamage      = 1f;

//     [Header("UI 引用")]
//     public Slider healthSliderForeground;
//     public Slider healthSliderBackground;
//     public Image  damageImage;

//     [Header("音效 / 动画")]
//     public AudioClip deathClip;
//     public float     flashSpeed           = 5f;
//     public Color     flashColor           = new Color(1f, 0f, 0f, 0.35f);

//     [HideInInspector] public int currentHealth;

//     private Animator     anim;
//     private AudioSource  playerAudio;
//     private PlayerMovement playerMovement;
//     private PlayerShooting playerShooting;

//     private bool   isDead;
//     private bool   damaged;
//     private float  invulnerableTimer;
//     private float  backgroundLerpTimer;

//     void Awake()
//     {
//         // 只要挂了 PhotonView 且又不是本地，就立刻禁用组件
//         if (photonView != null && !photonView.IsMine)
//         {
//             enabled = false;
//             return;
//         }

//         // 获取组件引用
//         anim            = GetComponent<Animator>();
//         playerAudio     = GetComponent<AudioSource>();
//         playerMovement  = GetComponent<PlayerMovement>();
//         playerShooting  = GetComponent<PlayerShooting>();

//         // 初始化数值
//         invulnerableTimer = invulnerabilityTime;
//         currentHealth     = startingHealth;

//         // 自动寻找 HUD 引用（如果 Inspector 中未赋值）
//         AutoFindUIRefs();
//     }

//     void Start()
//     {
//         // 初始化血条 Slider
//         if (healthSliderForeground != null)
//         {
//             healthSliderForeground.maxValue = startingHealth;
//             healthSliderForeground.value    = startingHealth;
//         }

//         if (healthSliderBackground != null)
//         {
//             healthSliderBackground.maxValue = startingHealth;
//             healthSliderBackground.value    = startingHealth;
//         }

//         // 确保受伤闪光初始透明
//         if (damageImage != null)
//             damageImage.color = Color.clear;
//     }

//     void Update()
//     {
//         if (!enabled) return;

//         // 处理受伤闪红效果
//         if (damageImage != null)
//         {
//             damageImage.color = damaged
//                 ? flashColor
//                 : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
//         }
//         damaged = false;

//         // 计时平滑背景血条
//         invulnerableTimer   += Time.deltaTime;
//         backgroundLerpTimer += Time.deltaTime;

//         if (healthSliderBackground != null && backgroundLerpTimer >= timeAfterLastDamage)
//         {
//             healthSliderBackground.value = Mathf.Lerp(
//                 healthSliderBackground.value,
//                 healthSliderForeground.value,
//                 2f * Time.deltaTime
//             );
//         }
//     }

//     /// <summary>
//     /// 受到伤害时调用，amount 为扣血值
//     /// </summary>

//     [PunRPC]
//     public void TakeDamage(int amount)
//     {
//         // 仅本地玩家实例生效
//         if (PhotonNetwork.IsConnected && photonView != null && !photonView.IsMine)
//         {
//             enabled = false;
//             return;
//         }


//         // 可加入无敌帧判定：if (invulnerableTimer < invulnerabilityTime) return;
//         invulnerableTimer = 0f;
//         backgroundLerpTimer = 0f;
//         damaged = true;

//         currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

//         if (healthSliderForeground != null)
//             healthSliderForeground.value = currentHealth;

//         if (healthSliderBackground != null)
//             StartCoroutine(SmoothBackground());

//         // 播放受击音效
//         playerAudio?.Play();

//         if (currentHealth <= 0 && !isDead)
//             Death();
//     }

//     /// <summary>
//     /// 恢复血量
//     /// </summary>
//     public void AddHealth(int amount)
//     {
//         currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

//         if (healthSliderForeground != null)
//             healthSliderForeground.value = currentHealth;
//         if (healthSliderBackground != null)
//             healthSliderBackground.value = currentHealth;
//     }

//     /// <summary>
//     /// 角色死亡处理
//     /// </summary>
//     void Death()
//     {
//         isDead = true;

//         // 禁用移动和射击
//         if (playerMovement != null)
//             playerMovement.enabled = false;
//         if (playerShooting != null)
//             playerShooting.enabled = false;

//         // 播放死亡动画和音效
//         anim?.SetTrigger("Die");
//         playerAudio.clip = deathClip;
//         playerAudio.Play();
//     }

//     public bool IsAlive()
//     {
//         return currentHealth > 0;
//     }

//     /// <summary>
//     /// 自动查找 UI 组件（当 Inspector 中未拖入时使用）
//     /// </summary>
//     void AutoFindUIRefs()
//     {
//         if (healthSliderForeground == null || healthSliderBackground == null)
//         {
//             var hud = FindObjectOfType<PlayerHUD>();
//             if (hud != null)
//             {
//                 if (healthSliderForeground == null)
//                     healthSliderForeground = hud.foreground;
//                 if (healthSliderBackground == null)
//                     healthSliderBackground = hud.background;
//             }
//         }

//         if (damageImage == null)
//         {
//             var imgObj = GameObject.Find("DamageImage");
//             if (imgObj != null)
//                 damageImage = imgObj.GetComponent<Image>();
//         }
//     }

//     IEnumerator SmoothBackground()
//     {
//         float elapsed = 0f;
//         float duration = 0.5f;
//         float startVal = healthSliderBackground.value;

//         while (elapsed < duration)
//         {
//             elapsed += Time.deltaTime;
//             healthSliderBackground.value = Mathf.Lerp(startVal, currentHealth, elapsed / duration);
//             yield return null;
//         }

//         healthSliderBackground.value = currentHealth;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerHealth : MonoBehaviourPun
{
    [Header("数值设置")]
    public int   startingHealth           = 100;
    public float invulnerabilityTime      = 1f;
    public float timeAfterLastDamage      = 1f;

    [Header("UI 引用")]
    public Slider healthSliderForeground;
    public Slider healthSliderBackground;
    public Image  damageImage;

    [Header("音效 / 动画")]
    public AudioClip deathClip;
    public float     flashSpeed           = 5f;
    public Color     flashColor           = new Color(1f, 0f, 0f, 0.35f);

    [HideInInspector] public int currentHealth;

    private Animator     anim;
    private AudioSource  playerAudio;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    private bool   isDead;
    private bool   damaged;
    private float  invulnerableTimer;
    private float  backgroundLerpTimer;

    void Awake()
    {
        // 仅本地玩家实例执行后续逻辑
        if (photonView != null && !photonView.IsMine)
        {
            enabled = false;
            return;
        }

        // 获取组件引用
        anim           = GetComponent<Animator>();
        playerAudio    = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();

        // 初始化数值
        invulnerableTimer = invulnerabilityTime;
        currentHealth     = startingHealth;

        // 自动寻找 HUD 引用（如果 Inspector 中未赋值）
        AutoFindUIRefs();
    }

    void Start()
    {
        // 初始化血条 Slider
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

        // 确保受伤闪光初始透明
        if (damageImage != null)
            damageImage.color = Color.clear;
    }

    void Update()
    {
        if (!enabled) return;

        // 处理受伤闪红效果
        if (damageImage != null)
        {
            damageImage.color = damaged
                ? flashColor
                : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;

        // 计时平滑背景血条
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

    [PunRPC]
    public void TakeDamage(int amount)
    {
        // 仅本地玩家实例生效
        if (photonView != null && !photonView.IsMine)
            return;

        // 可加入无敌帧判定：if (invulnerableTimer < invulnerabilityTime) return;
        invulnerableTimer   = 0f;
        backgroundLerpTimer = 0f;
        damaged             = true;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

        if (healthSliderForeground != null)
            healthSliderForeground.value = currentHealth;

        if (healthSliderBackground != null)
            StartCoroutine(SmoothBackground());

        // 播放受击音效
        playerAudio?.Play();

        if (currentHealth <= 0 && !isDead)
            Death();
    }

    public void AddHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

        if (healthSliderForeground != null)
            healthSliderForeground.value = currentHealth;
        if (healthSliderBackground != null)
            healthSliderBackground.value = currentHealth;
    }

    /// <summary>
    /// 判断是否还活着
    /// </summary>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    void Death()
    {
        isDead = true;

        // 禁用移动和射击
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerShooting != null)
            playerShooting.enabled = false;

        // 播放死亡动画和音效
        anim?.SetTrigger("Die");
        playerAudio.clip = deathClip;
        playerAudio.Play();
    }

    void AutoFindUIRefs()
    {
        if ((healthSliderForeground == null || healthSliderBackground == null) && FindObjectOfType<PlayerHUD>() is PlayerHUD hud)
        {
            if (healthSliderForeground == null)
                healthSliderForeground = hud.foreground;
            if (healthSliderBackground == null)
                healthSliderBackground = hud.background;
        }

        if (damageImage == null && GameObject.Find("DamageImage") is GameObject imgObj)
        {
            damageImage = imgObj.GetComponent<Image>();
        }
    }

    IEnumerator SmoothBackground()
    {
        float elapsed = 0f;
        float duration = 0.5f;
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
