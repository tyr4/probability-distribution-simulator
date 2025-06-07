using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    [Header("General variables")]
    [SerializeField] private GraphHandler graphHandler;
    [SerializeField] private HistogramHandler histogramHandler;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject menuSideButtons;
    [SerializeField] private new Camera camera;
    [SerializeField] private GameObject histogramGraphCanvas;
    [SerializeField] private GameObject xArrow;
    [SerializeField] private GameObject yArrow;
    [SerializeField] private TextMeshPro origin;
    
    [Header("Animation variables")]
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private CameraHandler cameraScroller;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private CircleDrawer circleDrawer;

    [Header("Other canvas")] 
    [SerializeField] private GameObject graphCanvas;
    [SerializeField] private GameObject distributionCanvas;

    [Header("Normal Distribution")]
    [SerializeField] private float xMaxNormal = 28f;
    [SerializeField] private float yMaxNormal = 20f;
    [SerializeField] public float xArrowOffsetNormal = 7f;
    [SerializeField] public float yArrowOffsetNormal = 5f;
    [SerializeField] public float cameraOffsetNormal = 5f;
    [SerializeField] public float originSizeNormal = 15f;
    [SerializeField] private float pointHeightNormal = 0.5f;
    [SerializeField] private float pointStepNormal = 2f;
    [SerializeField] private float pointPosOffsetNormal = -1f;
    [SerializeField] private float pointAnimationDurationNormal = 0.02f;
    [SerializeField] private float fontSizeNormal = 8f;
    [SerializeField] private float axisWidthNormal = 0.25f;
    [SerializeField] private int histogramBinsNormal = 1000;
    [SerializeField] private float histogramHeightMultiplierNormal = 100f;
    
    [Header("Bernoulli Distribution")]
    [SerializeField] private float xMaxBern = 7f;
    [SerializeField] private float yMaxBern = 5f;
    [SerializeField] public float xArrowOffsetBern = 7f;
    [SerializeField] public float yArrowOffsetBern = 5f;
    [SerializeField] public float cameraOffsetBern = 3f;
    [SerializeField] public float originSizeBern = 5f;
    [SerializeField] private float pointHeightBern = 0.25f;
    [SerializeField] private float pointStepBern = 1f;
    [SerializeField] private float pointPosOffsetBern = -0.5f;
    [SerializeField] private float pointAnimationDurationBern = 0.04f;
    [SerializeField] private float fontSizeBern = 4f;
    [SerializeField] private float axisWidthBern = 0.125f;
    [SerializeField] private int histogramBinsBern = 14;
    [SerializeField] private float histogramHeightMultiplierBern = 5f;
    [SerializeField] private float lineWidthBern = 0.1f;
    
    private bool _isOpenSideMenu = false;
    private float _boxWidth;
    private float _initialBoxPosition;
    private float _initialButtonsPosition;
    
    private LineRenderer _bottomLine;
    private LineRenderer _topLine;
    private LineRenderer _leftLine;
    private LineRenderer _rightLine;
    
    
    private void Start()
    {
        RectTransform box = menuButton.transform.GetChild(0) as RectTransform;
        
        _boxWidth = box.rect.width;
        _initialBoxPosition = box.rect.position.x;
        _initialButtonsPosition = menuSideButtons.transform.position.x;

        // initialize the line renderers for the rectangle simulations
        CreateLines();
        SetLineMaterials();
        SetLineParents();
        SetLineWidth();
        
        InitBernoulliDistribution();
    }
    
    // the disablerect() function needs to be called only in the normal and bern functions 
    // because they act as templates for the rest of the distributions/simulations
    public void InitNormalDistribution()
    {
        graphHandler.xAxisMaxExtent = xMaxNormal;
        graphHandler.yAxisMaxExtent = yMaxNormal;
        graphHandler.xArrowOffset = xArrowOffsetNormal;
        graphHandler.yArrowOffset = yArrowOffsetNormal;
        graphHandler.pointHeight = pointHeightNormal;
        graphHandler.pointStep = pointStepNormal;
        graphHandler.pointPosOffset = pointPosOffsetNormal;
        graphHandler.pointAnimationDuration = pointAnimationDurationNormal;
        graphHandler.fontSize = fontSizeNormal;
        
        histogramHandler.binSize = histogramBinsNormal;
        histogramHandler.heightMultiplier = histogramHeightMultiplierNormal;
        
        SetAxisWidth(graphHandler.xAxis, axisWidthNormal);
        SetAxisWidth(graphHandler.yAxis, axisWidthNormal);
        
        SetAxisArrowFont(xArrow, 12f, 8.5f);
        SetAxisArrowFont(yArrow, 12f, 8.5f);

        camera.orthographicSize = graphHandler.yAxisMaxExtent + cameraOffsetNormal;
        origin.fontSize = originSizeNormal;
        histogramGraphCanvas.SetActive(true);
        
        graphHandler.ResetGraph();
        graphHandler.ViewPlotsNormal();

        graphHandler.CurrentDistribution = graphHandler.formulas.NormalDistribution;
        EnableCorrectCanvas("Normal");
        DisableRectLines();
    }
    
    public void InitBernoulliDistribution()
    {
        graphHandler.xAxisMaxExtent = xMaxBern;
        graphHandler.yAxisMaxExtent = yMaxBern;
        graphHandler.xArrowOffset = xArrowOffsetBern;
        graphHandler.yArrowOffset = yArrowOffsetBern;
        graphHandler.pointHeight = pointHeightBern;
        graphHandler.pointStep = pointStepBern;
        graphHandler.pointPosOffset = pointPosOffsetBern;
        graphHandler.pointAnimationDuration = pointAnimationDurationBern;
        graphHandler.fontSize = fontSizeBern;
        
        histogramHandler.binSize = histogramBinsBern;
        histogramHandler.heightMultiplier = histogramHeightMultiplierBern;

        SetAxisWidth(graphHandler.xAxis, axisWidthBern);
        SetAxisWidth(graphHandler.yAxis, axisWidthBern);
        
        SetAxisArrowFont(xArrow, 24f, 17f);
        SetAxisArrowFont(yArrow, 24f, 17f);
        
        camera.orthographicSize = graphHandler.yAxisMaxExtent + cameraOffsetBern;
        origin.fontSize = originSizeBern;
        histogramGraphCanvas.SetActive(false);
        
        graphHandler.ResetGraph();
        graphHandler.ViewHistogram();
        graphHandler.CurrentDistribution = graphHandler.formulas.BernoulliDistribution;
        
        EnableCorrectCanvas("Bernoulli");
        DisableRectLines();
    }

    public void InitUniformDistribution0ToN()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("Uniform0N");

        graphHandler.CurrentDistribution = graphHandler.formulas.UniformDistribution0ToN;
    }

    public void InitUniformDistributionNToM()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("UniformNM");
        
        graphHandler.CurrentDistribution = graphHandler.formulas.UniformDistributionNToM;
    }

    public void InitUniformDistributionInterval()
    {
        InitBernoulliDistribution();
        
        histogramHandler.binSize = histogramBinsNormal;
        histogramHandler.heightMultiplier = histogramHeightMultiplierNormal;
        histogramGraphCanvas.SetActive(false);
        
        EnableCorrectCanvas("Uniform[ab)");
        
        graphHandler.ResetGraph();
        graphHandler.ViewHistogram();
        graphHandler.CurrentDistribution = graphHandler.formulas.UniformDistributionInterval;
    }

    public void InitBinomialDistribution()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("Binomial");
        
        graphHandler.CurrentDistribution = graphHandler.formulas.BinomialDistribution;
    }

    public void InitGeometricDistribution()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("Geometric");
        
        graphHandler.CurrentDistribution = graphHandler.formulas.GeometricDistribution;
    }

    public void InitExponentialDistribution()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("Exponential");
        
        graphHandler.CurrentDistribution = graphHandler.formulas.ExponentialDistribution;
    }

    public void InitPoissonDistribution()
    {
        InitBernoulliDistribution();
        EnableCorrectCanvas("Poisson");
        
        graphHandler.CurrentDistribution = graphHandler.formulas.PoissonDistribution;
    }

    public void InitRectangleSimulation()
    {
        Formulas f = graphHandler.formulas;
        
        InitBernoulliDistribution();
        EnableCorrectCanvas("Rectangle");

        AnimateRectLines(f.rectA, f.rectB, f.rectC, f.rectD);

        graphHandler.ViewHistogram();
        graphHandler.ViewPlotsSimulation();
        graphHandler.CurrentDistribution = graphHandler.formulas.RectangleSimulation;
    }

    public void InitCircleSimulation()
    {
        Formulas f = graphHandler.formulas;
        
        InitBernoulliDistribution();
        EnableCorrectCanvas("Circle");
        
        circleDrawer.gameObject.SetActive(true);
        circleDrawer.DrawCircle(f.circleRadius, f.circleRadius, f.circleX0, f.circleY0);
        AnimateRectLines(
            f.circleX0 - f.circleRadius,
            f.circleX0 + f.circleRadius,
            f.circleY0 - f.circleRadius,
            f.circleY0 + f.circleRadius
        );
        
        graphHandler.ViewHistogram();
        graphHandler.ViewPlotsSimulation();
        graphHandler.CurrentDistribution = graphHandler.formulas.CircleSimulation;
    }

    public void InitEllipseSimulation()
    {
        Formulas f = graphHandler.formulas;
        
        InitBernoulliDistribution();
        EnableCorrectCanvas("Ellipse");
        
        circleDrawer.gameObject.SetActive(true);
        circleDrawer.DrawCircle(f.ellipseA, f.ellipseB, f.ellipseX0, f.ellipseY0);
        AnimateRectLines(
            f.ellipseX0 - f.ellipseA,
            f.ellipseX0 + f.ellipseA,
            f.ellipseY0 - f.ellipseB,
            f.ellipseY0 + f.ellipseB
        );
        
        graphHandler.ViewHistogram();
        graphHandler.ViewPlotsSimulation();
        graphHandler.CurrentDistribution = graphHandler.formulas.EllipseSimulation;
    }

    public void AnimateRectLines(float rectA, float rectB, float rectC, float rectD)
    {
        float offset = lineWidthBern / 2;
        EnableRectLines();
        
        graphHandler.AnimateXAxis(_bottomLine, rectA - offset, rectB + offset, rectC);
        graphHandler.AnimateXAxis(_topLine, rectA - offset, rectB + offset, rectD);
        
        graphHandler.AnimateYAxis(_leftLine, rectC - offset, rectD + offset, rectA);
        graphHandler.AnimateYAxis(_rightLine, rectC - offset, rectD + offset, rectB);
    }

    private void CreateLines()
    {
        _bottomLine = new GameObject().AddComponent<LineRenderer>();
        _topLine = new GameObject().AddComponent<LineRenderer>();
        _leftLine = new GameObject().AddComponent<LineRenderer>();
        _rightLine = new GameObject().AddComponent<LineRenderer>();
    }

    private void SetLineMaterials()
    {
        _bottomLine.material = lineMaterial;
        _topLine.material = lineMaterial;
        _leftLine.material = lineMaterial;
        _rightLine.material = lineMaterial;
    }

    private void SetLineParents()
    {
        Transform graphTransform = graphHandler.transform;
        
        _bottomLine.transform.SetParent(graphTransform);
        _topLine.transform.SetParent(graphTransform);
        _leftLine.transform.SetParent(graphTransform);
        _rightLine.transform.SetParent(graphTransform);
    }

    private void SetLineWidth()
    {
        _bottomLine.startWidth = _bottomLine.endWidth = lineWidthBern;
        _topLine.startWidth = _topLine.endWidth = lineWidthBern;
        _leftLine.startWidth = _leftLine.endWidth = lineWidthBern;
        _rightLine.startWidth = _rightLine.endWidth = lineWidthBern;
    }

    private void DisableRectLines()
    {
        _bottomLine.gameObject.SetActive(false);
        _topLine.gameObject.SetActive(false);
        _leftLine.gameObject.SetActive(false);
        _rightLine.gameObject.SetActive(false);
    }

    private void EnableRectLines()
    {
        _bottomLine.gameObject.SetActive(true);
        _topLine.gameObject.SetActive(true);
        _leftLine.gameObject.SetActive(true);
        _rightLine.gameObject.SetActive(true);
    }
    
    private void SetAxisWidth(LineRenderer line, float width)
    {
        line.startWidth = width;
        line.endWidth = width;
    }

    private void SetAxisArrowFont(GameObject arrowObject, float arrowFontSize, float symbolFontSize)
    {
        var arrow = arrowObject.transform.GetChild(0);
        var symbol = arrowObject.transform.GetChild(1);
        
        var arrowText = arrow.gameObject.GetComponent<TextMeshPro>();
        var symbolText = symbol.gameObject.GetComponent<TextMeshPro>();
        
        arrowText.fontSize = arrowFontSize;
        symbolText.fontSize = symbolFontSize;
    }

    public void OpenSideMenu()
    {
        GameObject menuButtonChild = menuButton.transform.GetChild(0).gameObject;
        float currentMenuPos = menuButtonChild.transform.position.x;
        float currentSidePos = menuSideButtons.transform.position.x;

        if (!_isOpenSideMenu)
        {
            AnimateBarXAxis(menuButtonChild, currentMenuPos, currentMenuPos + _boxWidth, animationDuration);
            AnimateBarXAxis(menuSideButtons, currentSidePos, currentSidePos + _boxWidth, animationDuration);
            
            DisableButtons();
            
            _isOpenSideMenu = true;
            cameraScroller.enabled = false;
        }
        else
        {
            AnimateBarXAxis(menuButtonChild, currentMenuPos, _initialBoxPosition, animationDuration);
            AnimateBarXAxis(menuSideButtons, currentSidePos, _initialButtonsPosition, animationDuration);

            // this waits until the animation completes to avoid overlap
            StartCoroutine(EnableButtons());
            
            _isOpenSideMenu = false;
            cameraScroller.enabled = true;
        }
        
        // deselect all buttons because the unity buttons are dumb
        // and it doesnt support partial deselection (visual purposes)
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void AnimateBarXAxis(GameObject obj, float start, float end, float duration)
    {
        DOTween.To(() => start, val =>
        {
            start = val;
            obj.transform.position = new Vector3(start, obj.transform.position.y, 0);
        }, end, duration).SetEase(Ease.Linear);
    }

    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(animationDuration);
        
        graphCanvas.SetActive(true);
        distributionCanvas.SetActive(true);
    }
    
    private void DisableButtons()
    {
        graphCanvas.SetActive(false);
        distributionCanvas.SetActive(false);
    }

    private void EnableCorrectCanvas(string distributionName)
    {
        for (int i = 0; i < distributionCanvas.transform.childCount; i++)
        {
            var obj = distributionCanvas.transform.GetChild(i).gameObject;
            
            if (obj.name.Contains(distributionName))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }
    }
}
