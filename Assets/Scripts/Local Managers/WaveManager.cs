using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{

    // Reference to the player's heatlh.
    public PlayerHealth playerHealth;
    // The enemy prefab to be spawned.
    public GameObject[] enemies;
    // The distance from our Camera View Frustrum we want to spawn enemies
    // to make sure they are not visisble when they spawn. I'm too lazy to
    // do any proper checks.
    public float bufferDistance = 200;
    // The time in seconds between each wave.
    public float timeBetweenWaves = 5f;
    public float spawnTime = 3f;
    public int startingWave = 1;
    public int startingDifficulty = 1;
    // Reference to the Text component.
    public Text number;
    [HideInInspector]
    public int enemiesAlive = 0;

    Vector3 spawnPosition = Vector3.zero;
    int waveNumber;
    float timer;
    Wave currentWave;
    int spawnedThisWave = 0;
    int totalToSpawnForWave;
    bool shouldSpawn = false;
    int difficulty;

    public class Wave
    {
        public List<Entry> entries;

        public Wave()
        {
            this.entries = new List<Entry>();
        }

        public class Entry
        {
            public GameObject enemy;
            public int count;
            public int spawned;

            public Entry(GameObject enemy, int count)
            {
                this.enemy = enemy;
                this.count = count;
                this.spawned = 0;
            }
        }
    }

    List<Wave> waves = new List<Wave>();

    void Start()
    {
        // Let us start on a higher wave and difficulty if we wish.
        waveNumber = startingWave > 0 ? startingWave - 1 : 0;
        difficulty = startingDifficulty;

        // Create our waves.
        CreateWaves();

        // Start the next, ie. the first wave.
        StartCoroutine("StartNextWave");
    }

    void Update()
    {
        // This is false while we're setting up the next wave.
        if (!shouldSpawn)
        {
            return;
        }

        // Start the next wave when we've spawned all our enemies and the player
        // has killed them all.
        if (spawnedThisWave == totalToSpawnForWave && enemiesAlive == 0)
        {
            StartCoroutine("StartNextWave");
            return;
        }

        // Add the time since Update was last called to the timer.
        timer += Time.deltaTime;

        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive attack.
        if (timer >= spawnTime)
        {
            // Spawn one enemy from each of the entries in this wave.
            foreach (Wave.Entry entry in currentWave.entries)
            {
                if (entry.spawned < entry.count)
                {
                    Spawn(entry);
                }
            }
        }
    }

    /**
     * 
     */
    IEnumerator StartNextWave()
    {
        shouldSpawn = false;

        yield return new WaitForSeconds(timeBetweenWaves);

        if (waveNumber == waves.Count)
        {
            waveNumber = 0;
            difficulty++;
            waves = new List<Wave>();
            CreateWaves();
        }

        currentWave = waves[waveNumber];

        totalToSpawnForWave = 0;
        foreach (Wave.Entry entry in currentWave.entries)
        {
            totalToSpawnForWave += entry.count;
        }

        spawnedThisWave = 0;
        shouldSpawn = true;

        waveNumber++;

        number.text = (waveNumber + ((difficulty - 1) * waves.Count)).ToString();
        number.GetComponent<Animation>().Play();
    }

    /**
     * Spawn enemies.
     * 
     * This method is called at regular intervals, but all the ways this function 
     * can end up not spawning an enemy means it could be many intervals between each 
     * actual spawn and our enemies will spawn very irregularly. I guess that just 
     * makes it seem more random though. And I'm lazy. :p
     */
    void Spawn(Wave.Entry entry)
    {
        // Reset the timer.
        timer = 0f;

        // If the player has no health left, stop spawning.
        if (playerHealth.currentHealth <= 0f)
        {
            return;
        }

        // Find a random position roughly on the level.
        Vector3 randomPosition = Random.insideUnitSphere * 35;
        randomPosition.y = 0;

        // Find the closest position on the nav mesh to our random position.
        // If we can't find a valid position return and try again.
        UnityEngine.AI.NavMeshHit hit;
        if (!UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5, 1))
        {
            return;
        }

        // We have a valid spawn position on the nav mesh.
        spawnPosition = hit.position;

        // Check if this position is visible on the screen, if it is we
        // return and try again.
        Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);
        if ((screenPos.x > -bufferDistance && screenPos.x < (Screen.width + bufferDistance)) &&
            (screenPos.y > -bufferDistance && screenPos.y < (Screen.height + bufferDistance)))
        {
            return;
        }

        // We passed all the checks, spawn our enemy.
        GameObject enemy = Instantiate(entry.enemy, spawnPosition, Quaternion.identity) as GameObject;
        // Multiply health and score value by the current difficulty.
        enemy.GetComponent<EnemyHealth>().startingHealth *= difficulty;
        enemy.GetComponent<EnemyHealth>().scoreValue *= difficulty;

        entry.spawned++;
        spawnedThisWave++;
        enemiesAlive++;
    }

    /**
     * Set up all our waves of enemies.
     * 
     * Should be done through a custom editor inspector,
     * but again... lazy.
     */
    void CreateWaves()
    {
        Wave wave1 = new Wave();
        wave1.entries.Add(new Wave.Entry(enemies[0], 10 * difficulty));
        waves.Add(wave1);

        Wave wave2 = new Wave();
        wave2.entries.Add(new Wave.Entry(enemies[0], 20 * difficulty));
        waves.Add(wave2);

        Wave wave3 = new Wave();
        wave3.entries.Add(new Wave.Entry(enemies[0], 20 * difficulty));
        wave3.entries.Add(new Wave.Entry(enemies[1], 10 * difficulty));
        waves.Add(wave3);

        Wave wave4 = new Wave();
        wave4.entries.Add(new Wave.Entry(enemies[0], 20 * difficulty));
        wave4.entries.Add(new Wave.Entry(enemies[1], 20 * difficulty));
        waves.Add(wave4);

        Wave wave5 = new Wave();
        wave5.entries.Add(new Wave.Entry(enemies[2], 1 * difficulty));
        waves.Add(wave5);

        Wave wave6 = new Wave();
        wave6.entries.Add(new Wave.Entry(enemies[0], 30 * difficulty));
        wave6.entries.Add(new Wave.Entry(enemies[1], 30 * difficulty));
        waves.Add(wave6);

        Wave wave7 = new Wave();
        wave7.entries.Add(new Wave.Entry(enemies[0], 20 * difficulty));
        wave7.entries.Add(new Wave.Entry(enemies[1], 20 * difficulty));
        wave7.entries.Add(new Wave.Entry(enemies[2], 1 * difficulty));
        waves.Add(wave7);

        Wave wave8 = new Wave();
        wave8.entries.Add(new Wave.Entry(enemies[0], 30 * difficulty));
        wave8.entries.Add(new Wave.Entry(enemies[1], 30 * difficulty));
        wave8.entries.Add(new Wave.Entry(enemies[2], 2 * difficulty));
        waves.Add(wave8);

        Wave wave9 = new Wave();
        wave9.entries.Add(new Wave.Entry(enemies[0], 40 * difficulty));
        wave9.entries.Add(new Wave.Entry(enemies[1], 40 * difficulty));
        wave9.entries.Add(new Wave.Entry(enemies[2], 3 * difficulty));
        waves.Add(wave9);

        Wave wave10 = new Wave();
        wave10.entries.Add(new Wave.Entry(enemies[0], 50 * difficulty));
        wave10.entries.Add(new Wave.Entry(enemies[1], 50 * difficulty));
        wave10.entries.Add(new Wave.Entry(enemies[2], 5 * difficulty));
        waves.Add(wave10);
    }
}