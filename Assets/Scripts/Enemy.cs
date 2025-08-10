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
    [SerializeField] private float distanceToPlayer;

    [Header("Chasing Settings")]
    public float chaseSpeed;

    [Header("Melee Attack Settings")]
    public float meleeAttackDistance;
    [SerializeField] private float meleeAttackCooldown = 1.5f;
    private float meleeAttackTimer = 0f;

    [Header("Range Attack Settings")]
    public float rangeAttackDistance;
    [SerializeField] private float rangeAttackCooldown = 1.5f; // i secondi che deve aspettare per sparare dopo che spara
    private float rangeAttackTimer = 0f; // parte da 0 perché poi lo faccio andare a rangeAttackCooldownquando ha sparato
    public GameObject enemyBulletPrefab;
    public Transform shootPoint;
    public int bulleteDamage;

    [Header("Retreat Settings")]
    public float retreatDistance;
    public float rangeRetreatDistance;
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
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (rangeAttackTimer > 0)
        {
            rangeAttackTimer -= Time.deltaTime;
        }

        if (meleeAttackTimer > 0)
        {
            meleeAttackTimer -= Time.deltaTime;
        }

        /*if(distanceToPlayer <= rangeRetreatDistance)
        {
            ChangeState(AIState.Retreat);
        }*/

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

    private void MeleeAttack(float distance)
    {
        agent.ResetPath();

        if (meleeAttackTimer <= 0)
        {
            Debug.Log("Attacco melee");
            meleeAttackTimer = meleeAttackCooldown;
        }

        if (distance > meleeAttackDistance) { ChangeState(AIState.Chasing); }
    }

    private void RangeAttack(float distance)
    {
        agent.ResetPath();
        
        if (rangeAttackTimer <= 0) {
            ShootProjectile();
            rangeAttackTimer = rangeAttackCooldown;
        }

        ChangeState(AIState.Chasing);
    }

    void ShootProjectile()
    {
        if (enemyBulletPrefab && shootPoint)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, shootPoint.position, shootPoint.rotation);
            bullet.GetComponent<EnemyBullet>().SetDirection(transform.forward);
            bullet.GetComponent<EnemyBullet>().SetDamage(bulleteDamage);
        }
    }

    private void RetreatFromPlayer(float distance)
    {
        agent.updateRotation = false;

        retreatTimer -= Time.deltaTime;

        Vector3 dirAway = (transform.position - player.transform.position).normalized;
        agent.ResetPath();
        agent.SetDestination(transform.position +  dirAway * retreatDistance);

        Vector3 lookDir = player.transform.position - transform.position;
        lookDir.y = 0;
        if(lookDir != Vector3.zero) 
            transform.rotation = Quaternion.LookRotation(lookDir);

        if (retreatTimer <= 0)
        { // dopo che ha finito di scappare controllo se il player si trova in range di sparo
            // se sì sparo
            if(distanceToPlayer <= rangeAttackDistance)
                ChangeState(AIState.Range_Attack);
            // se no inseguo
            else
                ChangeState(AIState.Chasing);
            
            Debug.Log("Dopo ritirata uso " + currentState);
        }
        else if (distance <= meleeAttackDistance)
            ChangeState(AIState.Melee_Attack);

        agent.updateRotation = true;
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
