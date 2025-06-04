using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GraphHandler : MonoBehaviour
{
    [Header("Graph variables")] 
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Material drawMaterial;
    [SerializeField] public LineRenderer xAxis;
    [SerializeField] public LineRenderer yAxis;
    [SerializeField] public GameObject origin;
    [SerializeField] public GameObject xArrow;
    [SerializeField] public GameObject yArrow;
    [SerializeField] public float xArrowOffset = 7f;
    [SerializeField] public float yArrowOffset = 5f;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private GameObject distributionCanvas;

    [Header("Animation settings")] 
    [SerializeField] public float xAxisMaxExtent = 28f;
    [SerializeField] public float yAxisMaxExtent = 20f;
    [SerializeField] private float lineAnimationDuration = 0.5f;

    [Header("Point settings")]
    [SerializeField] public float pointHeight = 0.5f;
    [SerializeField] public float pointWidth = 0.07f;
    [SerializeField] public float pointAnimationDuration = 0.02f;
    [SerializeField] public float pointStep = 2f;

    [Header("Text settings")] 
    [SerializeField] public float fontSize = 8f;
    [SerializeField] public float pointPosOffset = -1f;

    [Header("Formulas")] 
    [SerializeField] public Formulas formulas;
    [SerializeField] private RawImage image;
    
    [Header("Plot slider")]
    [SerializeField] private Slider plotSlider;
    [SerializeField] private TextMeshProUGUI plotSliderText;
    [SerializeField] private TextMeshProUGUI totalPlottedText;
    [SerializeField] private Slider timescaleSlider;
    [SerializeField] private TextMeshProUGUI timescaleText;

    [Header("Histogram")] 
    [SerializeField] private HistogramHandler histogram;
    [SerializeField] private TextMeshProUGUI selectedButtonArrow;

    public delegate Vector2 DistributionFunction();
    public DistributionFunction CurrentDistribution;

    private GameObject _xAxisGameObj;
    private GameObject _yAxisGameObj;
    
    public List<Vector2> points = new List<Vector2>();
    private Texture2D _texture;
    
    // private float _probabilityPlaceholderValue = 0.5f;
    private static int _textureSize = 512;
    private int _totalPlottedNumber;
    public bool isHistogramSelected;
    
    public void Awake()
    {
        _xAxisGameObj = xAxis.transform.gameObject;
        _yAxisGameObj = yAxis.transform.gameObject;
        plotSliderText.text = $"Points to simulate: {plotSlider.value}";
    }
    
    #region Axis Animations

    // TODO: add a param to this to accept a max size instead of the serialized field
    private void AnimateXAxis()
    {
        float currentExtent = -xAxisMaxExtent;
        xAxis.SetPosition(0, new Vector3(currentExtent, 0, 0));

        DOTween.To(() => currentExtent, val =>
        {
            currentExtent = val;
            xAxis.SetPosition(1, new Vector3(currentExtent, 0, 0));
        }, xAxisMaxExtent, lineAnimationDuration).SetEase(Ease.Linear);
    }

    // TODO: add a param to this to accept a max size instead of the serialized field
    private void AnimateYAxis()
    {
        float currentExtent = -yAxisMaxExtent;
        yAxis.SetPosition(0, new Vector3(0, currentExtent, 0));

        DOTween.To(() => currentExtent, val =>
        {
            currentExtent = val;
            yAxis.SetPosition(1, new Vector3(0, currentExtent, 0));
        }, yAxisMaxExtent, lineAnimationDuration).SetEase(Ease.Linear);
    }

    private IEnumerator AnimateXAxisPoints()
    {
        for (float i = -xAxisMaxExtent + pointStep; i < xAxisMaxExtent; i += pointStep)
        {
            if (i == 0)
            {
                origin.SetActive(true);
                continue;
            }

            float tempExtent = 0f;
            float pointXPos = i;

            // line renderer component on the graph
            var lineObj = new GameObject($"{pointXPos} x");
            lineObj.transform.SetParent(_xAxisGameObj.gameObject.transform);
            lineObj.transform.localPosition = Vector3.zero;

            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = pointWidth;
            lineRenderer.endWidth = pointWidth;

            // text component under the line renderer
            var textObj = new GameObject($"text {pointXPos} x");
            var tmp = textObj.AddComponent<TextMeshPro>();

            tmp.transform.SetParent(lineObj.transform);
            tmp.transform.localPosition = new Vector3(pointXPos, pointPosOffset, 0);

            tmp.text = $"{pointXPos}";
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            // animate the point line bar
            DOTween.To(() => tempExtent, val =>
            {
                tempExtent = val;
                lineRenderer.SetPosition(0, new Vector3(pointXPos, -tempExtent, 0));
                lineRenderer.SetPosition(1, new Vector3(pointXPos, tempExtent, 0));
            }, pointHeight, lineAnimationDuration / 5f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(pointAnimationDuration);
        }

        xArrow.SetActive(true);
        xArrow.transform.localPosition = new Vector3(xAxisMaxExtent - xArrowOffset, 0, 0);
    }

    private IEnumerator AnimateYAxisPoints()
    {
        for (float i = -yAxisMaxExtent + pointStep; i < yAxisMaxExtent; i += pointStep)
        {
            if (i == 0) continue;

            float tempExtent = 0f;
            float pointYPos = i;

            // line renderer component on the graph
            var lineObj = new GameObject($"{i} y");
            lineObj.transform.SetParent(_yAxisGameObj.gameObject.transform);
            lineObj.transform.localPosition = Vector3.zero;

            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = pointWidth;
            lineRenderer.endWidth = pointWidth;

            // text component under the line renderer
            var textObj = new GameObject($"text {pointYPos} x");
            var tmp = textObj.AddComponent<TextMeshPro>();

            tmp.transform.SetParent(lineObj.transform);
            tmp.transform.localPosition = new Vector3(pointPosOffset, pointYPos, 0);

            tmp.text = $"{pointYPos}";
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            // animate the point line bar
            DOTween.To(() => tempExtent, val =>
            {
                tempExtent = val;
                lineRenderer.SetPosition(0, new Vector3(-tempExtent, pointYPos, 0));
                lineRenderer.SetPosition(1, new Vector3(tempExtent, pointYPos, 0));
            }, pointHeight, lineAnimationDuration / 5f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(pointAnimationDuration);
        }

        yArrow.SetActive(true);
        yArrow.transform.localPosition = new Vector3(0, yAxisMaxExtent - yArrowOffset, 0);
    }


    #endregion
    
    private void CleanAxisPoints()
    {
        origin.SetActive(false);
        yArrow.SetActive(false);
        xArrow.SetActive(false);

        foreach (Transform child in _xAxisGameObj.GetComponentsInChildren<Transform>())
        {
            if (child != _xAxisGameObj.transform)
                Destroy(child.gameObject);
        }

        foreach (Transform child in _yAxisGameObj.GetComponentsInChildren<Transform>())
        {
            if (child != _yAxisGameObj.transform)
                Destroy(child.gameObject);
        }
    }

    private void InitGraph()
    {
        _texture = new Texture2D(_textureSize, _textureSize, TextureFormat.RGBA32, false);
        
        Color[] clearPixels = new Color[_textureSize * _textureSize];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color(0, 0, 0, 0);
        
        _texture.SetPixels(clearPixels);
        _texture.Apply();
        image.texture = _texture;
        if (!isHistogramSelected)
            image.gameObject.SetActive(true);

        StopAllCoroutines();
        CleanAxisPoints();

        AnimateXAxis();
        AnimateYAxis();
        StartCoroutine(AnimateXAxisPoints());
        StartCoroutine(AnimateYAxisPoints());

        // Callback distributionFunctionCallback = DistributionFunction;
        histogram.Start();
    }
    
    public void ResetGraph()
    {
        histogram.ResetHistogramBars();
        points.Clear();
        
        _totalPlottedNumber = 0;
        totalPlottedText.text = "Total points plotted: 0";
        
        // formulas.UpdateProbability(_probabilityPlaceholderValue);
        
        InitGraph();
    }
    
    public void PlotPointFromButton()
    {
        StartCoroutine(PlotAndDrawCoroutine());
    }

    private IEnumerator PlotAndDrawCoroutine()
    {
        int pointsToPlot = (int)plotSlider.value;
        
        for (int i = 1; i <= pointsToPlot; i++)
        {
            // assumes (0, 0) if the function doesnt exist
            Vector2 pos = CurrentDistribution?.Invoke() ?? Vector2.zero;
            
            points.Add(pos);

            int x = Mathf.RoundToInt(Mathf.InverseLerp(-5.25f, 5.25f, pos.x) * (_textureSize - 1));
            int y = Mathf.RoundToInt(Mathf.InverseLerp(-5.25f, 5.25f, pos.y) * (_textureSize - 1));
            _texture.SetPixel(x, y, Color.red);
            
            float rangeMin = -xAxisMaxExtent;
            float rangeMax = xAxisMaxExtent;
            float range = rangeMax - rangeMin;
            float binWidth = range / histogram.binSize;

            int index = Mathf.Clamp(Mathf.RoundToInt((pos.x - rangeMin) / binWidth), 0, histogram.binSize - 1);

            histogram.binFrequency[index]++;
            
            if (i % (pointsToPlot / 100) == 0)
            {
                _texture.Apply();
                totalPlottedText.text = $"Total points simulated: {_totalPlottedNumber + i}";
                
                histogram.UpdateHistogramBars(points);
                
                yield return new WaitForFixedUpdate();
            }
        }

        _texture.Apply();
        image.texture = _texture;
        
        _totalPlottedNumber += pointsToPlot;
        totalPlottedText.text = $"Total points simulated: {_totalPlottedNumber}";
        
        histogram.UpdateHistogramBars(points);
    }

    public void UpdateSliderTextValue()
    {
        plotSliderText.text = $"Points to simulate: {plotSlider.value}";
    }

    public void ViewPlots()
    {
        if (isHistogramSelected)
        {
            distributionCanvas.SetActive(true);
            histogram.gameObject.SetActive(false);

            var width = selectedButtonArrow.rectTransform.rect.width;
            var height = selectedButtonArrow.rectTransform.rect.height;
            selectedButtonArrow.rectTransform.sizeDelta = new Vector2(width, height + 357f);

            isHistogramSelected = false;
        }
    }

    public void ViewHistogram()
    {
        if (!isHistogramSelected)
        {
            distributionCanvas.SetActive(false);
            histogram.gameObject.SetActive(true);
            
            var width = selectedButtonArrow.rectTransform.rect.width;
            var height = selectedButtonArrow.rectTransform.rect.height;
            selectedButtonArrow.rectTransform.sizeDelta = new Vector2(width, height - 357f);
            
            isHistogramSelected = true;
        }
    }

    public void SetTimescale()
    {
        float value = 1.0f * timescaleSlider.value / 100;
        
        Time.timeScale = value;
        timescaleText.text = $"Timescale: x{value}";
    }
}