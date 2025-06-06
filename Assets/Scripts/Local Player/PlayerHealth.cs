using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public float invulnerabilityTime = 1f;
    public float timeAfterWeLastTookDamage = 1f;
    public Slider healthSliderForeground;
    public Slider healthSliderBackground;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    public int currentHealth;
    private Animator anim;
    private AudioSource playerAudio;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private bool isDead;
    private bool damaged;
    public float invulnerableTimer;
    private float backgroundLerpTimer;
    private SkinnedMeshRenderer myRenderer;
    private Color rimColor;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponentInChildren<PlayerShooting>();

        currentHealth = startingHealth;

        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            if (meshRenderer.gameObject.name == "Player")
            {
                myRenderer = meshRenderer;
                break;
            }
        }
    }

    void Start()
    {
        rimColor = myRenderer.materials[0].GetColor("_RimColor");
    }

    void Update()
    {
        if (damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        invulnerableTimer += Time.deltaTime;
        backgroundLerpTimer += Time.deltaTime;

        if (backgroundLerpTimer >= timeAfterWeLastTookDamage)
        {
            healthSliderBackground.value = Mathf.Lerp(healthSliderBackground.value, healthSliderForeground.value, 2f * Time.deltaTime);
        }

        damaged = false;
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("TakeDamage called");
        // if (invulnerableTimer < invulnerabilityTime)
        // {
        //     Debug.Log("[TakeDamage] 无敌中，忽略伤害");
        //     return;
        // }

        Debug.Log($"[TakeDamage] 受到伤害：{amount}");
        
        invulnerableTimer = 0f;
        backgroundLerpTimer = 0f;
        
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");
        
        damaged = true;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);
        if (healthSliderForeground == null)
        {
            Debug.Log("Slider null!");
        }
        healthSliderForeground.value = currentHealth;//不扣血的问题在这里
        Debug.Log("Here12");
        playerAudio.Play();
        Debug.Log("Here11");
        Debug.Log($"TakeDamage 后当前血量：{currentHealth}");
        if (currentHealth <= 0 && !isDead)
        {
            Death();
            Debug.Log("Here10");
        }
    }

    IEnumerator Ishit()
    {
        Color newColor = new Color(10, 0, 0, 0);
        myRenderer.materials[0].SetColor("_RimColor", newColor);

        float time = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            if (elapsedTime < (time / 2f))
            {
                newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
            }
            myRenderer.materials[0].SetColor("_RimColor", newColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, startingHealth);
        healthSliderForeground.value = currentHealth;
    }

    void Death()
    {
        isDead = true;

        if (playerShooting != null)
            playerShooting.DisableEffects();

        if (anim != null)
            anim.SetTrigger("Die");

        playerAudio.clip = deathClip;
        playerAudio.Play();

        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerShooting != null)
            playerShooting.enabled = false;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
