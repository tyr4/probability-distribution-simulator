using UnityEngine;
using UnityEngine.UI;

public class CanvasResizeWithScreen : MonoBehaviour
{
    [SerializeField] private CanvasScaler plotCanvasScaler;
    [SerializeField] private RectTransform normalPlotCanvas;
    [SerializeField] private RectTransform rectPlotCanvas;

    private float _resizeNormalValue;
    private float _resizeRectValue;

    private float _screenSizeX;
    private float _screenSizeY;
    
    private void Start()
    {
        _resizeRectValue = plotCanvasScaler.referenceResolution.x / rectPlotCanvas.rect.width;
        _resizeNormalValue = plotCanvasScaler.referenceResolution.x / normalPlotCanvas.rect.width;
        
        _screenSizeX = Screen.width;
        _screenSizeY = Screen.height;
        
        float normalNewSize = _screenSizeX / _resizeNormalValue;
        float rectNewSize = _screenSizeX / _resizeRectValue;
        
        plotCanvasScaler.referenceResolution = new Vector2(_screenSizeX, _screenSizeY);
        
        normalPlotCanvas.sizeDelta = new Vector2(normalNewSize, normalNewSize);
        rectPlotCanvas.sizeDelta = new Vector2(rectNewSize, rectNewSize);
    }
}
