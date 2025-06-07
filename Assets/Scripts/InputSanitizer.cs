using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InputSanitizer : MonoBehaviour
{
    [Header("Text variables")] 
    [SerializeField] public TMP_InputField inputField;
    [SerializeField] private Formulas f;
    
    [SerializeField] private TMP_InputField unifNMValueN;
    [SerializeField] private TMP_InputField unifNMValueM;
    
    private string _patternProbability;
    private string _patternInteger;
    private string _patternFloat;

    private void Start()
    {
        _patternProbability = @"[a-zA-Z,!@#$%^&*()_+\-=\\\/]|^[2-9]|^1\.|(?<=\.)\.{1,}";
        _patternInteger = @"^(?!-*[0-9]*$).*";
        _patternFloat = @"^(?!-*[0-9]*\.*[0-9]*$).*";

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onEndEdit.AddListener(ValidateInput);
        }
    }
    
    private void OnValueChanged(string input)
    {
        string sanitized = string.Empty;

        switch (transform.parent.name)
        {
            case "PBern Input":
            case "PBin Input":
            case "PGeom Input":
                sanitized = Regex.Replace(input, _patternProbability, string.Empty);
                break;
            
            case "NUnif0 Input":
            case "NUnif1 Input":
            case "NBin Input":
            case "MUnif1 Input":
                sanitized = Regex.Replace(input, _patternInteger, string.Empty);
                break;
            
            case "NUnif2 Input":
            case "MUnif2 Input":
            case "Theta Input":
            case "Lambda Input":
            case "A Input":
            case "B Input":
            case "C Input":
            case "D Input":
            case "CX0 Input":
            case "CY0 Input":
            case "R Input":
            case "EX0 Input":
            case "EY0 Input":
            case "EllipseA Input":
            case "EllipseB Input":
                sanitized = Regex.Replace(input, _patternFloat, string.Empty);
                break;
        }
        
        inputField.SetTextWithoutNotify(sanitized);
    }

    private void ValidateInput(string input)
    {
        try
        {
            float probabilityResult = 0;
            int unifResult = 0;
            float unifIntervalResult = 0;
            float rate = 0;
            
            // a first switch for sanitizing values
            switch (transform.parent.name)
            {
                case "PBern Input":
                case "PBin Input":
                case "PGeom Input":
                    probabilityResult = (float)Convert.ToDouble(input);
                    probabilityResult = Mathf.Min(probabilityResult, 1);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(probabilityResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NUnif0 Input":
                    unifResult = Convert.ToInt32(input);
                    unifResult = Mathf.Min(unifResult, (int)(f.graph.xAxisMaxExtent) - 1);
                    unifResult = Mathf.Max(unifResult, 0);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NBin Input":
                    unifResult = Convert.ToInt32(input);
                    unifResult = Mathf.Min(unifResult, (int)(f.graph.xAxisMaxExtent) - 1);
                    unifResult = Mathf.Max(unifResult, -(int)(f.graph.xAxisMaxExtent) + 1);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NUnif1 Input":
                case "MUnif1 Input":
                case "A Input":
                case "B Input":
                case "C Input":
                case "D Input":
                    unifResult = Convert.ToInt32(input);
                    unifResult = Mathf.Min(unifResult, (int)(f.graph.xAxisMaxExtent) - 1);
                    unifResult = Mathf.Max(unifResult, -(int)(f.graph.xAxisMaxExtent) + 1);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NUnif2 Input":
                case "MUnif2 Input":
                case "CX0 Input":
                case "CY0 Input":
                case "R Input":
                case "EX0 Input":
                case "EY0 Input":
                    unifIntervalResult = (float)Convert.ToDouble(input);
                    unifIntervalResult = Mathf.Min(unifIntervalResult, f.graph.xAxisMaxExtent - 1);
                    unifIntervalResult = Mathf.Max(unifIntervalResult, -f.graph.xAxisMaxExtent + 1);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifIntervalResult, CultureInfo.InvariantCulture));
                    break;
                
                case "Theta Input":
                case "Lambda Input":
                    rate = (float)Convert.ToDouble(input);
                    rate = Mathf.Max(0, rate);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(rate, CultureInfo.InvariantCulture));
                    break;
                
                case "EllipseA Input":
                case "EllipseB Input":
                    unifIntervalResult = (float)Convert.ToDouble(input);
                    unifIntervalResult = Mathf.Min(unifIntervalResult, f.graph.xAxisMaxExtent - 1);
                    unifIntervalResult = Mathf.Max(unifIntervalResult, -f.graph.xAxisMaxExtent + 1);
                    unifIntervalResult = unifIntervalResult == 0? 1 : unifIntervalResult;
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifIntervalResult, CultureInfo.InvariantCulture));
                    break;
            }
            
            // a 2nd switch for assigning values, yes i need it
            switch (transform.parent.name)
            {
                case "PBern Input":
                    f.bernProbabilityValue = probabilityResult;
                    break;

                case "PBin Input":
                    f.binomialProbabilityValue = probabilityResult;
                    break;
                
                case "PGeom Input":
                    f.geometricProbabilityValue = probabilityResult;
                    break;
                
                case "Theta Input":
                    f.exponentialTheta = rate;
                    break;
                case "Lambda Input":
                    f.poissonLambda = rate;
                    break;
                    
                case "NUnif0 Input":
                    f.unifNValueN = unifResult;
                    break;
                case "NUnif1 Input":
                    f.unifNMValueN = unifResult;
                    break;
                case "NUnif2 Input":
                    f.unifStartA = unifIntervalResult;
                    break;
                case "NBin Input":
                    f.binomialValueN = unifResult;
                    break;
                
                case "MUnif1 Input":
                    f.unifNMValueM = unifResult;
                    break;
                case "MUnif2 Input":
                    f.unifEndB = unifIntervalResult;
                    break;
                
                case "A Input":
                    f.rectA = unifResult;
                    f.menu.AnimateRectLines(f.rectA, f.rectB, f.rectC, f.rectD);
                    break;
                case "B Input":
                    f.rectB = unifResult;
                    f.menu.AnimateRectLines(f.rectA, f.rectB, f.rectC, f.rectD);
                    break;
                case "C Input":
                    f.rectC = unifResult;
                    f.menu.AnimateRectLines(f.rectA, f.rectB, f.rectC, f.rectD);
                    break;
                case "D Input":
                    f.rectD = unifResult;
                    f.menu.AnimateRectLines(f.rectA, f.rectB, f.rectC, f.rectD);
                    break;
                
                case "CX0 Input":
                    f.circleX0 = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.circleRadius, f.circleRadius, f.circleX0, f.circleY0);
                    f.menu.AnimateRectLines(
                        f.circleX0 - f.circleRadius,
                        f.circleX0 + f.circleRadius,
                        f.circleY0 - f.circleRadius,
                        f.circleY0 + f.circleRadius
                    );
                    break;
                case "CY0 Input":
                    f.circleY0 = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.circleRadius, f.circleRadius, f.circleX0, f.circleY0);
                    f.menu.AnimateRectLines(
                        f.circleX0 - f.circleRadius,
                        f.circleX0 + f.circleRadius,
                        f.circleY0 - f.circleRadius,
                        f.circleY0 + f.circleRadius
                    );
                    break;
                case "R Input":
                    f.circleRadius = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.circleRadius, f.circleRadius, f.circleX0, f.circleY0);
                    f.menu.AnimateRectLines(
                        f.circleX0 - f.circleRadius,
                        f.circleX0 + f.circleRadius,
                        f.circleY0 - f.circleRadius,
                        f.circleY0 + f.circleRadius
                    );
                    break;
                
                case "EX0 Input":
                    f.ellipseX0 = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.ellipseA, f.ellipseB, f.ellipseX0, f.ellipseY0);
                    f.menu.AnimateRectLines(
                        f.ellipseX0 - f.ellipseA,
                        f.ellipseX0 + f.ellipseA,
                        f.ellipseY0 - f.ellipseB,
                        f.ellipseY0 + f.ellipseB
                    );
                    break;
                case "EY0 Input":
                    f.ellipseY0 = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.ellipseA, f.ellipseB, f.ellipseX0, f.ellipseY0);
                    f.menu.AnimateRectLines(
                        f.ellipseX0 - f.ellipseA,
                        f.ellipseX0 + f.ellipseA,
                        f.ellipseY0 - f.ellipseB,
                        f.ellipseY0 + f.ellipseB
                    );
                    break;
                case "EllipseA Input":
                    f.ellipseA = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.ellipseA, f.ellipseB, f.ellipseX0, f.ellipseY0);
                    f.menu.AnimateRectLines(
                        f.ellipseX0 - f.ellipseA,
                        f.ellipseX0 + f.ellipseA,
                        f.ellipseY0 - f.ellipseB,
                        f.ellipseY0 + f.ellipseB
                    );
                    break;
                case "EllipseB Input":
                    f.ellipseB = unifIntervalResult;
                    
                    f.circleDrawer.DrawCircle(f.ellipseA, f.ellipseB, f.ellipseX0, f.ellipseY0);
                    f.menu.AnimateRectLines(
                        f.ellipseX0 - f.ellipseA,
                        f.ellipseX0 + f.ellipseA,
                        f.ellipseY0 - f.ellipseB,
                        f.ellipseY0 + f.ellipseB
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            inputField.SetTextWithoutNotify(string.Empty);
            Debug.Log(e.Message);
        }
    }

    // this is for integers
    public void ValidateNMUnif()
    {
        int n = unifNMValueN.text == "" ? 0: Convert.ToInt32(unifNMValueN.text.Trim());
        int m = unifNMValueM.text == "" ? 10 : Convert.ToInt32(unifNMValueM.text.Trim());
        
        if (n > m)
        {
            unifNMValueN.text = string.Empty;
            unifNMValueM.text = string.Empty;
            f.unifNMValueN = 0;
            f.unifNMValueM = 10;
        }
    }

    // and for floats
    public void ValidateABUnif()
    {
        float n = unifNMValueN.text == "" ? 0: (float)Convert.ToDouble(unifNMValueN.text.Trim());
        float m = unifNMValueM.text == "" ? 10 : (float)Convert.ToDouble(unifNMValueM.text.Trim());
        
        if (n > m)
        {
            unifNMValueN.text = string.Empty;
            unifNMValueM.text = string.Empty;
            f.unifStartA = 0;
            f.unifEndB = 10;
        }
    }
}