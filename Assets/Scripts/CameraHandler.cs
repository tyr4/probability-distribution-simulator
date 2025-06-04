using System;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private RectTransform plotCanvas;
    [SerializeField] private float maxCameraDistance = 40f;
    [SerializeField] private float minCameraDistance = 1f;
    [SerializeField] private float maxSteps = 20f;

    private float _initialCameraSize;
    // private float _initialCanvasSize;
    
    private float _cameraStep;
    // private float _canvasStep;
    private void Start()
    {
        _initialCameraSize = camera.orthographicSize;
        // _initialCanvasSize = plotCanvas.rect.width;
        
        _cameraStep = _initialCameraSize / maxSteps;
        // _canvasStep = _initialCanvasSize / maxSteps;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (camera.orthographicSize > minCameraDistance)
            {
                camera.orthographicSize -= _cameraStep;
                // plotCanvas.sizeDelta = new Vector2(plotCanvas.sizeDelta.x + _canvasStep, plotCanvas.sizeDelta.y + _canvasStep);
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (camera.orthographicSize < maxCameraDistance)
            {
                camera.orthographicSize += _cameraStep;
                // plotCanvas.sizeDelta = new Vector2(plotCanvas.sizeDelta.x - _canvasStep, plotCanvas.sizeDelta.y - _canvasStep);
            }
        }
    }
}
