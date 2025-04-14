using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private HPBarController hpBarController;
    
    private Canvas _canvas;
    private AsyncOperationHandle<GameObject> _playerObjectHandle;
    
    private void Start()
    {
        // 커서 설정
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Cursor.visible = true;
        //     Cursor.lockState = CursorLockMode.None;
        // }
    }

    public void SetHP(float hp)
    {
        // hpBarController.SetHP(hp);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _canvas = FindCanvas();

        if (scene.name == "Main")
        {
            if (_playerObjectHandle.IsValid())
            {
                Addressables.ReleaseInstance(_playerObjectHandle);
            }
        }
        else
        {
            var spawnPoint = GameObject.Find("SpawnPoint");

            if (!_playerObjectHandle.IsValid())
            {
                _playerObjectHandle = Addressables.InstantiateAsync("Ellen",
                    spawnPoint.transform.position, spawnPoint.transform.rotation);
                _playerObjectHandle.Completed += handle =>
                {
                    DontDestroyOnLoad(handle.Result);
                };
            }
            else
            {
                var playerObject = _playerObjectHandle.Result;
                
                playerObject.transform.position = spawnPoint.transform.position;
                playerObject.transform.rotation = spawnPoint.transform.rotation;
                
                playerObject.SetActive(true);
            }
        }
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        _canvas = null;

        if (scene.name == "Main")
        {
            
        }
        else
        {
            if (_playerObjectHandle.IsValid())
            {
                var playerObject = _playerObjectHandle.Result;
                playerObject.SetActive(false);
            }
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    // 비동기로 씬 로드하는 코루틴 함수
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        var loadingPanelPrefab = Resources.Load<GameObject>("Common/LoadingPanel");
        var loadingPanelObject = Instantiate(loadingPanelPrefab, _canvas.transform);
        var loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingPanelController.SetLoadingBar(asyncOperation.progress);
            yield return null;
        }
        loadingPanelController.SetLoadingBar(1);
        asyncOperation.allowSceneActivation = true;

        Destroy(loadingPanelObject);
    }
    
    public Canvas FindCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("MainCanvas");
        Canvas canvas;

        if (canvasObject == null)
        {
            canvasObject = new GameObject("MainCanvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.tag = "MainCanvas";
        }
        else
        {
            canvas = canvasObject.GetComponent<Canvas>();
        }
        return canvas;
    }
    
}
