using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{

    private Input_Map inputActions;
    // public UnityEvent OnGunShoot;
    public int damage, magazine, bulletsLeft, bulletsShot;

    public float fireCooldown, spread;

    [SerializeField] private bool shooting, readyToShoot;

    public bool isAutomatic, loading;

    public RaycastHit raycastHit;

    public LayerMask enemy;

    private Animator animator;

    private void Awake()
    {
        inputActions = new Input_Map();

        // quando parte voglio che sia già in grado di sparare
        readyToShoot = true;

        inputActions.Player.Shoot.performed += PerformShot;

        /*inputActions.Player.Shoot.started += ctx => StartShoot();
        inputActions.Player.Shoot.canceled += ctx => EndShoot();*/
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //if(shooting && readyToShoot) PerformShot();

    }

    private void StartShoot()
    {
        shooting = true;
    }

    private void EndShoot()
    {
        shooting = false;
    }

    public void PerformShot(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // disegna un raggio lungo 100 unità per debug
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);

        if (context.performed && readyToShoot)
        {
            //shooting = true;
            Debug.Log("SHOT! (" + context.action + ")");

            animator.SetBool("isShooting", true);

            //Shoot();

            if (Physics.Raycast(ray, out hit, 100f)) Debug.Log("Hai colpito: " + hit.collider.name);
            else Debug.Log("Nessun oggetto colpito");

            FireCooldown();

        } else return;
        
        
        //shooting = false; 
        
    }

    private void Shoot()
    {
        throw new NotImplementedException();
    }

    private void FireCooldown()
    {
        readyToShoot = false;
        Invoke(nameof(CooldownTimer), fireCooldown);
    }

    private void CooldownTimer()
    {
        readyToShoot = true;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
