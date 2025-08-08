using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{

    private Input_Map inputActions;
    public int damage, magazine, bulletsLeft, bulletsShot;
    //public float shootForce;

    public Transform firePoint;
    public GameObject bulletPrefab;

    public float fireCooldown, spread;

    [SerializeField] private bool shooting, readyToShoot;

    public bool isAutomatic, loading;

    public RaycastHit raycastHit;

    public LayerMask enemy;

    private Animator animator;

    private void Awake()
    {
        inputActions = new Input_Map();
        animator = GetComponent<Animator>();

        // quando parte voglio che sia già in grado di sparare
        readyToShoot = true;
        inputActions.Player.Shoot.performed += PerformShot;
    
    }

    void Start()
    {

    }

    private void Update()
    {
    }

    public void PerformShot(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPoint;

        if (context.performed && readyToShoot)
        {
            //Debug.Log("SHOT! (" + context.action + ")");

            animator.SetTrigger("Shoot");

            // disegna un raggio lungo 100 unità per debug
            if (Physics.Raycast(firePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
            {
                Debug.DrawRay(firePoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 1f);
                //Debug.Log("Hai colpito: " + hit.collider.name);
            }
            else
            {
                Debug.Log("Nessun oggetto colpito");
            }

            if (Physics.Raycast(ray, out RaycastHit rayhit, 100f))
            {
                targetPoint = hit.point;

            } else { targetPoint = ray.GetPoint(100f); }

            Vector3 shootDir = (targetPoint - firePoint.position).normalized;    
            
            // istanzia il proiettile, la velocità e la forza sono giù inpostati nel prefab di esso
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            bullet.GetComponent<Bullet>().SetDirection(shootDir);

            FireCooldown();

        } else return;
        
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
