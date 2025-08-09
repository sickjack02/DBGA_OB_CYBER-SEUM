using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{

    public float health;
    private float currentHealth;

    [SerializeField] FloatingHealhtBar healthBar;
    [SerializeField] private GameObject floatingTextPrefab;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealhtBar>();
        currentHealth = health;
    }

    void Start()
    {
        healthBar.UpdateHealthBar(currentHealth, health);
    }

    public void OnHit(float damageValue)
    {
        currentHealth -= damageValue;
        healthBar.UpdateHealthBar(currentHealth, health);
        healthBar.ShowDamage(damageValue.ToString());
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }


}
