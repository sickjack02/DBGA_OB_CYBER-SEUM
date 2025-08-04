using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public int Healt, currentHealt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealt == 0) { Enemy.Destroy(gameObject); }
    }

    private void OnHit()
    {

    }
}
