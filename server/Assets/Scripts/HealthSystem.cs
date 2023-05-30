using UnityEngine;
using System;

public class HealthSystem {
    private int health;
    private int maxHealth;

    public EventHandler<EventArgs> OnHealthChanged;
    public EventHandler<EventArgs> OnDead;

    public HealthSystem(int maxHealth) {
        health = maxHealth;
        this.maxHealth = maxHealth;
    }

    public int GetHealth() {
        return health;
    }

    public void Damage(int amount) {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(this, new EventArgs());
        if (health == 0) OnDead?.Invoke(this, new EventArgs());
    }
    
    public void Heal(int amount) {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(this, new EventArgs());
    }
}