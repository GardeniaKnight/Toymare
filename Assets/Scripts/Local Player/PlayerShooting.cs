using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    /// <summary>
    /// 子弹颜色
    /// </summary>
    public Color[] bulletColors;

    /// <summary>
    /// 反弹周期
    /// </summary>
    public float bounceDuration = 10;

    /// <summary>
    /// 穿透时间
    /// </summary>
    public float pierceDuration = 10;


    // 子弹伤害
    public int damagePerShot = 20;

    //子弹个数
    public int numberOfBullets = 1;


    // 每次射击间隔的时间
    public float timeBetweenBullets = 0.15f;

    //子弹角度
    public float angleBetweenBullets = 10f;


    // 射击的范围
    public float range = 100f;
    //射击层标记
    public LayerMask shootableMask;
    // Reference to the UI's green health bar.
    public Image bounceImage;
    // Reference to the UI's red health bar.
    public Image pierceImage;

    public GameObject bullet;

    /// <summary>
    /// 子弹生成锚点
    /// </summary>
    public Transform bulletSpawnAnchor;

    // 开火的计时器
    float timer;
    // 射击射线
    Ray shootRay;
    // 击中点
    RaycastHit shootHit;
    // 枪的粒子效果
    ParticleSystem gunParticles;
    // 枪线
    LineRenderer gunLine;
    // Reference to the audio source.
    AudioSource gunAudio;
    // Reference to the light component.
    Light gunLight;
    // 呈现时间
    float effectsDisplayTime = 0.2f;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    public float BounceTimer
    {
        get { return bounceTimer; }
        set { bounceTimer = value; }
    }

    public float PierceTimer
    {
        get { return pierceTimer; }
        set { pierceTimer = value; }
    }

    void Awake()
    {
        // Set up the references.
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }

    void Update()
    {
        if (bounceTimer < bounceDuration)
        {
            bounce = true;
        }
        else
        {
            bounce = false;
        }

        if (pierceTimer < pierceDuration)
        {
            piercing = true;
        }
        else
        {
            piercing = false;
        }

        bulletColor = bulletColors[0];
        if (bounce)
        {
            bulletColor = bulletColors[1];

            bounceImage.color = bulletColors[1];
        }
        bounceImage.gameObject.SetActive(bounce);

        if (piercing)
        {
            bulletColor = bulletColors[2];

            pierceImage.color = bulletColors[2];
        }
        pierceImage.gameObject.SetActive(piercing);

        if (piercing & bounce)
        {
            bulletColor = bulletColors[3];
            bounceImage.color = bulletColors[3];
            pierceImage.color = bulletColors[3];
        }

        gunParticles.startColor = bulletColor;
        gunLight.color = bulletColor;

        // Add the time since Update was last called to the timer.
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // If the Fire1 button is being press and it's time to fire...
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets)
        {
            // ... shoot the gun.
            Shoot();
        }

        // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            // ... disable the effects.
            DisableEffects();
        }
    }

    public void DisableEffects()
    {
        // Disable the line renderer and the light.
        gunLight.enabled = false;
    }

    void Shoot()
    {
        // Reset the timer.
        timer = 0f;

        // Play the gun shot audioclip.
        gunAudio.pitch = Random.Range(1.2f, 1.3f);

        if (bounce)
        {
            gunAudio.pitch = Random.Range(1.1f, 1.2f);
        }

        if (piercing)
        {
            gunAudio.pitch = Random.Range(1.0f, 1.1f);
        }

        if (piercing & bounce)
        {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        gunAudio.Play();

        // Enable the light.
        gunLight.intensity = 1 + (0.5f * (numberOfBullets - 1));
        gunLight.enabled = true;

        // Stop the particles from playing if they were, then start the particles.
        gunParticles.Stop();
        gunParticles.startSize = 1 + (0.1f * (numberOfBullets - 1));
        gunParticles.Play();

        // Set the shootRay so that it starts at the end ofres the gun and points forward from the barrel.
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        for (int i = 0; i < numberOfBullets; i++)
        {
            // Make sure our bullets spread out in an even pattern.
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bullet, bulletSpawnAnchor.transform.position, rot) as GameObject;
            instantiatedBullet.GetComponent<Bullet>().piercing = piercing;
            instantiatedBullet.GetComponent<Bullet>().bounce = bounce;
            instantiatedBullet.GetComponent<Bullet>().bulletColor = bulletColor;
        }
    }
}