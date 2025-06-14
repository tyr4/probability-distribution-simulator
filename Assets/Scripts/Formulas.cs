using System;
using UnityEngine;

public class Formulas : MonoBehaviour
{
    [SerializeField] public GraphHandler graph;
    [SerializeField] public MenuHandler menu;
    [SerializeField] public CircleDrawer circleDrawer;

    // random number generator seed
    private int _seed = 123456;
    
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
    
    // circle simulation
    public float circleRadius = 3;
    public float circleX0 = 0;
    public float circleY0 = 0;
    
    // ellipse simulation
    public float ellipseA = 1;
    public float ellipseB = 1;
    public float ellipseX0 = 0;
    public float ellipseY0 = 0;
    
    // this doesnt take any params, could be edited to take the number of
    // random values to be generated though
    private float Urand()
    {
        const int p = Int32.MaxValue; // 2^31 - 1
        const int a = 16807;

        float u;

        int F(int seed)
        {
            long mult = (long)a * seed;
            return (int)(mult % p);
        }
        
        _seed = F(_seed);
        u = 1.0f * _seed / p; // u ~ Unif[0, 1)
        
        return u;
    }

    public Vector2 NormalDistribution()
    {
        float u1 = Urand();
        float u2 = Urand();
        
        var z1 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        var z2 = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

        return new Vector2((float)z1, (float)z2);
    }

    public Vector2 BernoulliDistribution()
    {
        float u = Urand();
        int result = u < bernProbabilityValue ? 1 : 0;
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistribution0ToN()
    {
        float u = Urand();
        int result = (int)(unifNValueN * u);
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistributionNToM()
    {
        float u = Urand();
        int result = unifNMValueN + (int)((unifNMValueM - unifNMValueN + 1) * u);
        
        return new Vector2(result, 0);
    }

    public Vector2 UniformDistributionInterval()
    {
        float u = Urand();
        float result = unifStartA + (unifEndB - unifStartA) * u;

        return new Vector2(result, 0);
    }

    public Vector2 BinomialDistribution()
    {
        int sum = 0;
        
        for (int i = 0; i < binomialValueN; i++)
        {
            float u = Urand();
            sum += u < binomialProbabilityValue ? 1 : 0;
        }

        return new Vector2(sum, 0);
    }

    public Vector2 GeometricDistribution()
    {
        float u = Urand();
        int result = Mathf.CeilToInt(Mathf.Log(1 - u) / Mathf.Log(1 - geometricProbabilityValue));
        
        return new Vector2(result, 0);
    }

    public Vector2 ExponentialDistribution()
    {
        float u = Urand();
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
            p *= Urand();
        } while (p > L);
        
        return new Vector2(k - 1, 0);
    }

    public Vector2 RectangleSimulation()
    {
        float u1 = Urand();
        float u2 = Urand();
        
        float x = rectA + (rectB - rectA) * u1;
        float y = rectC + (rectD - rectC) * u2;
        
        return new Vector2(x, y);
    }

    public Vector2 CircleSimulation()
    {
        float x, y;
        // float theta = 2 * Mathf.PI * Urand();
        // float r = (float)(circleRadius * Math.Sqrt(Urand()));
        //
        // x = circleX0 + r * Mathf.Cos(theta);
        // y = circleY0 + r * Mathf.Sin(theta);
        //
        // return new Vector2(x, y);
        
        do
        {
            float u1 = Urand();
            float u2 = Urand();
        
            x = -circleRadius + circleX0 + 2 * circleRadius * u1;
            y = -circleRadius + circleY0 + 2 * circleRadius * u2;
        } while (!((x - circleX0) * (x - circleX0) + (y - circleY0) * (y - circleY0) <= circleRadius * circleRadius));
        
        return new Vector2(x, y);
    }

    public Vector2 EllipseSimulation()
    {
        float x, y;
        do
        {
            float u1 = Urand();
            float u2 = Urand();

            x = ellipseX0 - ellipseA + 2 * ellipseA * u1;
            y = ellipseY0 - ellipseB + 2 * ellipseB * u2;
        } 
        while (!(((x - ellipseX0) * (x - ellipseX0)) / (ellipseA * ellipseA) + 
               ((y - ellipseY0) * (y - ellipseY0)) / (ellipseB * ellipseB) <= 1f));

        return new Vector2(x, y);
    }
}