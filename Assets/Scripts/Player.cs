using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    
    [Header("Player Movement")]
    public float jumpForce;
    public float moveSpeed;
    
    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public float groundCheckRadius;
    private bool _isGrounded = true;
    
    private bool _canDoubleJump = true;
    private float _movingInput;
    
    private bool _isFacingRight = true;
    private int _facingDirection = 1;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimationController();
        CollisionChecks();
        InputChecks();
        Move();
        FlipController();
    }

    private void AnimationController()
    {
        bool isMoving = _rb.velocity.x != 0;
        _animator.SetBool("isMoving", isMoving);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("yVelocity", _rb.velocity.y);
    }

    private void FlipController()
    {
        if (_isFacingRight && _movingInput < 0)
        {
            FlipCharacter();
        }
        else if (!_isFacingRight && _movingInput > 0)
        {
            FlipCharacter();
        }
    }

    private void FlipCharacter()
    {
        _facingDirection *= -1;
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void InputChecks()
    {
        _movingInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }

    private void JumpButton()
    {
        if (_isGrounded)
        {
            Jump();
        }
        else if (!_isGrounded && _canDoubleJump)
        {
            Jump();
            _canDoubleJump = false;
        }
        
        if (_isGrounded)
        {
            _canDoubleJump = true;
        }
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_movingInput * moveSpeed, _rb.velocity.y);
    }


    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }

    private void CollisionChecks()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, whatIsGround);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckRadius, transform.position.z));
    }
}
