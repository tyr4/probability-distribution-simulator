using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Formulas : MonoBehaviour
{
    [SerializeField] private GraphHandler graph;
    [SerializeField] private MenuHandler menu;
    // [SerializeField] private InputSanitizer inputSanitizer;

    private float _probabilityValue = 0.5f;
    
    // TODO: FIX THIS SHIT
    private double[] Urand(int n)
    {
        const int p = Int32.MaxValue;
        const int a = 16807;
        int s = Environment.TickCount;
        Debug.Log(s);

        double[] u = new double[n];

        int F(int seed)
        {
            long mult = (long)a * seed;
            return (int)(mult % p);
        }

        for (int i = 0; i < n; i++)
        {
            s = F(s);
            u[i] = (double)s / p; // value in [0, 1)
        }

        return u;
    }

    public Vector2 NormalDistribution()
    {
        var u1 = Random.value;
        var u2 = Random.value;
        // Debug.Log($"{u1} & {u2}");
        
        var z1 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        var z2 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

        return new Vector2((float)z1, (float)z2);
    }

    public Vector2 BernoulliDistribution()
    {
        var u1 = Random.value;
        int result = u1 < _probabilityValue ? 1 : 0;
        
        return new Vector2(result, 0);
    }
    
    public void UpdateProbability(float newProbability)
    {
        _probabilityValue = newProbability;
    }
}
