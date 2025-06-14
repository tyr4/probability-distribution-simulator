using UnityEngine;
using UnityEngine.UI;

public class CanvasResizeWithScreen : MonoBehaviour
{
    [SerializeField] private CanvasScaler plotCanvasScaler;
    [SerializeField] private RectTransform normalPlotCanvas;
    [SerializeField] private RectTransform rectPlotCanvas;
    [SerializeField] private GraphHandler graph;
    
    public float resizeNormalValue;
    public float resizeRectValue;

    private float _screenSizeX;
    private float _screenSizeY;
    
    public Vector2 InitCanvasSize()
    {
        resizeRectValue = plotCanvasScaler.referenceResolution.x / rectPlotCanvas.rect.width;
        resizeNormalValue = plotCanvasScaler.referenceResolution.x / normalPlotCanvas.rect.width;
        
        _screenSizeX = Screen.width;
        _screenSizeY = Screen.height;
        
        float normalNewSize = _screenSizeX / resizeNormalValue;
        float rectNewSize = _screenSizeX / resizeRectValue;
        
        plotCanvasScaler.referenceResolution = new Vector2(_screenSizeX, _screenSizeY);
        
        normalPlotCanvas.sizeDelta = new Vector2(normalNewSize, normalNewSize);
        rectPlotCanvas.sizeDelta = new Vector2(rectNewSize, rectNewSize);
        
        return new Vector2(normalNewSize, rectNewSize);
    }
}
