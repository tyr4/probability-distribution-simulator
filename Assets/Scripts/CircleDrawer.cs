using System;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Formulas formulas;
    
    public int segments = 64;
    public bool loop = true;

    private void Start()
    {
        line.positionCount = segments + 1;
        Debug.Log("da sefu");
        line.material = lineMaterial;
        line.loop = loop;
        line.startWidth = line.endWidth = 0.1f;
    }

    public void DrawCircle(float a, float b, float x0, float y0)
    {
        float angleStep = 360f / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = x0 + Mathf.Cos(angle) * a;
            float y = y0 + Mathf.Sin(angle) * b;
            
            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}
