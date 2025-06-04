using System.Collections.Generic;
using UnityEngine;

public class HistogramHandler : MonoBehaviour
{
    [SerializeField] public int binSize = 1000;
    [SerializeField] public float heightMultiplier = 100f;
    [SerializeField] private Material barMaterial;
    [SerializeField] private GraphHandler graphComponent;
    
    private float _xAxisMax;
    private List<LineRenderer> _lineRenderers = new List<LineRenderer>();
    public int[] binFrequency;
    
    public void Start()
    {
        _xAxisMax = graphComponent.xAxisMaxExtent;
        binFrequency = new int[binSize];
        
        CreateHistogramBars();

        if (!graphComponent.isHistogramSelected)
        {
            gameObject.SetActive(false);    
        }
    }

    public void CreateHistogramBars()
    {
        if (transform.childCount != 0)
            DeleteHistogramBars();
        
        float binWidth = (_xAxisMax * 2) / binSize;
        
        for (float i = -_xAxisMax; i < _xAxisMax; i += binWidth)
        {
            var lineObj = new GameObject($"Bin {i}");
            lineObj.transform.SetParent(transform);
            lineObj.transform.localPosition = Vector3.zero;

            var lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = barMaterial;
            lineRenderer.startWidth = 0;
            lineRenderer.endWidth = 0;
    
            lineRenderer.SetPosition(0, new Vector3(i, 0, 0));
            lineRenderer.SetPosition(1, new Vector3(i + binWidth, 0, 0));
            
            _lineRenderers.Add(lineRenderer);
        }
    }

    public void UpdateHistogramBars(List<Vector2> points)
    {
        int binIndex = 0;
        foreach (var line in _lineRenderers)
        {
            var lrPos0 = line.GetPosition(0);
            var lrPos1 = line.GetPosition(1);
            
            float height = 1.0f * binFrequency[binIndex] / points.Count;
            line.startWidth = height * heightMultiplier;
            line.endWidth = height * heightMultiplier;
            
            line.SetPosition(0, new Vector3(lrPos0.x, line.startWidth / 2, 0));
            line.SetPosition(1, new Vector3(lrPos1.x, line.startWidth / 2, 0));
            
            binIndex++;
            if (binIndex == binSize) return;
        }
    }

    public void ResetHistogramBars()
    {
        binFrequency = new int[binSize];
        
        foreach (var line in _lineRenderers)
        {
            line.startWidth = 0;
            line.endWidth = 0;
        }
    }

    public void DeleteHistogramBars()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        _lineRenderers.Clear();
    }
}

