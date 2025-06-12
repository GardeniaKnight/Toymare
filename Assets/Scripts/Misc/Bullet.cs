
// Bullet.cs
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 600f;
    public float life = 3f;
    public ParticleSystem normalTrailParticles;
    public ParticleSystem bounceTrailParticles;
    public ParticleSystem pierceTrailParticles;
    public ParticleSystem ImpactParticles;
    public int damage = 20;
    public bool piercing = false;
    public bool bounce = false;
    public Color bulletColor;
    public AudioClip bounceSound;
    public AudioClip hitSound;

    private Vector3 velocity;
    private Vector3 newPos;
    private Vector3 oldPos;
    private Vector3 direction;
    private bool hasHit = false;
    private RaycastHit lastHit;
    private AudioSource bulletAudio;
    private float timer;

    void Awake()
    {
        bulletAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        newPos = transform.position;
        oldPos = newPos;

        normalTrailParticles.startColor = bulletColor;
        bounceTrailParticles.startColor = bulletColor;
        pierceTrailParticles.startColor = bulletColor;
        ImpactParticles.startColor = bulletColor;

        normalTrailParticles.gameObject.SetActive(!bounce && !piercing);
        if (bounce)
        {
            bounceTrailParticles.gameObject.SetActive(true);
            speed = 20f;
            life  = 1f;
        }
        if (piercing)
        {
            pierceTrailParticles.gameObject.SetActive(true);
            speed = 40f;
        }
    }

    void Update()
    {
        if (hasHit) return;

        timer += Time.deltaTime;
        if (timer >= life)
        {
            Dissipate();
            return;
        }

        velocity = transform.forward;
        velocity.y = 0f;
        velocity = velocity.normalized * speed;
        newPos += velocity * Time.deltaTime;

        direction = newPos - oldPos;
        float distance = direction.magnitude;
        if (distance > 0f)
        {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);
            foreach (var hit in hits)
            {
                if (lastHit.collider == hit.collider && lastHit.point == hit.point)
                    continue;
                OnHit(hit);
                lastHit = hit;
                if (hasHit)
                {
                    newPos = hit.point;
                    break;
                }
            }
        }

        oldPos = transform.position;
        transform.position = newPos;
    }

    void OnHit(RaycastHit hit)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        // 环境
        if (hit.transform.CompareTag("Environment"))
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
            return;
        }

        // 玩家
        var ph = hit.collider.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            // 已死的玩家不再受击
            if (!ph.IsAlive()) return;

            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();

            var pv = hit.collider.GetComponent<PhotonView>();
            if (pv != null && PhotonNetwork.IsConnectedAndReady)
            {
                pv.RPC("TakeDamage", pv.Owner, damage);
            }
            else
            {
                ph.TakeDamage(damage);
            }

            // —— 新增：向 MasterClient 报告得分
            // this.photonView 是子弹的 PhotonView
            int shooterActor = photonView.OwnerActorNr;
            int dmg = damage;
            NetScoreManager.Instance.photonView.RPC(
                "RPC_AddScore",
                RpcTarget.MasterClient,
                shooterActor,
                dmg
            );

            if (!piercing)
            {
                hasHit = true;
                DelayedDestroy();
            }

            bulletAudio.clip = hitSound;
            bulletAudio.volume = 0.5f;
            bulletAudio.pitch = Random.Range(1.2f, 1.3f);
            bulletAudio.Play();
            return;
        }

        // 敌人
        if (hit.transform.CompareTag("Enemy"))
        {
            ImpactParticles.transform.position = hit.point;
            ImpactParticles.transform.rotation = rotation;
            ImpactParticles.Play();

            var enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage, hit.point);

            if (!piercing)
            {
                hasHit = true;
                DelayedDestroy();
            }

            bulletAudio.clip = hitSound;
            bulletAudio.volume = 0.5f;
            bulletAudio.pitch = Random.Range(1.2f, 1.3f);
            bulletAudio.Play();
            return;
        }
    }

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

    void DelayedDestroy()
    {
        normalTrailParticles.gameObject.SetActive(false);
        if (bounce)    bounceTrailParticles.gameObject.SetActive(false);
        if (piercing)  pierceTrailParticles.gameObject.SetActive(false);
        Destroy(gameObject, hitSound.length);
    }
}
