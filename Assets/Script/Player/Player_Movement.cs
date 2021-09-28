using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    MasterInput inputs;
    Rigidbody2D rb2d;
    private BoxCollider2D bxC2D;
    [SerializeField]private LayerMask groundLayer;
    public float Speed;
    private void Awake()
    {
        inputs = new MasterInput(); // init master input.
    }
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); // get object components.
        bxC2D = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        inputs.Player.Jump.performed += Jumped; // subscribe to methods.
        inputs.Enable(); // enable input
    }

    private void Jumped(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (isGrounded())
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, Speed); // jump
            }
        }
    }

    private void OnDisable()
    {
        inputs.Player.Jump.performed -= Jumped; 
        inputs.Disable();
    }
    private void FixedUpdate()
    {
        rb2d.velocity = new Vector2(inputs.Player.Movement.ReadValue<float>() * Speed, rb2d.velocity.y); // player movement
    }
    private bool isGrounded()
    {
        RaycastHit2D boxhit = Physics2D.BoxCast(bxC2D.bounds.center, bxC2D.bounds.size, 0, Vector2.down,0.1f, groundLayer); // check if player is grounded
        return boxhit.collider != null;
    }
}
