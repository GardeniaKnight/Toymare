// // using UnityEngine;
// // using UnityEngine.UI;
// // using System.Collections;

// // public class PlayerHealth : MonoBehaviour
// // {
// //     public int startingHealth = 100;
// //     public float invulnerabilityTime = 1f;
// //     public float timeAfterWeLastTookDamage = 1f;
// //     public Slider healthSliderForeground;
// //     public Slider healthSliderBackground;
// //     public Image damageImage;
// //     public AudioClip deathClip;
// //     public float flashSpeed = 5f;
// //     public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
// //     public int currentHealth;
// //     private Animator anim;
// //     private AudioSource playerAudio;
// //     private PlayerMovement playerMovement;
// //     private PlayerShooting playerShooting;
// //     private bool isDead;
// //     private bool damaged;
// //     public float invulnerableTimer;
// //     private float backgroundLerpTimer;
// //     private SkinnedMeshRenderer myRenderer;
// //     private Color rimColor;

// //     void Awake()
// //     {
// //         anim = GetComponent<Animator>();
// //         playerAudio = GetComponent<AudioSource>();
// //         playerMovement = GetComponent<PlayerMovement>();
// //         playerShooting = GetComponentInChildren<PlayerShooting>();

// //         currentHealth = startingHealth;

// //         SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
// //         foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
// //         {
// //             if (meshRenderer.gameObject.name == "Player")
// //             {
// //                 myRenderer = meshRenderer;
// //                 break;
// //             }
// //         }
// //     }

// //     void Start()
// //     {
// //         rimColor = myRenderer.materials[0].GetColor("_RimColor");
// //         if (healthSliderForeground != null)
// //         {
// //             healthSliderForeground.maxValue = startingHealth;
// //             healthSliderForeground.value = startingHealth;
// //         }
// //         if (healthSliderBackground != null)
// //         {
// //             healthSliderBackground.maxValue = startingHealth;
// //             healthSliderBackground.value = startingHealth;
// //         }
// //     }

// //     void Update()
// //     {
// //         if (damaged)
// //         {
// //             damageImage.color = flashColour;
// //         }
// //         else
// //         {
// //             damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
// //         }

// //         invulnerableTimer += Time.deltaTime;
// //         backgroundLerpTimer += Time.deltaTime;

// //         if (backgroundLerpTimer >= timeAfterWeLastTookDamage)
// //         {
// //             healthSliderBackground.value = Mathf.Lerp(healthSliderBackground.value, healthSliderForeground.value, 2f * Time.deltaTime);
// //         }

// //         damaged = false;
// //     }

// //     public void TakeDamage(int amount)
// //     {
// //         Debug.Log("TakeDamage called");
// //         // if (invulnerableTimer < invulnerabilityTime)
// //         // {
// //         //     Debug.Log("[TakeDamage] 无敌中，忽略伤害");
// //         //     return;
// //         // }

// //         Debug.Log($"[TakeDamage] 受到伤害：{amount}");
        
// //         invulnerableTimer = 0f;
// //         backgroundLerpTimer = 0f;
        
// //         StopCoroutine("Ishit");
// //         StartCoroutine("Ishit");
        
// //         damaged = true;

// //         currentHealth -= amount;
// //         currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
// //         if (healthSliderForeground == null)
// //         {
// //             Debug.Log("Slider null!");
// //         }
// //         healthSliderForeground.value = currentHealth;//不扣血的问题在这里
// //         Debug.Log("Here12");
// //         playerAudio.Play();
// //         Debug.Log("Here11");
// //         Debug.Log($"TakeDamage 后当前血量：{currentHealth}");
// //         if (currentHealth <= 0 && !isDead)
// //         {
// //             Death();
// //             Debug.Log("Here10");
// //         }
// //     }

// //     IEnumerator Ishit()
// //     {
// //         Color newColor = new Color(10, 0, 0, 0);
// //         myRenderer.materials[0].SetColor("_RimColor", newColor);

// //         float time = 1f;
// //         float elapsedTime = 0f;
// //         while (elapsedTime < time)
// //         {
// //             if (elapsedTime < (time / 2f))
// //             {
// //                 newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
// //             }
// //             myRenderer.materials[0].SetColor("_RimColor", newColor);
// //             elapsedTime += Time.deltaTime;
// //             yield return null;
// //         }
// //     }

// //     public void AddHealth(int amount)
// //     {
// //         currentHealth += amount;
// //         currentHealth = Mathf.Min(currentHealth, startingHealth);
// //         healthSliderForeground.value = currentHealth;
// //     }

// //     void Death()
// //     {
// //         isDead = true;

