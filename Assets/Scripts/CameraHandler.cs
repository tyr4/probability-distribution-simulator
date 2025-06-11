using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private new Camera camera;
    [SerializeField] private RectTransform plotCanvas;
    [SerializeField] private float maxCameraDistance = 40f;
    [SerializeField] private float minCameraDistance = 1f;
    [SerializeField] private float maxSteps = 20f;

    private float _initialCameraSize;
    private float _cameraStep;
    private Vector3 _initialCameraPosition;
    private bool _isDragging;
    private Vector3 _dragStartPosition;
    
    private void Start()
    {
        _initialCameraSize = camera.orthographicSize;
        _initialCameraPosition = camera.transform.position;
        _cameraStep = _initialCameraSize / maxSteps;
    }

    private void Update()
    {
        HandleZoom();
        HandleDrag();
    }
    
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f && camera.orthographicSize > minCameraDistance)
        {
            camera.orthographicSize -= _cameraStep;
        }
        else if (scroll < 0f && camera.orthographicSize < maxCameraDistance)
        {
            camera.orthographicSize += _cameraStep;
        }
    }
    
    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            _isDragging = true;
            _dragStartPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if (_isDragging)
        {
            Vector3 currentMouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = _dragStartPosition - currentMouseWorldPos;

            camera.transform.position += difference;
        }
    }

    public void ResetCamera()
    {
        camera.orthographicSize = _initialCameraSize;
        camera.transform.position = _initialCameraPosition;
    }
    
    private bool IsPointerOverUI()
    {
        return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    }
}
