using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement = 0f;

    [Header("Jump")]
    public float jumpPower = 10f;
    public bool isGrounded = true;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 1;
    public float maxFallspeed = 18;
    public float fallSpeedMultiplier = 2;

    [Header("Death Barrier")]
    public LayerMask deathLayer;
    public bool isInDeathBarrier = false;
    public int deathCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
        isGrounded = IsGrounded();
        isInDeathBarrier = IsDeathBarrier();
        Gravity();
        isDead();
    }

    public void Gravity()
    {
        if(rb.linearVelocityY < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; // increasinly faster
            rb.linearVelocity = new Vector2(rb.linearVelocityX, MathF.Max(rb.linearVelocityY, -maxFallspeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private bool IsGrounded()
    {
        Vector2 checkPos = new Vector2(groundCheckPos.position.x, groundCheckPos.position.y);

        if (Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, groundLayer))
        {
            return true;
        }

        return false;
    }
    private bool IsDeathBarrier()
    {
        Vector2 checkPos = new Vector2(groundCheckPos.position.x, groundCheckPos.position.y);

        if (Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, deathLayer))
        {
            return true;
        }

        return false;
    }

    public void Die() // Die and add one to deathcount, Sets you back to 0,0 , Though this can be updated to "Checkpoint"
    {
            Vector2 SpawnPoint = new Vector2(0, 0);
            rb.transform.position = SpawnPoint;
            deathCount++;
            Debug.Log("Death: " + deathCount);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpPower);
        }
        if (context.canceled && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Convert.ToInt32(rb.linearVelocityY * 0.5));
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (groundCheckPos != null)
            Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    private void isDead()
    {
        if (isInDeathBarrier)
        {
            Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Trap trap = collision.GetComponent<Trap>();
        if (trap)
        {
            Die();
        }
    }

}
