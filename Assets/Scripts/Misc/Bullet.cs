using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    //子弹速度 
    public float speed = 600.0f;

    //子弹生存时间为3秒
    public float life = 3;
    //普通的例子效果
    public ParticleSystem normalTrailParticles;
    //反弹的例子效果
    public ParticleSystem bounceTrailParticles;
    //穿透例子效果
    public ParticleSystem pierceTrailParticles;
    //组合例子效果
    public ParticleSystem ImpactParticles;

    public int damage = 20;

    //是否为穿透弹
    public bool piercing = false;
    //是否为反弹弹
    public bool bounce = false;

    //子弹颜色
    public Color bulletColor;
    public AudioClip bounceSound;
    public AudioClip hitSound;

    //速度
    Vector3 velocity;
    //力量
    Vector3 force;
    //新位置 
    Vector3 newPos;
    //旧位置
    Vector3 oldPos;
    //方向
    Vector3 direction;

    //是否击中目标
    bool hasHit = false;

    RaycastHit lastHit;

    // Reference to the audio source.
    AudioSource bulletAudio;

    float timer;

    void Awake()
    {
        bulletAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        newPos = transform.position;
        oldPos = newPos;

        // Set our particle colors.
        normalTrailParticles.startColor = bulletColor;
        bounceTrailParticles.startColor = bulletColor;
        pierceTrailParticles.startColor = bulletColor;
        ImpactParticles.startColor = bulletColor;

        //一开始的时候使用默认的例子系统
        normalTrailParticles.gameObject.SetActive(true);

        //反弹子弹的话，生命为1，速度为20
        if (bounce)
        {
            bounceTrailParticles.gameObject.SetActive(true);
            normalTrailParticles.gameObject.SetActive(false);
            life = 1;
            speed = 20;
        }

        //穿透弹的话，速度为40
        if (piercing)
        {
            pierceTrailParticles.gameObject.SetActive(true);
            normalTrailParticles.gameObject.SetActive(false);

            speed = 40;
        }
    }

    void Update()
    {
        if (hasHit)
        {
            return;
        }

        // 开始计时
        timer += Time.deltaTime;

        // 开始销毁子弹
        if (timer >= life)
        {
            Dissipate();
        }

        //向前面的位置移动，Y轴上不需要才在速度.
        velocity = transform.forward;
        velocity.y = 0;
        //速度和方向都设定好了
        velocity = velocity.normalized * speed;

        // 子弹新位置
        newPos += velocity * Time.deltaTime;

        // 检测子弹路径上是不是碰撞上什么东西
        direction = newPos - oldPos;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);


            // 找到第一个可用的碰撞点
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (ShouldIgnoreHit(hit))
                {
                    //结束的循环
                    continue;
                }

                // 通知碰撞
                OnHit(hit);

                lastHit = hit;

                if (hasHit)
                {
                    // 结束所有的循环
                    newPos = hit.point;
                    break;
                }
            }
        }

        oldPos = transform.position;
        transform.position = newPos;
    }

    /*
     * So we don't hit the same enemy twice with the same raycast when we have
     * piercing shots. The shot can still bounce on a wall, come back and hit
     * the enemy again if we have both bouncing and piercing shots.
     */
    bool ShouldIgnoreHit(RaycastHit hit)
    {
        if (lastHit.point == hit.point || lastHit.collider == hit.collider)
            return true;

        return false;
    }

    /**
     * 子弹碰撞到游戏对象将发生的事情
     */
    void OnHit(RaycastHit hit)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment")
        {
            newPos = hit.point;
            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();

            if (bounce)
            {
                Vector3 reflect = Vector3.Reflect(direction, hit.normal);
                transform.forward = reflect;
                bulletAudio.clip = bounceSound;
                bulletAudio.pitch = Random.Range(0.8f, 1.2f);
                bulletAudio.Play();
            }
            else
            {
                hasHit = true;
                bulletAudio.clip = hitSound;
                bulletAudio.volume = 0.5f;
                bulletAudio.pitch = Random.Range(1.2f, 1.3f);
                bulletAudio.Play();
                DelayedDestroy();
            }
        }

        if (hit.transform.tag == "Enemy")
        {
            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();

            // Try and find an EnemyHealth script on the gameobject hit.
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();

            // If the EnemyHealth component exist...
            if (enemyHealth != null)
            {
                // ... the enemy should take damage.
                enemyHealth.TakeDamage(damage, hit.point);
            }

            if (!piercing)
            {
                hasHit = true;
                DelayedDestroy();
            }

            bulletAudio.clip = hitSound;
            bulletAudio.volume = 0.5f;
            bulletAudio.pitch = Random.Range(1.2f, 1.3f);
            bulletAudio.Play();
        }
    }

    // Just a method for destroying the game object, but which
    // first detaches the particle effect and leaves it for a
    // second. Called if the bullet end its life in midair
    // so we get an effect of the bullet fading out instead
    // of disappearing immediately.
    //子弹消散的效果
    void Dissipate()
    {
        normalTrailParticles.enableEmission = false;
        normalTrailParticles.transform.parent = null;
        Destroy(normalTrailParticles.gameObject, normalTrailParticles.duration);

        if (bounce)
        {
            bounceTrailParticles.enableEmission = false;
            bounceTrailParticles.transform.parent = null;
            Destroy(bounceTrailParticles.gameObject, bounceTrailParticles.duration);
        }

        if (piercing)
        {
            pierceTrailParticles.enableEmission = false;
            pierceTrailParticles.transform.parent = null;
            Destroy(pierceTrailParticles.gameObject, pierceTrailParticles.duration);
        }

        Destroy(gameObject);
    }


    /// <summary>
    /// 延迟销毁
    /// </summary>
    void DelayedDestroy()
    {
        normalTrailParticles.gameObject.SetActive(false);
        if (bounce)
        {
            bounceTrailParticles.gameObject.SetActive(false);
        }
        if (piercing)
        {
            pierceTrailParticles.gameObject.SetActive(false);
        }
        Destroy(gameObject, hitSound.length);
    }
}