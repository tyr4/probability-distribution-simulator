using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Formulas : MonoBehaviour
{
    [SerializeField] public GraphHandler graph;
    [SerializeField] private MenuHandler menu;

    // bernoulli
    public float bernProbabilityValue = 0.5f;
    
    // unif {0, 1, ..., n - 1}
    public int unifNValueN = 10;
    
    // unif {n, n + 1, ..., m}
    public int unifNMValueN = 0;
    public int unifNMValueM = 10;
    
    // unif [a, b)
    public float unifStartA = 0;
    public float unifEndB = 10;
    
    // binomial
    public int binomialValueN = 10;
    public float binomialProbabilityValue = 0.5f;
    
    // geometric
    public float geometricProbabilityValue = 0.5f;
    
    // exponential
    public float exponentialTheta = 1.2f;
    
    // poisson
    public float poissonLambda = 0.5f;
    
    // rectangle simulation
    public float rectA = -2;
    public float rectB = 2;
    public float rectC = -1;
    public float rectD = 1;
    
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
        float u1 = Random.value;
        float u2 = Random.value;
        // Debug.Log($"{u1} & {u2}");
        
        var z1 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        var z2 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

        return new Vector2((float)z1, (float)z2);
    }

    public Vector2 BernoulliDistribution()
    {
        float u = Random.value;
        int result = u < bernProbabilityValue ? 1 : 0;
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistribution0ToN()
    {
        float u = Random.value;
        int result = (int)(unifNValueN * u);
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistributionNToM()
    {
        float u = Random.value;
        int result = unifNMValueN + (int)((unifNMValueM - unifNMValueN + 1) * u);
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistributionInterval()
    {
        float u = Random.value;
        float result = unifStartA + (unifEndB - unifStartA) * u;

        return new Vector2(result, 0);
    }

    public Vector2 BinomialDistribution()
    {
        int sum = 0;
        for (int i = 0; i < binomialValueN; i++)
        {
            float u = Random.value;
            sum += u < binomialProbabilityValue ? 1 : 0;
        }

        return new Vector2(sum, 0);
    }

    public Vector2 GeometricDistribution()
    {
        float u = Random.value;
        int result = (int)(Mathf.Log(1 - u) / Mathf.Log(1 - geometricProbabilityValue)) + 1;
        
        return new Vector2(result, 0);
    }

    public Vector2 ExponentialDistribution()
    {
        float u = Random.value;
        float result = -exponentialTheta * Mathf.Log(1 - u);
        
        return new Vector2(result, 0);
    }
    
    public Vector2 PoissonDistribution()
    {
        float L = Mathf.Exp(-poissonLambda);
        int k = 0;
        float p = 1f;

        do
        {
            k++;
            p *= Random.value;
        } while (p > L);
        
        return new Vector2(k - 1, 0);
    }

    public Vector2 RectangleSimulation()
    {
        float u1 = Random.value;
        float u2 = Random.value;
        
        float x = rectA + (rectB - rectA) * u1;
        float y = rectC + (rectD - rectC) * u2;
        
        return new Vector2(x, y);
    }
}

