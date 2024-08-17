using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    
    [Header("Player Movement")]
    public float jumpForce;
    public float moveSpeed;
    public Vector2 wallJumpForce;
    private bool _canMove = true;
    
    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public float groundCheckRadius;
    private bool _isGrounded = true;
    public float wallCheckRadius;
    private bool _isTouchingWall;
    private bool _canWallSlide;
    private bool _isWallSliding;
    
    
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
        FlipController();
        
        if (_isGrounded)
        {
            _canMove = true;
            _canDoubleJump = true;
        }

        if (_canWallSlide)
        {
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.1f);
        }
        
        Move();
    }

    private void AnimationController()
    {
        bool isMoving = _rb.velocity.x != 0;
        _animator.SetBool("isMoving", _movingInput != 0);
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetFloat("yVelocity", _movingInput);
        _animator.SetBool("isWallSliding", _isWallSliding);
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
        _movingInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetAxis("Vertical") < 0) _canWallSlide = false;
        if (Input.GetKeyDown(KeyCode.Space)) JumpButton();
    }

    private void JumpButton()
    {
        if (_isWallSliding)
        {
            WallJump();
        }
        else if (_isGrounded)
        {
            Jump();
        }
        else if (!_isGrounded && _canDoubleJump)
        {
            Jump();
            _canDoubleJump = false;
        }
        
        _canWallSlide = false;
    }

    private void Move()
    {
        if (_canMove)
            _rb.velocity = new Vector2(_movingInput * moveSpeed, _rb.velocity.y);
    }


    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
    }

    private void WallJump()
    {
        _canMove = false;
        _rb.velocity = new Vector2(wallJumpForce.x * _facingDirection * -1, wallJumpForce.y);
        FlipCharacter();
    }

    private void CollisionChecks()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius, whatIsGround);
        _isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * _facingDirection, wallCheckRadius, whatIsGround);

        if (_isTouchingWall && _rb.velocity.y < 0)
        {
            _canWallSlide = true;
        }

        if (!_isTouchingWall)
        {
            _canWallSlide = false;
            _isWallSliding = false;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckRadius, transform.position.z));
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckRadius * _facingDirection, transform.position.y));
    }
}
