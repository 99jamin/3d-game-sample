using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Speed = Animator.StringToHash("Speed");

    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float jumpForce = 2;
    [SerializeField] private LayerMask groundLayer;     // 땅으로 인식할 레이어
    
    private CharacterController _characterController;
    private Animator _animator;
    
    private float _gravity = -9.81f;
    private Vector3 _velocity;
    private float _groundDistance;
    private float _groundedMinDistance = 0.1f;
    private float _speed = 0f;

    private bool IsGrounded
    {
        get
        {
            var distance = GetDistanceToGround();
            return distance < _groundedMinDistance;
        }
    }
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 커서 설정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 커서 락 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        HandleMovement();
        CheckRun();
        
        // 점프 높이 설정
        _animator.SetFloat("GroundDistance", GetDistanceToGround());
    }

    // 사용자 입력 처리 함수
    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0)
        {
            _animator.SetBool(Move, true);
        }
        else
        {
            _animator.SetBool(Move, false);
        }
        _animator.SetFloat(Speed, _speed);

        Vector3 movement = transform.forward * vertical;
        transform.Rotate(0, horizontal * rotateSpeed * Time.deltaTime, 0);
        
        // 점프
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * _gravity);
            _animator.SetTrigger("Jump2");
        }
        
    }
    
    // 달리기 처리
    private void CheckRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _speed += Time.deltaTime;
            _speed = Mathf.Clamp01(_speed);
        }
        else
        {
            _speed -= Time.deltaTime;
            _speed = Mathf.Clamp01(_speed);
        }
    }
    
    // 바닥과 거리를 계산하는 함수
    private float GetDistanceToGround()
    {
        float maxDistance = 10f;
        if (Physics.Raycast(transform.position, 
                Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }

    #region Animator Method

    private void OnAnimatorMove()
    {
        Vector3 movePosition;
        
        movePosition = _animator.deltaPosition;
        
        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        movePosition.y = _velocity.y * Time.deltaTime;
        
        _characterController.Move(movePosition);
    }

    #endregion

    #region Debug
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundDistance);
    }
    
    #endregion
}
