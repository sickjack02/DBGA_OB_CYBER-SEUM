using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.XR;

using Random = System.Random;

public class Enemy : MonoBehaviour
{

    public float health;
    private float currentHealth;
    
    [SerializeField] FloatingHealhtBar healthBar;
    [SerializeField] private GameObject floatingTextPrefab;

    private GameObject player;

    enum AIState
    {
        Chasing, Melee_Attack, Range_Attack, Retreat
    }

    [Header("AI State")]
    [SerializeField] private AIState currentState;

    [Header("Components")]
    NavMeshAgent agent;

    [Header("Chasing Settings")]
    public float chaseSpeed;

    [Header("Melee Attack Settings")]
    public float meleeAttackDistance;

    [Header("Range Attack Settings")]
    public float rangeAttackDistance;
    public GameObject enemyBulletPrefab;
    public Transform shootPoint;
    private float rangeAttackCooldown = 1.5f; // i secondi che deve aspettare per sparare dopo che spara
    private float rangeAttackTimer = 0f; // parte da 0 perché poi lo faccio andare a rangeAttackCooldownquando ha sparato

    [Header("Retreat Settings")]
    public float retreatDistance;
    private float retreatTimer = 3f;

    private void Awake()
    {
        healthBar = GetComponentInChildren<FloatingHealhtBar>();
        currentHealth = health;
    }

    void Start()
    {
        healthBar.UpdateHealthBar(currentHealth, health);

        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (rangeAttackTimer > 0)
        {
            rangeAttackTimer -= Time.deltaTime;
        }

        switch (currentState) 
        { 
            case AIState.Chasing:
                ChasePlayer(distanceToPlayer); 
                break;
            case AIState.Melee_Attack:
                MeleeAttack(distanceToPlayer);
                break;
            case AIState.Range_Attack:
                RangeAttack(distanceToPlayer);
                break;
            case AIState.Retreat:
                RetreatFromPlayer(distanceToPlayer);
                break;
        }
    }

    private void MeleeAttack(float distance)
    {
        agent.ResetPath();
        Debug.Log("Attacco melee");

        if (distance > meleeAttackDistance) { ChangeState(AIState.Chasing); }
    }

    private void RangeAttack(float distance)
    {
        agent.ResetPath();
        Debug.Log("In range per il range");

        if (rangeAttackCooldown <= 0)
        {
            Debug.Log("Attacco range");
            // metodo per sparare
            rangeAttackTimer = rangeAttackCooldown;
        }

        // una volta sparato controllo dove si trova il Player per sparare ancora o inseguirlo
        if (distance <= meleeAttackDistance)
            ChangeState(AIState.Melee_Attack);
        else if (distance > rangeAttackDistance)
            ChangeState(AIState.Chasing);

    }

    private void RetreatFromPlayer(float distance)
    {
        retreatTimer -= Time.deltaTime;

        Vector3 dirAway = (transform.position - player.transform.position).normalized;
        agent.SetDestination(transform.position +  dirAway * retreatDistance);

        if (retreatTimer <= 0)
        {
            ChangeState(UnityEngine.Random.value > 0.5 ? AIState.Range_Attack : AIState.Chasing);
            Debug.Log("Dopo ritirata uso " + currentState);
        }
        else if (distance <= meleeAttackDistance)
            ChangeState(AIState.Melee_Attack);
    }

    private void ChasePlayer(float distance)
    {
        agent.SetDestination(player.transform.position);

        if (distance <= meleeAttackDistance)
        {
            ChangeState(AIState.Melee_Attack);
        }
        else if (distance <= rangeAttackDistance)
        {
            ChangeState(AIState.Range_Attack);
        }
    }

    private void ChangeState(AIState newState)
    {
        currentState = newState;

        if (newState == AIState.Retreat)
            retreatTimer = retreatTimer;
    }

    public void OnHit(float damageValue)
    {
        currentHealth -= damageValue;

        string showDamage = "-" + damageValue.ToString();

        healthBar.UpdateHealthBar(currentHealth, health);
        healthBar.ShowDamage(showDamage);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }


}
