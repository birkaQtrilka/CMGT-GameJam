using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Range(0, 100)]
    public int maxHealth;
    private float damageCooldown;
    public float damageCooldownMax;
    public int health { get; private set; }
    public event Action OnDeath;
    public event Action<Checkpoint> OnCheckpointTake;
    // Start is called before the first frame update

    public Volume volume;
    void Start()
    {
        CheckpointManager.checkpointManager.player = this; 
        volume = FindObjectOfType<Volume>();
        Respawn();
    }
    private void Awake()
    {
    }
    private void ChangeIndicator()
    {
        if (volume.profile.TryGet(out Vignette vignette)) // for e.g set vignette intensity to .4f
        {
            vignette.intensity.value = (1 - (float)health / maxHealth)/2;
        }
    }
    public void Respawn()
    {
        transform.position = CheckpointManager.checkpointManager.currentCheckpoint.position + new Vector3(0, 1, 0);
        RestoreHealth();
        SoundManager.main.PlayerRespawn();
    }
    public void RestoreHealth()
    {
        health = maxHealth;
        ChangeIndicator();
    }
    public void Kill()
    {
        TakeDamage(maxHealth);
    }
    public void TakeDamage(int damage)
    {
        if (damageCooldown > 0) return;
        damageCooldown = damageCooldownMax;
        health -= damage;
        ClampHealth();
        if (health == 0)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            OnDeath?.Invoke();
        }
        ChangeIndicator();
    }
    public void Heal (int healed)
    {
        TakeDamage(-healed);
    }
    private void ClampHealth()
    {
        if (health > maxHealth)
            health = maxHealth;
        else if (health < 0)
            health = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (damageCooldown > 0)
            damageCooldown-= Time.deltaTime;
        else
            
        if (Input.GetKeyDown(KeyCode.R))
        {
            Kill();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Danger"))
        {
            DangerousObject danger = other.GetComponent<DangerousObject>();
            if (danger != null)
                TakeDamage(danger.contactDamage);
        }
    }
}
