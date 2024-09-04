using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        volume = FindObjectOfType<Volume>();
        RestoreHealth();
    }
    private void ChangeIndicator()
    {
        if (volume.profile.TryGet(out Vignette vignette)) // for e.g set vignette intensity to .4f
        {
            vignette.intensity.value = (1 - (float)health / maxHealth)/2;
        }
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint taken: " + other.name);
            Checkpoint checkpoint = other.GetComponent<Checkpoint>();
            if (checkpoint != null)
            {
                OnCheckpointTake?.Invoke(checkpoint);
            }
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
