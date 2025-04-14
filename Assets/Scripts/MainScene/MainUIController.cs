using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIController : MonoBehaviour
{
    public void OnClickStart()
    {
        GameManager.Instance.LoadSceneAsync("Outdoor");
    }
}
