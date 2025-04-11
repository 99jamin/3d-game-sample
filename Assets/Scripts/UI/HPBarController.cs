using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Image _hpGauge;

    private void Update()
    {
        var cameraTransform = Camera.main.transform;
        transform.rotation = cameraTransform.rotation;
    }

    public void SetHP(float hp)
    {
        _hpGauge.fillAmount = hp;
    }
}
