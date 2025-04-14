using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private HPBarController hpBarController;
    
    private void Start()
    {
        // 커서 설정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetHP(float hp)
    {
        hpBarController.SetHP(hp);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }
}
