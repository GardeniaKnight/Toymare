// // using UnityEngine;
// // using UnityEngine.UI;
// // using Unity.Netcode;

// // public class PlayerShooting : NetworkBehaviour
// // {
// //     public Color[] bulletColors;
// //     public float bounceDuration = 10;
// //     public float pierceDuration = 10;

// //     public int damagePerShot = 20;
// //     public int numberOfBullets = 1;
// //     public float timeBetweenBullets = 0.15f;
// //     public float angleBetweenBullets = 10f;

// //     public float range = 100f;
// //     public LayerMask shootableMask;
// //     public Image bounceImage;
// //     public Image pierceImage;

// //     public GameObject bullet;
// //     public Transform bulletSpawnAnchor;

// //     float timer;
// //     float bounceTimer;
// //     float pierceTimer;
// //     bool bounce;
// //     bool piercing;
// //     Color bulletColor;

// //     AudioSource gunAudio;
// //     Light gunLight;
// //     ParticleSystem gunParticles;
// //     float effectsDisplayTime = 0.2f;

// //     public float BounceTimer { get => bounceTimer; set => bounceTimer = value; }
// //     public float PierceTimer { get => pierceTimer; set => pierceTimer = value; }

// //     void Awake()
// //     {
// //         gunParticles = GetComponentInChildren<ParticleSystem>();
// //         gunAudio = GetComponent<AudioSource>();
// //         gunLight = GetComponentInChildren<Light>();

// //         bounceTimer = bounceDuration;
// //         pierceTimer = pierceDuration;
// //     }

// //     void Update()
// //     {
// //         // Debug.Log("🌀 [Update] PlayerShooting is running...");
// //         // // ✅ 仅当 Netcode 存在时再判断是否为本地玩家（多人模式下）
// //         // if (Unity.Netcode.NetworkManager.Singleton != null && !IsOwner)
// //         //     return;
// //         ////这里以后要改，多人模式不能这样

// //         // ✅ 状态逻辑
// //         bounce = bounceTimer < bounceDuration;
// //         piercing = pierceTimer < pierceDuration;

// //         // ✅ 设置子弹颜色（状态优先级：双属性 > 穿透 > 反弹 > 默认）
// //         bulletColor = bulletColors[0];
// //         if (bounce && piercing)
// //         {
// //             bulletColor = bulletColors[3];
// //         }
// //         else if (piercing)
// //         {
// //             bulletColor = bulletColors[2];
// //         }
// //         else if (bounce)
// //         {
// //             bulletColor = bulletColors[1];
// //         }

// //         // ✅ 更新 UI 显示与颜色（空检查）
// //         if (bounceImage != null)
// //         {
// //             bounceImage.gameObject.SetActive(bounce);
// //             bounceImage.color = bulletColor;
// //         }

// //         if (pierceImage != null)
// //         {
// //             pierceImage.gameObject.SetActive(piercing);
// //             pierceImage.color = bulletColor;
// //         }

// //         // ✅ 粒子与光照颜色
// //         if (gunParticles != null)
// //             gunParticles.startColor = bulletColor;

// //         if (gunLight != null)
// //             gunLight.color = bulletColor;

// //         // ✅ 时间推进
// //         bounceTimer += Time.deltaTime;
// //         pierceTimer += Time.deltaTime;
// //         timer += Time.deltaTime;

// //         Debug.Log("🌀 [Update] PlayerShooting is running...");
    
// //         Debug.Log("⛳ Fire1 status: " + Input.GetButton("Fire1"));
// //         Debug.Log("⏱ Timer: " + timer + " / " + timeBetweenBullets);

// //         if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
// //         {
// //             Debug.Log("✅ Fire1 triggered, calling Shoot()");
// //             Shoot();
// //         }

// //         // ✅ 判断是否需要关闭枪口光效
// //         if (timer >= timeBetweenBullets * effectsDisplayTime)
// //         {
// //             DisableEffects();
// //         }
// //     }


// //     public void DisableEffects()
// //     {
// //         if (gunLight != null)
// //             gunLight.enabled = false;
// //     }

