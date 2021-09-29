using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    MasterInput inputs;
    Rigidbody2D rb;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private BoxCollider2D bxC2D;
    private float inputMovement;
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    private bool isGrounded;
    [Header("Wall Jump")]
    public float wallJumpForceX;
    public float wallJumpForceY;
    public float wallSlidingSpeed;
    private bool isWallSliding;
    private float wallJumpTime = 0.2f;
    private float jumpTime;

    private void Awake()
    {
        inputs = new MasterInput(); // init master input.
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // get object components.
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
            Jump();
        }
    }

    private void OnDisable()
    {
        inputs.Player.Jump.performed -= Jumped; 
        inputs.Disable();
    }
    private void Update()
    {
        isGrounded = GroundCheck();
    }
    private void FixedUpdate()
    {
        // Player movement.
        inputMovement = inputs.Player.Movement.ReadValue<float>();
        rb.velocity = new Vector2(inputMovement * moveSpeed, rb.velocity.y);
        // wall slide and jump
        if(WallCheck() && !isGrounded)
        {
            isWallSliding = true;
            jumpTime = Time.time + wallJumpTime;
        }
        else if(jumpTime < Time.time)
        {
            isWallSliding = false;
        }
        if (isWallSliding && !isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
    }
    private void Jump()
    {
        if (isGrounded || isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // jump
        }
        /*else if (isWallSliding && !isGrounded)
        {
            rb.velocity = new Vector2(-inputMovement * wallJumpForceX,wallJumpForceY);
        }*/
    }
    

    private bool GroundCheck()
    {
        RaycastHit2D boxhit = Physics2D.BoxCast(bxC2D.bounds.center, bxC2D.bounds.size, 0, Vector2.down,0.1f, groundLayer); // check if player is grounded
        return boxhit.collider != null;
    }
    private bool WallCheck()
    {
        RaycastHit2D hitLeft = Physics2D.BoxCast(bxC2D.bounds.center, bxC2D.bounds.size, 0, Vector2.left, 0.1f, wallLayer); // check if player is grounded
        RaycastHit2D hitRight = Physics2D.BoxCast(bxC2D.bounds.center, bxC2D.bounds.size, 0, Vector2.right, 0.1f, wallLayer);
        return hitLeft.collider != null || hitRight.collider != null;
    }
}
