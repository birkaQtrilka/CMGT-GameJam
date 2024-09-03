using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Range(0f, int.MaxValue)]
    public int maxHealth;
    public int health { get; private set; }
    public Action OnDeath;
    // Start is called before the first frame update
    void Start()
    {
        RestoreHealth();
    }

    public void RestoreHealth()
    {
            health = maxHealth;
    }
    public void Kill()
    {
        health = 0;
        OnDeath?.Invoke();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        ClampHealth();
        if (health == 0)
            OnDeath?.Invoke();
    }
    public void Heal (int healed)
    {
        health += healed;
        ClampHealth();
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
        
    }
}
