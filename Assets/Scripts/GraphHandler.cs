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
    [SerializeField] public Material lineMaterial;
    [SerializeField] private Material drawMaterial;
    [SerializeField] public LineRenderer xAxis;
    [SerializeField] public LineRenderer yAxis;
    [SerializeField] public GameObject origin;
    [SerializeField] public GameObject xArrow;
    [SerializeField] public GameObject yArrow;
    [SerializeField] public float xArrowOffset = 7f;
    [SerializeField] public float yArrowOffset = 5f;
    [SerializeField] private Sprite circleSprite;
    [SerializeField] public GameObject distributionPlotCanvasImage;
    [SerializeField] public GameObject simulationPlotCanvasImage;
    [SerializeField] private Canvas distributionPlotCanvasParent;

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
    [SerializeField] private RawImage imageDistribution;
    [SerializeField] private RawImage imageSimulation;
    
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
    private Texture2D _textureDistribution;
    private Texture2D _textureSimulation;
    
    // private float _probabilityPlaceholderValue = 0.5f;
    private static int _textureDistributionSize = 512;
    private static int _textureSimulationSize = 3136;
    private int _totalPlottedNumber;
    public bool isHistogramSelected;
    public bool isSimulationCanvasSelected;
    
    public void Awake()
    {
        _xAxisGameObj = xAxis.transform.gameObject;
        _yAxisGameObj = yAxis.transform.gameObject;
        plotSliderText.text = $"Points to simulate: {plotSlider.value}";
        
        distributionPlotCanvasParent.renderMode = RenderMode.WorldSpace;
    }
    
    
    #region Axis Animations
    
    public void AnimateXAxis(LineRenderer line, float start, float end, float yPos)
    {
        float currentExtent = start;
        line.SetPosition(0, new Vector3(currentExtent, yPos, 0));

        DOTween.To(() => currentExtent, val =>
        {
            currentExtent = val;
            line.SetPosition(1, new Vector3(currentExtent, yPos, 0));
        }, end, lineAnimationDuration).SetEase(Ease.Linear);
    }
    
    public void AnimateYAxis(LineRenderer line, float start, float end, float xPos)
    {
        float currentExtent = start;
        line.SetPosition(0, new Vector3(xPos, currentExtent, 0));

        DOTween.To(() => currentExtent, val =>
        {
            currentExtent = val;
            line.SetPosition(1, new Vector3(xPos, currentExtent, 0));
        }, end, lineAnimationDuration).SetEase(Ease.Linear);
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
        _textureDistribution = InitTexture(_textureDistribution, imageDistribution, _textureDistributionSize);
        _textureSimulation = InitTexture(_textureSimulation, imageSimulation, _textureSimulationSize);

        if (!isHistogramSelected && !isSimulationCanvasSelected)
        {
            imageDistribution.gameObject.SetActive(true);
        }
        
        StopAllCoroutines();
        CleanAxisPoints();

        AnimateXAxis(xAxis, -xAxisMaxExtent, xAxisMaxExtent, 0);
        AnimateYAxis(yAxis, -yAxisMaxExtent, yAxisMaxExtent, 0);
        StartCoroutine(AnimateXAxisPoints());
        StartCoroutine(AnimateYAxisPoints());
        
        histogram.Start();
    }

    private Texture2D InitTexture(Texture2D texture, RawImage image, int textureSize)
    {
        texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        
        Color[] clearPixels = new Color[textureSize * textureSize];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color(0, 0, 0, 0);
        
        texture.SetPixels(clearPixels);
        texture.Apply();
        
        image.texture = texture;

        return texture;
    }
    
    public void ResetGraph()
    {
        histogram.ResetHistogramBars();
        points.Clear();
        
        _totalPlottedNumber = 0;
        totalPlottedText.text = "Total points plotted: 0";
        
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

            int xDistribution = Mathf.RoundToInt(Mathf.InverseLerp(-5.25f, 5.25f, pos.x) * (_textureDistributionSize - 1));
            int yDistribution = Mathf.RoundToInt(Mathf.InverseLerp(-5.25f, 5.25f, pos.y) * (_textureDistributionSize - 1));
            _textureDistribution.SetPixel(xDistribution, yDistribution, Color.red);

            if (isSimulationCanvasSelected)
            {
                int xSimulation = Mathf.RoundToInt(Mathf.InverseLerp(-29f, 29f, pos.x) * (_textureSimulationSize - 1));
                int ySimulation = Mathf.RoundToInt(Mathf.InverseLerp(-29f, 29f, pos.y) * (_textureSimulationSize - 1));
                _textureSimulation.SetPixel(xSimulation, ySimulation, Color.red);
            }
            
            float rangeMin = -xAxisMaxExtent;
            float rangeMax = xAxisMaxExtent;
            float range = rangeMax - rangeMin;
            float binWidth = range / histogram.binSize;

            int index = Mathf.Clamp(Mathf.RoundToInt((pos.x - rangeMin) / binWidth), 0, histogram.binSize - 1);

            histogram.binFrequency[index]++;
            
            if (i % (pointsToPlot / 100) == 0)
            {
                _textureDistribution.Apply();
                _textureSimulation.Apply();
                totalPlottedText.text = $"Total points simulated: {_totalPlottedNumber + i}";
                
                histogram.UpdateHistogramBars(points);
                
                yield return new WaitForFixedUpdate();
            }
        }

        _textureDistribution.Apply();
        _textureSimulation.Apply();
        
        imageDistribution.texture = _textureDistribution;
        imageSimulation.texture = _textureSimulation;
        
        _totalPlottedNumber += pointsToPlot;
        totalPlottedText.text = $"Total points simulated: {_totalPlottedNumber}";
        
        histogram.UpdateHistogramBars(points);
    }

    public void UpdateSliderTextValue()
    {
        plotSliderText.text = $"Points to simulate: {plotSlider.value}";
    }

    public void ViewPlotsNormal()
    {
        if (isHistogramSelected)
        {
            distributionPlotCanvasImage.SetActive(true);
            simulationPlotCanvasImage.SetActive(false);
            histogram.gameObject.SetActive(false);

            var width = selectedButtonArrow.rectTransform.rect.width;
            var height = selectedButtonArrow.rectTransform.rect.height;
            selectedButtonArrow.rectTransform.sizeDelta = new Vector2(width, height + 357f);

            isHistogramSelected = false;
            isSimulationCanvasSelected = false;
        }
    }

    public void ViewPlotsSimulation()
    {
        simulationPlotCanvasImage.SetActive(true);
        distributionPlotCanvasImage.SetActive(false);
        histogram.gameObject.SetActive(false);
        
        isHistogramSelected = false;
        isSimulationCanvasSelected = true;
    }

    public void ViewHistogram()
    {
        if (!isHistogramSelected)
        {
            distributionPlotCanvasImage.SetActive(false);
            simulationPlotCanvasImage.SetActive(false);
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