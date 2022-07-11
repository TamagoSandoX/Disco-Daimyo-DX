using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float bulletSpeed = 3f;


    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private GameObject target;
    private float destroyTime = 7f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player");
        moveDirection = (target.transform.position - transform.position).normalized * bulletSpeed;
        rb.velocity = new Vector2 (moveDirection.x, moveDirection.y);
        Destroy (gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
