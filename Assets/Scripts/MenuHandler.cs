using System;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

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
    
    private bool _isOpenSideMenu = false;
    private float _boxWidth;
    private float _initialBoxPosition;
    private float _initialButtonsPosition;

    
    private void Start()
    {
        RectTransform box = menuButton.transform.GetChild(0) as RectTransform;
        
        _boxWidth = box.rect.width;
        _initialBoxPosition = box.rect.position.x;
        _initialButtonsPosition = menuSideButtons.transform.position.x;
        
        InitBernoulliDistribution();
    }
    
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
        
        SetAxisArrowFont(xArrow, 24f, 17f);
        SetAxisArrowFont(yArrow, 24f, 17f);

        camera.orthographicSize = graphHandler.yAxisMaxExtent + cameraOffsetNormal;
        origin.fontSize = originSizeNormal;
        histogramGraphCanvas.SetActive(true);
        
        graphHandler.ResetGraph();
        graphHandler.ViewPlots();
        EnableCorrectCanvas("Normal");
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
        
        SetAxisArrowFont(xArrow, 12f, 8.5f);
        SetAxisArrowFont(yArrow, 12f, 8.5f);
        
        camera.orthographicSize = graphHandler.yAxisMaxExtent + cameraOffsetBern;
        origin.fontSize = originSizeBern;
        histogramGraphCanvas.SetActive(false);
        
        graphHandler.ResetGraph();
        graphHandler.ViewHistogram();
        EnableCorrectCanvas("Bernoulli");
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
        }
        else
        {
            AnimateBarXAxis(menuButtonChild, currentMenuPos, _initialBoxPosition, animationDuration);
            AnimateBarXAxis(menuSideButtons, currentSidePos, _initialButtonsPosition, animationDuration);

            // this waits until the animation completes to avoid overlap
            StartCoroutine(EnableButtons());
            
            _isOpenSideMenu = false;
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
