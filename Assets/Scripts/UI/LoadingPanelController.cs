using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private Image loadingBar;

    private void Start()
    {
        loadingBar.fillAmount = 0;
    }

    public void SetLoadingBar(float value)
    {
        loadingBar.fillAmount = value;
    }
}
