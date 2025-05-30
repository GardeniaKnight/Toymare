using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

	// The time in seconds between each attack.
	public float timeBetweenAttacks = 0.5f;  
	// The amount of health taken away per attack.
	public int attackDamage = 10;               
	   
	// Reference to the player GameObject.
	GameObject player;       
	// Reference to the player's health.
	PlayerHealth playerHealth;     
	// Reference to this enemy's health.
	EnemyHealth enemyHealth; 
	// Whether player is within the trigger collider and can be attacked.
	bool playerInRange;   
	// Timer for counting up to the next attack.
	float timer;                               
	
	void Awake() {
		// Setting up the references.
		// player = GameObject.FindGameObjectWithTag("Player");
		// playerHealth = player.GetComponent<PlayerHealth>();
		enemyHealth = GetComponent<EnemyHealth>();
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			player = other.gameObject;
			playerHealth = player.GetComponent<PlayerHealth>();
			playerInRange = true;
			timer = 0.2f;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject == player) {
			playerInRange = false;
		}
	}
	
	
	void Update() {
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;
		
		// If the timer exceeds the time between attacks, the player is in range,
		// we are alive and the player is alive then attack.
		if (timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0) {
			Attack();
		}
	}
	
	
	void Attack() {
		// Reset the timer.
		timer = 0f;

		if (playerHealth == null) {
			Debug.LogWarning("[EnemyAttack] 找不到玩家或 PlayerHealth，跳过攻击");
			return;
		}

		playerHealth.TakeDamage(attackDamage);
	}
}