// //     void Shoot()
// //     {
// //         timer = 0f;
// //         Debug.Log("[Shoot] Trying to shoot...");
// //         if (bullet == null)
// //         {
// //             Debug.LogError("bullet prefab 未设置！");
// //             return;
// //         }
// //         if (bulletSpawnAnchor == null)
// //         {
// //             Debug.LogError("bulletSpawnAnchor 未设置！");
// //             return;
// //         }

// //         if (gunAudio != null)
// //         {
// //             gunAudio.pitch = piercing && bounce ? 0.95f :
// //                              piercing ? 1.05f :
// //                              bounce ? 1.15f : 1.25f;
// //             gunAudio.Play();
// //         }

// //         if (gunLight != null)
// //         {
// //             gunLight.intensity = 1 + 0.5f * (numberOfBullets - 1);
// //             gunLight.enabled = true;
// //         }

// //         if (gunParticles != null)
// //         {
// //             gunParticles.Stop();
// //             gunParticles.startSize = 1 + 0.1f * (numberOfBullets - 1);
// //             gunParticles.Play();
// //         }

// //         for (int i = 0; i < numberOfBullets; i++)
// //         {
// //             float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2f) * (numberOfBullets - 1));
// //             Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * bulletSpawnAnchor.rotation;

// //             GameObject b = Instantiate(bullet, bulletSpawnAnchor.position, rot);
// //             Debug.Log("✅ Bullet instantiated at: " + b.transform.position);
// //             Bullet bulletScript = b.GetComponent<Bullet>();
// //             if (bulletScript != null)
// //             {
// //                 bulletScript.piercing = piercing;
// //                 bulletScript.bounce = bounce;
// //                 bulletScript.bulletColor = bulletColor;
// //             }
// //         }
// //     }
// // }


// using UnityEngine;
// using UnityEngine.UI;
// using Photon.Pun;

// public class PlayerShooting : MonoBehaviourPun
// {
//     [Header("子弹与特效")]
//     public GameObject bulletPrefab;      // 放在 Resources 文件夹下
//     public Transform  bulletSpawnAnchor;
//     public float      timeBetweenBullets = 0.15f;
//     public int        numberOfBullets    = 1;
//     public float      angleBetweenBullets = 10f;

//     [Header("UI 指示")]
//     public Image  bounceImage;
//     public Image  pierceImage;
//     public Color[] bulletColors;
//     public float   bounceDuration = 10f;
//     public float   pierceDuration = 10f;

//     [Header("音光粒子")]
//     public AudioSource    gunAudio;
//     public Light          gunLight;
//     public ParticleSystem gunParticles;
//     public float          effectsDisplayTime = 0.2f;

//     // 私有计时器
//     private float timer;
//     private float bounceTimer;
//     private float pierceTimer;

//     // 对外公开计时器，供 Pickup 等脚本使用，可读写
//     public float BounceTimer
//     {
//         get => bounceTimer;
//         set => bounceTimer = value;
//     }
//     public float PierceTimer
//     {
//         get => pierceTimer;
//         set => pierceTimer = value;
//     }

//     void Awake()
//     {
//         // 初始化特效组件引用
//         gunParticles = GetComponentInChildren<ParticleSystem>();
//         gunAudio     = GetComponent<AudioSource>();
//         gunLight     = GetComponentInChildren<Light>();

//         bounceTimer = bounceDuration;
//         pierceTimer = pierceDuration;

//         // 多人模式：如果不是本地玩家，就禁用本脚本
//         if (GameModeManager.CurrentMode == GameMode.MultiPlayer)
//         {
//             if (photonView != null && !photonView.IsMine)
//             {
//                 enabled = false;
//                 return;
//             }
            
//         }
//     }

//     void Update()
//     {
//         // 单人模式：脚本始终启用
//         // 多人模式：仅本地玩家实例启用
//         if (GameModeManager.CurrentMode == GameMode.MultiPlayer && photonView != null && !photonView.IsMine)
//             return;

