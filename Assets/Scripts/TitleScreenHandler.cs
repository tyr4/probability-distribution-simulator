using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Serialization;

public class TitleScreenHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private Transform parentCanvas;
    [SerializeField] private RectTransform button;
    [SerializeField] private float animationDelay = 2f;
    [SerializeField] private float animationDuration = 15f;

    private float _screenX;
    private float _screenY;
    private Vector2 _spawnPos;
    private void Start()
    {
        _screenX = parentCanvas.GetComponent<RectTransform>().rect.width * parentCanvas.localScale.x;
        _screenY = parentCanvas.GetComponent<RectTransform>().rect.height * parentCanvas.localScale.y;
        
        StartCoroutine(BeginAnimation());
    }
    
    public void LoadSimulationScene()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("Simulation");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator BeginAnimation()
    {
        while (true)
        {
            GameObject instance = Instantiate(prefabs[Random.Range(0, prefabs.Count)], parentCanvas);
            RectTransform rect = instance.GetComponent<RectTransform>();
            float randomRotation = Random.Range(-20f, 20f);
            
            _spawnPos = new Vector2(GetRandomXPosition(rect), 0 - _screenY - rect.rect.height);
            rect.anchoredPosition = _spawnPos;
            rect.rotation = Quaternion.Euler(0, 0, randomRotation);
            
            AnimateTextUp(rect, _spawnPos.y, _screenY, animationDuration, randomRotation);
            yield return new WaitForSeconds(animationDelay);
        }
    }

    private void AnimateTextUp(RectTransform obj, float start, float end, float duration, float rotation)
    {
        var seq = DOTween.Sequence();
        var randomDuration = Random.Range(3, 10);
        
        seq.Append(
            DOTween.To(() => start, val =>
                {
                    start = val;
                    obj.anchoredPosition = new Vector2(obj.anchoredPosition.x, start);
                }, end, randomDuration)
                .SetEase(Ease.Linear)
        );
        
        seq.Join(
            obj.DORotate(new Vector3(0, 0, -rotation), randomDuration, RotateMode.WorldAxisAdd)
        );

        seq.OnComplete(() =>
        {
            Destroy(obj.gameObject);
        });
    }

    private float GetRandomXPosition(RectTransform rect)
    {
        // Get the canvas dimensions from the RectTransform's parent canvas
        Canvas mainCanvas = rect.GetComponentInParent<Canvas>();
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        float canvasHalfWidth = canvasRect.rect.width / 2f;
    
        float spawnHalfWidth = rect.rect.width / 2f;
        float buttonHalfWidth = button.rect.width / 2f;
        float buttonCenterX = button.anchoredPosition.x; // Button's center position

        // Calculate safe spawn boundaries (ensure full visibility)
        float canvasMinX = -canvasHalfWidth + spawnHalfWidth;
        float canvasMaxX = canvasHalfWidth - spawnHalfWidth;

        // Calculate exclusion zone relative to button's position
        float excludeMinX = buttonCenterX - buttonHalfWidth - spawnHalfWidth;
        float excludeMaxX = buttonCenterX + buttonHalfWidth + spawnHalfWidth;

        float x;
        int attempts = 0;
        const int maxAttempts = 50; // Prevent infinite loops
    
        // Generate positions until we find a valid one
        do
        {
            x = Random.Range(canvasMinX, canvasMaxX);
            attempts++;
        } 
        while (attempts < maxAttempts && 
               x >= excludeMinX && 
               x <= excludeMaxX);

        // Fallback if too many attempts
        if (attempts >= maxAttempts)
        {
            // Force position outside exclusion zone
            x = (x < buttonCenterX) 
                ? excludeMinX - spawnHalfWidth 
                : excludeMaxX + spawnHalfWidth;
            
            // Clamp to canvas boundaries
            x = Mathf.Clamp(x, canvasMinX, canvasMaxX);
        }

        return x;
    }
}
