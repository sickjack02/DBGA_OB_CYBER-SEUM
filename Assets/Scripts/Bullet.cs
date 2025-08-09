using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Tags che distruggono il proiettile")]
    public List<string> destroyOnTags;

    public float shootForce;
    private Vector3 moveDir;

    private float damage;

    //private float lifetime = 3f;

    void Start()
    {
        //Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += moveDir * shootForce * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnTags.Contains(collision.gameObject.tag))
        {
            //Debug.Log("Colpito " + collision.gameObject.name);
            
            if(collision.gameObject.name == "Enemy")
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.OnHit(damage);
                }
            }
            
            Destroy(gameObject);
        }           
    }

}
