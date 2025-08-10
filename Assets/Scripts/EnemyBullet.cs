using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
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

            if (collision.gameObject.name == "Player")
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (player != null)
                    player.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}
