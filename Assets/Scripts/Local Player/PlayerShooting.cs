using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPun
{
    [Header("子弹与特效")]
    public GameObject bulletPrefab;        // Inspector 里关联你的 Bullet 预制体
    public Transform  bulletSpawnAnchor;   // Inspector 里关联子弹生成锚点
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
    [Tooltip("仅在多人模式：每颗子弹造成的伤害")]
    public int multiplayerBulletDamage = 6;

    // 私有计时器
    private float timer;
    private float bounceTimer;
    private float pierceTimer;

    // 给 Pickup 等脚本用
    public float BounceTimer { get => bounceTimer; set => bounceTimer = value; }
    public float PierceTimer { get => pierceTimer; set => pierceTimer = value; }

    void Awake()
    {
        // 拿到特效组件
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunAudio     = GetComponent<AudioSource>();
        gunLight     = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;

        // —— 修复点 —— 只在真正的多人模式（已进房间）下去禁用“非本地”实例
        if (PhotonNetwork.InRoom && photonView != null && !photonView.IsMine)
        {
            enabled = false;
            return;
        }
        // 单人模式下，这里不会触发，所以脚本会保持启用
    }

    void Update()
    {
        if (!enabled) return;

        // 累加计时
        timer       += Time.deltaTime;
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;

        // 计算状态 & 颜色
        bool bounce = bounceTimer < bounceDuration;
        bool pierce = pierceTimer < pierceDuration;
        Color col = bulletColors.Length > 0 ? bulletColors[0] : Color.white;
        if (bounce && pierce)      col = bulletColors.Length > 3 ? bulletColors[3] : col;
        else if (pierce)           col = bulletColors.Length > 2 ? bulletColors[2] : col;
        else if (bounce)           col = bulletColors.Length > 1 ? bulletColors[1] : col;

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

        // 更新粒子 & 光照
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
            Debug.LogError("PlayerShooting: bulletPrefab 或 bulletSpawnAnchor 未设置");
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

            if (PhotonNetwork.InRoom && photonView.IsMine)
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
                    bs.damage      = multiplayerBulletDamage;
                    bs.bounce      = bounce;
                    bs.piercing    = pierce;
                    bs.bulletColor = bulletColor;
                }
            }
            else
            {
                // 单人模式本地实例化
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
