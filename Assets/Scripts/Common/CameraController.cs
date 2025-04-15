using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private LayerMask obstacleLayerMask;
    
    private Transform _target;
    
    private float _azimuthAngle;
    private float _polarAngle = 45f;
    
    private void LateUpdate()
    {
        if (!_target) return;
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        _azimuthAngle += mouseX * rotationSpeed * Time.deltaTime;
        _polarAngle -= mouseY * rotationSpeed * Time.deltaTime;
        _polarAngle = Mathf.Clamp(_polarAngle, 10f, 45f);
        
        // 벽감지 처리
        var currentDistance = AdjustCameraDistance();
        
        var cartesianPosition = GetCameraPosition(currentDistance, _polarAngle, _azimuthAngle);
        var cameraPosition = _target.position - cartesianPosition;
        
        transform.position = cameraPosition;
        transform.LookAt(_target);
    }

    Vector3 GetCameraPosition(float r, float polarAngle, float azimuthAngle)
    {
        float b = r * Mathf.Cos(polarAngle * Mathf.Deg2Rad);
        float z = b * Mathf.Cos(azimuthAngle * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(polarAngle * Mathf.Deg2Rad) * -1;
        float x = b * Mathf.Sin(azimuthAngle * Mathf.Deg2Rad);
        
        return new Vector3(x, y, z);
    }
    
    public void SetTarget(Transform target)
    {
        _target = target;
        
        var cartesianPosition = GetCameraPosition(distance, _polarAngle, _azimuthAngle);
        var cameraPosition = _target.position - cartesianPosition;
        
        transform.position = cameraPosition;
        transform.LookAt(_target);
    }
    
    // 카메라와 타겟 사이에 장애물이 있을 때 카메라와 타겟간의 거리를 조절하는 함수
    private float AdjustCameraDistance()
    {
        var currentDistance = distance;
        
        // 타겟에서 카메라 방향으로 레이케이스를 발사
        Vector3 direction = GetCameraPosition(1f, _polarAngle, _azimuthAngle).normalized;
        RaycastHit hit;

        // 타겟에서 카메라 예정 위치까지 레이케이스 발사
        if (Physics.Raycast(_target.position, -direction, out hit, 
                distance, obstacleLayerMask))
        {
            float offset = 0.3f;
            currentDistance = hit.distance - offset;
            currentDistance = Mathf.Max(currentDistance, 0.5f);
        }
        return currentDistance;
    }
}