//         // 时间累加
//         timer       += Time.deltaTime;
//         bounceTimer += Time.deltaTime;
//         pierceTimer += Time.deltaTime;

//         // 计算状态和子弹颜色
//         bool bounce  = bounceTimer < bounceDuration;
//         bool pierce  = pierceTimer < pierceDuration;
//         Color col    = bulletColors.Length > 0 ? bulletColors[0] : Color.white;
//         if (bounce && pierce) col = bulletColors.Length > 3 ? bulletColors[3] : col;
//         else if (pierce)      col = bulletColors.Length > 2 ? bulletColors[2] : col;
//         else if (bounce)      col = bulletColors.Length > 1 ? bulletColors[1] : col;

//         // 更新 UI 显示
//         if (bounceImage)
//         {
//             bounceImage.gameObject.SetActive(bounce);
//             bounceImage.color = col;
//         }
//         if (pierceImage)
//         {
//             pierceImage.gameObject.SetActive(pierce);
//             pierceImage.color = col;
//         }

//         // 粒子与光照颜色
//         if (gunParticles) gunParticles.startColor = col;
//         if (gunLight)     gunLight.color = col;

//         // 射击输入
//         if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
//         {
//             Shoot(bounce, pierce, col);
//         }

//         // 关闭光效
//         if (gunLight && timer >= timeBetweenBullets * effectsDisplayTime)
//         {
//             gunLight.enabled = false;
//         }
//     }

//     void Shoot(bool bounce, bool pierce, Color bulletColor)
//     {
//         timer = 0f;

//         if (bulletPrefab == null || bulletSpawnAnchor == null)
//         {
//             Debug.LogError("PlayerShooting: bulletPrefab 或 spawnAnchor 未设置");
//             return;
//         }

//         // 音效
//         gunAudio?.Play();

//         // 光效
//         if (gunLight)
//         {
//             gunLight.intensity = 1 + 0.5f * (numberOfBullets - 1);
//             gunLight.enabled = true;
//         }

//         // 粒子效果
//         if (gunParticles)
//         {
//             gunParticles.Stop();
//             gunParticles.startSize = 1 + 0.1f * (numberOfBullets - 1);
//             gunParticles.Play();
//         }

//         for (int i = 0; i < numberOfBullets; i++)
//         {
//             float angle = i * angleBetweenBullets
//                         - (angleBetweenBullets * (numberOfBullets - 1) / 2f);
//             Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up)
//                              * bulletSpawnAnchor.rotation;