// //         if (playerShooting != null)
// //             playerShooting.DisableEffects();

// //         if (anim != null)
// //             anim.SetTrigger("Die");

// //         playerAudio.clip = deathClip;
// //         playerAudio.Play();

// //         if (playerMovement != null)
// //             playerMovement.enabled = false;
// //         if (playerShooting != null)
// //             playerShooting.enabled = false;
// //     }

// //     public bool IsAlive()
// //     {
// //         return currentHealth > 0;
// //     }
// // }

// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections;

// /// <summary>控制玩家血量与血条 UI 的脚本</summary>
// public class PlayerHealth : MonoBehaviour
// {
//     /* ---------- 可在 Inspector 调整的字段 ---------- */
//     [Header("数值设置")]
//     public int   startingHealth              = 100;   // 初始血量
//     public float invulnerabilityTime         = 1f;    // 受伤后的无敌时间
//     public float timeAfterWeLastTookDamage   = 1f;    // 背景条开始缓跟的延迟

//     [Header("UI 引用")]
//     public Slider healthSliderForeground;             // 亮条（即时）
//     public Slider healthSliderBackground;             // 暗条（缓跟）
//     public Image  damageImage;                        // 受伤闪红

//     [Header("音效 / 动画")]
//     public AudioClip deathClip;
//     public float     flashSpeed  = 5f;                // 闪红淡出速度
//     public Color     flashColour = new Color(1f, 0f, 0f, 0.1f);

//     /* ---------- 运行时私有字段 ---------- */
//     [HideInInspector] public int currentHealth;

//     private Animator        anim;
//     private AudioSource     playerAudio;
//     private PlayerMovement  playerMovement;
//     private PlayerShooting  playerShooting;

//     private bool   isDead;
//     private bool   damaged;
//     private float  invulnerableTimer;
//     private float  backgroundLerpTimer;

//     // 受击 Rim 效果
//     private SkinnedMeshRenderer myRenderer;
//     private Color               rimColor;

//     /* ---------- 生命周期 ---------- */
//     void Awake()
//     {
//         /* 组件获取 */
//         anim           = GetComponent<Animator>();
//         playerAudio    = GetComponent<AudioSource>();
//         playerMovement = GetComponent<PlayerMovement>();
//         playerShooting = GetComponentInChildren<PlayerShooting>();

//         /* 让第一次攻击就能生效 */
//         invulnerableTimer = invulnerabilityTime;

//         currentHealth = startingHealth;

//         /* 自动寻找 HUD（如果没有从 GameInitializer 注入） */
//         AutoFindHUDSliders();

//         /* 缓存 Rim 颜色（可删掉此段若你不用边缘高亮） */
//         foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>())
//         {
//             if (r.gameObject.name.Contains("Player"))
//             {
//                 myRenderer = r;
//                 rimColor   = r.materials[0].GetColor("_RimColor");
//                 break;
//             }
//         }
//     }

//     void Start()
//     {
//         /* 初始化血条最大/当前值 */
//         if (healthSliderForeground)
//         {
//             healthSliderForeground.maxValue = startingHealth;
//             healthSliderForeground.value    = startingHealth;
//         }
//         if (healthSliderBackground)
//         {
//             healthSliderBackground.maxValue = startingHealth;
//             healthSliderBackground.value    = startingHealth;
//         }
//     }

//     void Update()
//     {
//         /* 受伤闪红淡出 */
//         damageImage.color = damaged
//             ? flashColour
//             : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
//         damaged = false;

//         /* 计时器累积 */
//         invulnerableTimer  += Time.deltaTime;
//         backgroundLerpTimer += Time.deltaTime;

//         /* 背景条缓跟 */
//         if (healthSliderBackground &&
//             backgroundLerpTimer >= timeAfterWeLastTookDamage)
//         {
//             healthSliderBackground.value = Mathf.Lerp(
//                 healthSliderBackground.value,
//                 healthSliderForeground.value,
//                 2f * Time.deltaTime);
//         }
//     }

//     /* ---------- 受伤 / 加血 / 死亡 ---------- */
//     public void TakeDamage(int amount)
//     {
//         /* 若需要无敌帧，可放开下列两行 */
//         // if (invulnerableTimer < invulnerabilityTime) return;

//         invulnerableTimer   = 0f;
//         backgroundLerpTimer = 0f;
//         damaged             = true;

//         currentHealth = Mathf.Clamp(currentHealth - amount, 0, startingHealth);

//         /* 刷新前景条 */
//         if (healthSliderForeground) healthSliderForeground.value = currentHealth;

