
using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public float timeBetweenAttacks = 0.5f;    // 攻击间隔
    public int attackDamage = 10;              // 攻击伤害

    GameObject player;                         // 玩家对象引用
    PlayerHealth playerHealth;                 // 玩家生命脚本
    EnemyHealth enemyHealth;                   // 敌人自身生命脚本
    bool playerInRange;                        // 玩家是否在攻击范围内
    float timer;                               // 攻击计时器

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerInRange = true;
                timer = 0.2f; // 延迟以模拟“反应时间”
                Debug.Log("[EnemyAttack] 玩家进入攻击范围");
            }
            else
            {
                Debug.LogWarning("[EnemyAttack] 玩家对象没有 PlayerHealth 脚本");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
            Debug.Log("[EnemyAttack] 玩家离开攻击范围");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
		Debug.Log("Enemyattack update called");
		if (timer >= timeBetweenAttacks &&
			playerInRange &&
			enemyHealth != null && enemyHealth.currentHealth > 0 &&
			playerHealth != null && playerHealth.IsAlive())
		{
			Attack();
			Debug.Log("Enemyattack attack called");
        }
    }

    void Attack()
    {
        timer = 0f;

        if (playerHealth != null)
        {
            Debug.Log("[EnemyAttack] 攻击玩家");
            playerHealth.TakeDamage(attackDamage);
        }
        else
        {
            Debug.LogWarning("[EnemyAttack] 无法攻击，PlayerHealth 为空");
        }
    }
}