//             if (GameModeManager.CurrentMode == GameMode.MultiPlayer)
//             {
//                 // Photon 实例化 —— 需放在 Resources 下
//                 GameObject b = PhotonNetwork.Instantiate(
//                     bulletPrefab.name,
//                     bulletSpawnAnchor.position,
//                     rot
//                 );
//                 var bs = b.GetComponent<Bullet>();
//                 if (bs != null)
//                 {
//                     bs.bounce      = bounce;
//                     bs.piercing    = pierce;
//                     bs.bulletColor = bulletColor;
//                 }
//             }
//             else
//             {
//                 // 单人模式下本地实例化
//                 GameObject b = Instantiate(
//                     bulletPrefab,
//                     bulletSpawnAnchor.position,
//                     rot
//                 );
//                 var bs = b.GetComponent<Bullet>();
//                 if (bs != null)
//                 {
//                     bs.bounce      = bounce;
//                     bs.piercing    = pierce;
//                     bs.bulletColor = bulletColor;
//                 }
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPun
{
    [Header("子弹与特效")]
    public GameObject bulletPrefab;        // 放在 Resources 文件夹下
    public Transform  bulletSpawnAnchor;
    public float      timeBetweenBullets = 0.15f;
    public int        numberOfBullets    = 1;
    public float      angleBetweenBullets = 10f;

    [Header("UI 指示")]
    public Image   bounceImage;
    public Image   pierceImage;
    public Color[] bulletColors;
    public float   bounceDuration = 10f;
    public float   pierceDuration = 10f;

    [Header("音光粒子")]
    public AudioSource    gunAudio;
    public Light          gunLight;
    public ParticleSystem gunParticles;
    public float          effectsDisplayTime = 0.2f;

    [Header("多人模式专用")]
    [Tooltip("多人模式下，每颗子弹造成的伤害")]
    public int multiplayerBulletDamage = 6;

    // 私有计时器
    private float timer;
    private float bounceTimer;
    private float pierceTimer;

    // 公开给 Pickup 等脚本用
    public float BounceTimer { get => bounceTimer; set => bounceTimer = value; }
    public float PierceTimer { get => pierceTimer; set => pierceTimer = value; }

    void Awake()
    {
        // 初始化特效组件引用
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunAudio     = GetComponent<AudioSource>();
        gunLight     = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;

        // 只让本地玩家启用本脚本
        if (photonView != null && !photonView.IsMine)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!enabled) return;

        // 时间累加
        timer       += Time.deltaTime;
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;

        // 计算状态与子弹颜色
        bool bounce  = bounceTimer < bounceDuration;
        bool pierce  = pierceTimer < pierceDuration;
        Color col    = bulletColors.Length > 0 ? bulletColors[0] : Color.white;
        if (bounce && pierce)       col = bulletColors.Length > 3 ? bulletColors[3] : col;
        else if (pierce)            col = bulletColors.Length > 2 ? bulletColors[2] : col;
        else if (bounce)            col = bulletColors.Length > 1 ? bulletColors[1] : col;

        // 更新 UI
        if (bounceImage)
        {
            bounceImage.gameObject.SetActive(bounce);
            bounceImage.color = col;
        }
        if (pierceImage)
        {
            pierceImage.gameObject.SetActive(pierce);
            pierceImage.color = col;
        }

        // 粒子与光照
        if (gunParticles) gunParticles.startColor = col;
        if (gunLight)     gunLight.color = col;

        // 射击输入
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
        {
            Shoot(bounce, pierce, col);
        }

        // 关闭光效
        if (gunLight && timer >= timeBetweenBullets * effectsDisplayTime)
        {
            gunLight.enabled = false;
        }
    }

    void Shoot(bool bounce, bool pierce, Color bulletColor)
    {
        timer = 0f;

        if (bulletPrefab == null || bulletSpawnAnchor == null)
        {
            Debug.LogError("PlayerShooting: bulletPrefab 或 spawnAnchor 未设置");
            return;
        }

        // 音效
        gunAudio?.Play();
        // 光效
        if (gunLight)
        {
            gunLight.intensity = 1 + 0.5f * (numberOfBullets - 1);
            gunLight.enabled = true;
        }
        // 粒子
        if (gunParticles)
        {
            gunParticles.Stop();
            gunParticles.startSize = 1 + 0.1f * (numberOfBullets - 1);
            gunParticles.Play();
        }

        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = i * angleBetweenBullets
                        - (angleBetweenBullets * (numberOfBullets - 1) / 2f);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up)
                             * bulletSpawnAnchor.rotation;

            if (PhotonNetwork.IsConnected && photonView.IsMine)
            {
                // 多人模式网络实例化
                GameObject b = PhotonNetwork.Instantiate(
                    bulletPrefab.name,
                    bulletSpawnAnchor.position,
                    rot
                );
                var bs = b.GetComponent<Bullet>();
                if (bs != null)
                {
                    // 仅在多人模式重写伤害值为 6
                    bs.damage      = multiplayerBulletDamage;
                    bs.bounce      = bounce;
                    bs.piercing    = pierce;
                    bs.bulletColor = bulletColor;
                }
            }
            else
            {
                // 单人模式本地实例化，使用预制体默认 damage
                GameObject b = Instantiate(
                    bulletPrefab,
                    bulletSpawnAnchor.position,
                    rot
                );
                var bs = b.GetComponent<Bullet>();
                if (bs != null)
                {
                    bs.bounce      = bounce;
                    bs.piercing    = pierce;
                    bs.bulletColor = bulletColor;
                }
            }
        }
    }
}
