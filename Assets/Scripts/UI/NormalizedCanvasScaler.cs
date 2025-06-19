using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalizedCanvasScaler : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler _canvasScaler;

    // Start is called before the first frame update
    void Start()
    {
        if(CameraManager.Instance.IsVerticalScale)
        {
            _canvasScaler.matchWidthOrHeight = 0.2f;
        }
        else
        {
            _canvasScaler.matchWidthOrHeight = 1f;
        }
    }

}
