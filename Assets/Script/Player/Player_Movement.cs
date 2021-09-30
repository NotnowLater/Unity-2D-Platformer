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
    public float inputMovement { get; private set; }
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    private bool isGrounded;
    private bool isFacingRight;
    [Header("Wall Jump")]
    public float wallJumpForceX;
    public float wallJumpForceY;
    public float wallSlidingSpeed;
    private bool isWallSliding;
    private float wallJumpTime = 0.2f;
    private float jumpTime;
    [Header("Dash")]
    public float dashSpeed;
    public float dashLenght;
    public float dashCooldown;
    private float dashTimer;
    private bool isDashing;
    private bool canDash;
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
        inputs.Player.Dash.performed += Dashed;
        inputs.Enable(); // enable input
    }

    

    private void OnDisable()
    {
        inputs.Player.Jump.performed -= Jumped; 
        inputs.Player.Dash.performed -= Dashed;
        inputs.Disable();
    }
    private void Jumped(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Jump();
        }
    }
    private void Dashed(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (Time.time > dashTimer)
            {
                dashTimer = Time.time + dashCooldown;
                StartCoroutine(Dash(inputMovement, 0));
                Debug.Log("dashed");
            }
        }
    }

    private void Update()
    {
        isGrounded = GroundCheck();
    }
    private void FixedUpdate()
    {
        // Player movement.
        if (!isDashing)
        {
            inputMovement = inputs.Player.Movement.ReadValue<float>();
            rb.velocity = new Vector2(inputMovement * moveSpeed, rb.velocity.y);
            // wall slide and jump
            if (WallCheck() && !isGrounded)
            {
                isWallSliding = true;
                jumpTime = Time.time + wallJumpTime;
            }
            else if (jumpTime < Time.time)
            {
                isWallSliding = false;
            }
            if (isWallSliding && !isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
        }
        if (inputMovement < 0) isFacingRight = false;
        else isFacingRight = true;
        
    }
    private void Jump()
    {
        if (isGrounded || isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // jump
        }
    }
    IEnumerator Dash(float x,float y)
    {
        float dashStarttime = Time.time;
        isDashing = true;
        rb.velocity = Vector2.zero;

        Vector2 direction;
        if (x != 0 || y != 0) direction = new Vector2(x, y);
        else
        {
            if (isFacingRight) direction = new Vector2(1,0);
            else direction = new Vector2(-1,0);
        }
        while(Time.time < dashStarttime + dashLenght)
        {
            rb.velocity = direction.normalized * dashSpeed;
            yield return null;
        }
        isDashing = false;
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