//         /* 启动背景条缓跟 */
//         if (healthSliderBackground) StartCoroutine(SmoothBackground());

//         playerAudio.Play();

//         if (currentHealth <= 0 && !isDead) Death();
//     }

//     public void AddHealth(int amount)
//     {
//         currentHealth = Mathf.Min(currentHealth + amount, startingHealth);

//         if (healthSliderForeground)  healthSliderForeground.value = currentHealth;
//         if (healthSliderBackground)  healthSliderBackground.value = currentHealth;
//     }

//     void Death()
//     {
//         isDead = true;

//         if (playerShooting) playerShooting.DisableEffects();
//         if (anim)          anim.SetTrigger("Die");

//         playerAudio.clip = deathClip;
//         playerAudio.Play();

//         if (playerMovement) playerMovement.enabled = false;
//         if (playerShooting) playerShooting.enabled = false;
//     }

//     public bool IsAlive() => currentHealth > 0;

//     /* ---------- 私有辅助 ---------- */
//     void AutoFindHUDSliders()
//     {
//         if (healthSliderForeground && healthSliderBackground) return;

//         PlayerHUD hud = FindObjectOfType<PlayerHUD>();
//         if (hud)
//         {
//             if (!healthSliderForeground) healthSliderForeground = hud.foreground;
//             if (!healthSliderBackground) healthSliderBackground = hud.background;
//         }
//     }

//     IEnumerator SmoothBackground()
//     {
//         float t         = 0f;
//         float duration  = 0.5f;
//         float startVal  = healthSliderBackground.value;

//         while (t < duration)
//         {
//             t += Time.deltaTime;
//             healthSliderBackground.value =
//                 Mathf.Lerp(startVal, currentHealth, t / duration);
//             yield return null;
//         }
//         healthSliderBackground.value = currentHealth;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    /* ---------- Inspector 参数 ---------- */
    [Header("数值设置")]
    public int   startingHealth              = 100;
    public float invulnerabilityTime         = 1f;   // 若保留无敌帧
    public float timeAfterWeLastTookDamage   = 1f;   // 背景条延迟

    [Header("UI 引用 (可不手动拖)")]
    public Slider healthSliderForeground;            // 血条前景
    public Slider healthSliderBackground;            // 血条背景
    public Image  damageImage;                       // 红色闪光

    [Header("音效 / 动画")]
    public AudioClip deathClip;
    public float     flashSpeed  = 5f;
    public Color     flashColour = new Color(1f, 0f, 0f, 0.35f); // α 提高到 0.35

    /* ---------- 运行时字段 ---------- */
    [HideInInspector] public int currentHealth;

    private Animator        anim;
    private AudioSource     playerAudio;
    private PlayerMovement  playerMovement;
    private PlayerShooting  playerShooting;

    private bool   isDead;
    private bool   damaged;
    private float  invulnerableTimer;
    private float  backgroundLerpTimer;

    /* ---------- 生命周期 ---------- */
    void Awake()
    {
        anim           = GetComponent<Animator>();
        playerAudio    = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();

        invulnerableTimer = invulnerabilityTime;     // 第一击可生效
        currentHealth     = startingHealth;

        AutoFindUIRefs();                            // 自动抓 UI
    }

    void Start()
    {
        // 初始化 Slider 最大值与初始值
        if (healthSliderForeground)
        {
            healthSliderForeground.maxValue = startingHealth;
            healthSliderForeground.value    = startingHealth;
        }
        if (healthSliderBackground)
        {
            healthSliderBackground.maxValue = startingHealth;
            healthSliderBackground.value    = startingHealth;
        }

        // 确保闪光起始透明
        if (damageImage) damageImage.color = Color.clear;
    }

    void Update()
    {
        /* 闪光淡出 */
        if (damageImage)
        {
            damageImage.color = damaged
                ? flashColour
                : Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;

        /* 计时与背景条缓跟 */
        invulnerableTimer  += Time.deltaTime;
        backgroundLerpTimer += Time.deltaTime;

        if (healthSliderBackground &&
            backgroundLerpTimer >= timeAfterWeLastTookDamage)
        {
            healthSliderBackground.value = Mathf.Lerp(
                healthSliderBackground.value,
                healthSliderForeground.value,
                2f * Time.deltaTime);
        }
    }

    /* ---------- 受伤 / 加血 / 死亡 ---------- */
    public void TakeDamage(int amount)
    {
        // 若想保留无敌帧，把下一行的注释去掉
        // if (invulnerableTimer < invulnerabilityTime) return;

        invulnerableTimer   = 0f;
        backgroundLerpTimer = 0f;
        damaged             = true;

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
