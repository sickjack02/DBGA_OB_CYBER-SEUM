using UnityEngine.Events;
using UnityEngine;
using System;

public class Gun : MonoBehaviour
{

    private Input_Map inputActions;
    // public UnityEvent OnGunShoot;
    public int damage, magazine, bulletsLeft, bulletsShot;

    public float fireCooldown, spread;

    public bool isAutomatic, shooting, readyToShoot, loading;

    private float CurrentCooldown;

    public RaycastHit raycastHit;

    public LayerMask enemy;

    private void Awake()
    {
        inputActions = new Input_Map();

        inputActions.Player.Shoot.started += ctx => StartShoot();
        inputActions.Player.Shoot.canceled += ctx => EndShoot();
    }

    private void Update()
    {
        if(shooting && readyToShoot) PerformShot();
    }

    private void StartShoot()
    {
        shooting = true;
    }

    private void EndShoot()
    {
        shooting = false;
    }

    private void PerformShot()
    {
        readyToShoot = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // disegna un raggio lungo 100 unità per debug
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);
        

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Hai colpito: " + hit.collider.name);
        }
        else
        {
            Debug.Log("Nessun oggetto colpito");
        }

        Invoke("FireCooldown", fireCooldown);
    }

    private void FireCooldown()
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
