using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private float moveInput;
    public Player_Movement player_Movement;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    private string currentAnimationState;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        moveInput = player_Movement.inputMovement;
        if (moveInput > 0 || moveInput < 0)
        {
            ChangeAnimationState("walk");
        }
        else
        {
            ChangeAnimationState("idle");
        }
        if(moveInput < 0)
        {
            transform.localScale = new Vector2(-1,1);
        }
        else
        {
            transform.localScale = new Vector2(1,1);
        }
    }
    private void ChangeAnimationState(string newstate)
    {
        if(currentAnimationState == newstate)
            return;
        animator.Play(newstate);
        currentAnimationState = newstate;
    }
}
