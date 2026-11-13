using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6.5f;
    public float jumpForce = 14f;
    public float dashSpeed = 25f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public int health = 1;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundlayer;
    public int extraJumpValue = 1;

    private int extraJumps;
    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    private int facingDirection = 1;
    private float defaultGravityScale;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        extraJumps = extraJumpValue;
        defaultGravityScale = rb.gravityScale;
    }
    void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        if (isGrounded() && rb.linearVelocityY <= 0.01f)
        {
            extraJumps = extraJumpValue;
        }
        Move();
        SetAnimation(moveInput);
    }

    private void ReadInput()
    {
        Keyboard kb = Keyboard.current;

        

        moveInput = 0f;
        if (kb.dKey.isPressed) moveInput = 1f;
        if (kb.aKey.isPressed) moveInput = -1f;

        if (kb.spaceKey.wasPressedThisFrame) Jump();
        if (kb.leftShiftKey.wasPressedThisFrame && canDash) StartCoroutine(Dash());
        if (moveInput != 0f) facingDirection = (int)moveInput;
    }

    private void Jump()
    {
        if (isDashing) return;

        if (isGrounded())
        { 
            rb.linearVelocityY = jumpForce;
        }
        else if (extraJumps > 0)
        {
            rb.linearVelocityY = jumpForce;
            extraJumps--;
        }
    }


    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundlayer);
    }

    private void Move()
    {
        if (isDashing) return;

        rb.linearVelocityX = moveInput * moveSpeed;
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded())
        {
            if (moveInput == 0f)
            {
                animator.Play("PlayerIdle");
            }
            else
            {
                animator.Play("PlayerRun");
            }
        }
        else
        {
            if(rb.linearVelocityY > 0)
            {
                animator.Play("PlayerJump");
            }
            else if(rb.linearVelocityY < 0)
            {
                animator.Play("PlayerFall");
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage")
        {
            health -= 1;

            if(health <= 0)
            {
                Die();
            }
        }
    }
    private void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        rb.gravityScale = 0;
        rb.linearVelocityX = facingDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.gravityScale = defaultGravityScale;
        rb.linearVelocityX *= 0.5f;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
}
