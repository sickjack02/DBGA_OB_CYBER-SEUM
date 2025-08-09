using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public float health, currentHealth;

    [SerializeField] FloatingHealhtBar healthBar;
    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealhtBar>();
    }

    void Start()
    {
        healthBar.UpdateHealthBar(currentHealth, health);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHit(float damageValue)
    {
        currentHealth -= damageValue;
        healthBar.UpdateHealthBar(currentHealth, health);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
