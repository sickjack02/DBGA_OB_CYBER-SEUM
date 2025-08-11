using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.XR;

using Random = System.Random;
using System.Linq;

public class Enemy : MonoBehaviour
{

    public float health;
    private float currentHealth;
    
    [SerializeField] FloatingHealhtBar healthBar;
    [SerializeField] private GameObject floatingTextPrefab;

    private GameObject player;
    public LayerMask playerLayer;

    private Animator animator;
    private float stateTimer;
    [SerializeField] private bool isLockedInState;

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
    public int meleeDamage;
    public Transform hitbox;

    [Header("Range Attack Settings")]
    public float rangeAttackDistance;
    [SerializeField] private float rangeAttackCooldown = 1.5f; // i secondi che deve aspettare per sparare dopo che spara
    [SerializeField] private float rangeAttackTimer = 0f; // parte da 0 perché poi lo faccio andare a rangeAttackCooldownquando ha sparato
    public GameObject enemyBulletPrefab;
    public Transform shootPoint;
    public int bulleteDamage;

    [Header("Retreat Settings")]
    public float retreatDistance;
    public float rangeRetreatDistance;
    private float retreatTimer = 3f;



    //ho notato che per colpa della NavMesh fluttua
    private float groundHeight = 0;

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
        animator = GetComponent<Animator>();
    }

    private void OnDrawGizmosSelected()
    {
        if (hitbox == null)
            return;

        Gizmos.color = Color.red;

        foreach (Transform point in hitbox)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, meleeAttackDistance);
        }
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        // costringo a rimanere a terra, un po' rozzo ma mi dà fastidio che sembra che fluttui
        Vector3 pos = transform.position;
        pos.y = groundHeight; // valore fisso
        transform.position = pos;

        // timer per i colpi melee e range
        if (meleeAttackTimer > 0)
        {
            meleeAttackTimer -= Time.deltaTime;
        }

        if (rangeAttackTimer > 0)
        {
            rangeAttackTimer -= Time.deltaTime;
        }

        /*if(distanceToPlayer <= rangeRetreatDistance)
        {
            ChangeState(AIState.Retreat);
        }*/

        if (isLockedInState) 
        {
            stateTimer -= Time.deltaTime;
            if(stateTimer <= 0) 
                isLockedInState = false;
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
            /*case AIState.Retreat:
                RetreatFromPlayer(distanceToPlayer);
                break;*/
        }
    }
    private void ChasePlayer(float distance)
    {
        agent.SetDestination(player.transform.position);
        animator.SetBool("IsChasing", true);

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
        animator.SetBool("IsChasing", false);


        if (meleeAttackTimer <= 0 && distance <= meleeAttackDistance)
        {
            //Debug.Log("Attacco melee");
            animator.SetTrigger("MeleeAttack");
            meleeAttackTimer = meleeAttackCooldown;
        }

        if (distance >= meleeAttackDistance) {
            ChangeState(AIState.Chasing);
        }
    }

    private void RangeAttack(float distance)
    {
        agent.ResetPath();
        animator.SetBool("IsChasing", false);

        if (rangeAttackTimer <= 0 && distance <= rangeAttackDistance) {
            //Debug.Log("Attacco range");
            animator.SetTrigger("RangeAttack");
            // lo faccio sparare dopo l'avvio dell'animazione, nel momento giusto
            Invoke(nameof(ShootProjectile), 1f);
            rangeAttackTimer = rangeAttackCooldown;
        } 

        if(distance >= rangeAttackDistance)
            ChangeState(AIState.Chasing);
        if (distance <= meleeAttackDistance)
            ChangeState(AIState.Melee_Attack);
        //
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
        if (isLockedInState) return;

        currentState = newState;

        float animDuration = GetCurrentAnimationDuration(newState);
        if (animDuration > 0f)
            LockState(animDuration);

        /*if (newState == AIState.Retreat)
            retreatTimer = retreatTimer;*/
    }

    private void LockState(float duration)
    {
        animator.SetBool("IsChasing", false);
        isLockedInState = true;
        stateTimer = duration;
    }

    private float GetCurrentAnimationDuration(AIState state)
    {
        string animName = "";

        switch (state) 
        {
            case AIState.Melee_Attack:
                animName = "Enemy attack 1";
                break;
            case AIState.Range_Attack:
                animName = "Enemy Scream";
                break;
            case AIState.Retreat:
                animName = "Retreat";
                break;
        }

        if (string.IsNullOrEmpty(animName)) return 0f;

        foreach(var clip in animator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == animName)
                return clip.length;
            //Debug.Log(clip.name);
        }

        return 0f;
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

    public void Attack()
    {
        

            bool playerHitted = false;

            Collider hits = Physics.OverlapSphere(hitbox.position, 1f, playerLayer).FirstOrDefault();

            if (hits != null && !playerHitted)
            {
                player.GetComponent<PlayerController>().TakeDamage(meleeDamage);
                //Debug.Log("player colpito melee");
                playerHitted = true;
            }
    }
        
    

}
