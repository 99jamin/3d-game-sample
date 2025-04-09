using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyControllerOld))]
public class EnemyControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터를 그리기
        base.OnInspectorGUI();
        
        // 타겟 컴포넌트 참조 가져오기
        EnemyControllerOld enemyController = (EnemyControllerOld)target;
        
        // 여백 추가
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("상태 디버그 정보", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // 상태별 색상 지정
        switch (enemyController.CurrentState)
        {
            case EnemyState.Idle:
                GUI.backgroundColor = new Color(0, 0, 1, 1f);
                break;
            case EnemyState.Patrol:
                GUI.backgroundColor = new Color(0, 1, 0, 1f);
                break;
            case EnemyState.Trace:
                GUI.backgroundColor = new Color(1, 0, 1, 1f);
                break;
            case EnemyState.Attack:
                GUI.backgroundColor = new Color(1, 1, 0, 1f);
                break;
            case EnemyState.Hit:
                GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
                break;
            case EnemyState.Dead:
                GUI.backgroundColor = new Color(1, 0, 0, 1f);
                break;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("현재 상태", enemyController.CurrentState.ToString(),
            EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
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