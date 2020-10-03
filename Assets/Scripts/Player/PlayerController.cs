
﻿using System;
using UnityEngine;
using UnityEngine.Events;

using UnityEngine.Serialization;


[System.Serializable]

public class PlayerController : MonoBehaviour, HealthUpdatable
{
    //Related to Speed of Player Movement and Jumping
    public float maxSpeed = 12;
    public float smoothTime = 0.3f;
    public bool debug = true;
    public float initialJumpVelo = 10f;

    public float currHealth { get; set; }
    public float maxHealth = 100f;
    public GameObject healthBar;

    public UnityEvent<float> OnHealthUpdated {get; } = new UnityEvent<float>();


    public LayerMask groundMask;
    public Transform groundCheck;
    private Rigidbody2D rb2d;

    private Vector3 currentVel = Vector3.zero;

    private bool onGround = false;

    private GameObject currentGround;

    private Collider2D myCollider;
    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        currHealth = maxHealth;
    }
    private void FixedUpdate()
    {
       move();
       if (!onGround) checkGround();
    }

    //basic movement
    void move()
    {
        bool isJump = Input.GetKey(KeyCode.Space);
        float h = Input.GetAxisRaw("Horizontal");

        if (h != 0f)
        {
            Vector3 newVel = new Vector3(h * maxSpeed, rb2d.velocity.y);
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity,newVel,ref currentVel,smoothTime);
        }
        if (onGround && isJump)
        {
            onGround = false;
            rb2d.AddForce(new Vector2(0f, initialJumpVelo), ForceMode2D.Impulse);
        }
    }

    //when player jumped, check the bottom of player to check if the the radius we set hit with ground layer mask
   private void checkGround()
    {
        var collider = Physics2D.OverlapBox(groundCheck.position, new Vector2(transform.localScale.x, 0.001f), 0);
        if(rb2d.velocity.y <= 0 &&  collider != null && collider != myCollider)
        {
            onGround = true;
        }    
    }

    public void TakeDamage(float damage)
    {
        currHealth = (currHealth >= damage) ? currHealth - damage : 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //player takes damage
        var collObj = collision.gameObject;

        if(collObj.layer == LayerMask.NameToLayer("Enemy"))
        {
            float damage = collObj.GetComponent<BasicEnemy>().damage;
            
            TakeDamage(damage);
            OnHealthUpdated.Invoke(currHealth);
        }
    }
}