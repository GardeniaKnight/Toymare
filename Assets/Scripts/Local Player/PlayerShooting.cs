using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerShooting : NetworkBehaviour
{
    public Color[] bulletColors;
    public float bounceDuration = 10;
    public float pierceDuration = 10;

    public int damagePerShot = 20;
    public int numberOfBullets = 1;
    public float timeBetweenBullets = 0.15f;
    public float angleBetweenBullets = 10f;

    public float range = 100f;
    public LayerMask shootableMask;
    public Image bounceImage;
    public Image pierceImage;

    public GameObject bullet;
    public Transform bulletSpawnAnchor;

    float timer;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    AudioSource gunAudio;
    Light gunLight;
    ParticleSystem gunParticles;
    float effectsDisplayTime = 0.2f;

    public float BounceTimer { get => bounceTimer; set => bounceTimer = value; }
    public float PierceTimer { get => pierceTimer; set => pierceTimer = value; }

    void Awake()
    {
        gunParticles = GetComponentInChildren<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }

    void Update()
    {
        // Debug.Log("🌀 [Update] PlayerShooting is running...");
        // // ✅ 仅当 Netcode 存在时再判断是否为本地玩家（多人模式下）
        // if (Unity.Netcode.NetworkManager.Singleton != null && !IsOwner)
        //     return;
        ////这里以后要改，多人模式不能这样

        // ✅ 状态逻辑
        bounce = bounceTimer < bounceDuration;
        piercing = pierceTimer < pierceDuration;

        // ✅ 设置子弹颜色（状态优先级：双属性 > 穿透 > 反弹 > 默认）
        bulletColor = bulletColors[0];
        if (bounce && piercing)
        {
            bulletColor = bulletColors[3];
        }
        else if (piercing)
        {
            bulletColor = bulletColors[2];
        }
        else if (bounce)
        {
            bulletColor = bulletColors[1];
        }

        // ✅ 更新 UI 显示与颜色（空检查）
        if (bounceImage != null)
        {
            bounceImage.gameObject.SetActive(bounce);
            bounceImage.color = bulletColor;
        }

        if (pierceImage != null)
        {
            pierceImage.gameObject.SetActive(piercing);
            pierceImage.color = bulletColor;
        }

        // ✅ 粒子与光照颜色
        if (gunParticles != null)
            gunParticles.startColor = bulletColor;

        if (gunLight != null)
            gunLight.color = bulletColor;

        // ✅ 时间推进
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        Debug.Log("🌀 [Update] PlayerShooting is running...");
    
        Debug.Log("⛳ Fire1 status: " + Input.GetButton("Fire1"));
        Debug.Log("⏱ Timer: " + timer + " / " + timeBetweenBullets);

        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
        {
            Debug.Log("✅ Fire1 triggered, calling Shoot()");
            Shoot();
        }

        // ✅ 判断是否需要关闭枪口光效
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffects();
        }
    }


    public void DisableEffects()
    {
        if (gunLight != null)
            gunLight.enabled = false;
    }

    void Shoot()
    {
        timer = 0f;
        Debug.Log("[Shoot] Trying to shoot...");
        if (bullet == null)
        {
            Debug.LogError("bullet prefab 未设置！");
            return;
        }
        if (bulletSpawnAnchor == null)
        {
            Debug.LogError("bulletSpawnAnchor 未设置！");
            return;
        }

        if (gunAudio != null)
        {
            gunAudio.pitch = piercing && bounce ? 0.95f :
                             piercing ? 1.05f :
                             bounce ? 1.15f : 1.25f;
            gunAudio.Play();
        }

        if (gunLight != null)
        {
            gunLight.intensity = 1 + 0.5f * (numberOfBullets - 1);
            gunLight.enabled = true;
        }

        if (gunParticles != null)
        {
            gunParticles.Stop();
            gunParticles.startSize = 1 + 0.1f * (numberOfBullets - 1);
            gunParticles.Play();
        }

        for (int i = 0; i < numberOfBullets; i++)
        {
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2f) * (numberOfBullets - 1));
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * bulletSpawnAnchor.rotation;

            GameObject b = Instantiate(bullet, bulletSpawnAnchor.position, rot);
            Debug.Log("✅ Bullet instantiated at: " + b.transform.position);
            Bullet bulletScript = b.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.piercing = piercing;
                bulletScript.bounce = bounce;
                bulletScript.bulletColor = bulletColor;
            }
        }
    }
}
