using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터를 그리기
        base.OnInspectorGUI();
        
        // 타겟 컴포넌트 참조 가져오기
        PlayerController playerController = (PlayerController)target;
        
        // 여백 추가
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("상태 디버그 정보", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // 상태별 색상 지정
        switch (playerController.CurrentState)
        {
            case PlayerState.None:
                GUI.backgroundColor = new Color(1, 1, 1, 1f);
                break;
            case PlayerState.Idle:
                GUI.backgroundColor = new Color(0, 0, 1, 1f);
                break;
            case PlayerState.Move:
                GUI.backgroundColor = new Color(0, 1, 0, 1f);
                break;
            case PlayerState.Jump:
                GUI.backgroundColor = new Color(1, 0, 1, 1f);
                break;
            case PlayerState.Attack:
                GUI.backgroundColor = new Color(1, 1, 0, 1f);
                break;
            case PlayerState.Hit:
                GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
                break;
            case PlayerState.Dead:
                GUI.backgroundColor = new Color(1, 0, 0, 1f);
                break;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", playerController.CurrentState.ToString(),
            EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
        
        // 지면 접촉 상태
        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("캐릭터 위치 디버그 정보", EditorStyles.boldLabel);
        GUI.enabled = false;
        EditorGUILayout.Toggle("지면 접촉", playerController.IsGrounded);
        GUI.enabled = true;
        
        // 강제로 상태 변경 버튼
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Idle"))
            playerController.SetState(PlayerState.Idle);
        if (GUILayout.Button("Move"))
            playerController.SetState(PlayerState.Move);
        if (GUILayout.Button("Jump"))
            playerController.SetState(PlayerState.Jump);
        if (GUILayout.Button("Attack"))
            playerController.SetState(PlayerState.Attack);
        if (GUILayout.Button("Hit"))
            playerController.SetState(PlayerState.Hit);
        if (GUILayout.Button("Dead"))
            playerController.SetState(PlayerState.Dead);
        
        EditorGUILayout.EndHorizontal();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (target != null)
            Repaint();
    }
}
