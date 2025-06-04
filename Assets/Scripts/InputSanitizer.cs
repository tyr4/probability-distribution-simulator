using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InputSanitizer : MonoBehaviour
{
    [Header("Text variables")] 
    [SerializeField] public TMP_InputField inputField;
    [SerializeField] private Formulas formulas;
    
    [SerializeField] private TMP_InputField unifNMValueN;
    [SerializeField] private TMP_InputField unifNMValueM;
    
    private string _patternProbability;
    private string _patternInteger;
    private string _patternFloat;

    private void Start()
    {
        _patternProbability = @"[a-zA-Z,!@#$%^&*()_+\-=\\\/]|^[2-9]|^1\.|(?<=\.)\.{1,}";
        _patternInteger = @"^(?![0-9]*$).*";
        _patternFloat = @"^(?![0-9]*$)*";

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
            float theta = 0;
            
            // a first switch for sanitizing values
            switch (transform.parent.name)
            {
                case "PBern Input":
                case "PBin Input":
                case "PGeom Input":
                    probabilityResult = (float)Convert.ToDouble(input);
                    probabilityResult = Mathf.Min(probabilityResult, 1);
                    
                    // take the minimum and modify the field accordingly
                    inputField.SetTextWithoutNotify(Convert.ToString(probabilityResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NUnif1 Input":
                case "NUnif0 Input":
                case "NBin Input":
                case "MUnif1 Input":
                    unifResult = Convert.ToInt32(input);
                    unifResult = Mathf.Min(unifResult, (int)(formulas.graph.xAxisMaxExtent));
                    unifResult = Mathf.Max(unifResult, -(int)(formulas.graph.xAxisMaxExtent));
                    
                    // take the minimum and modify the field accordingly
                    inputField.SetTextWithoutNotify(Convert.ToString(unifResult, CultureInfo.InvariantCulture));
                    break;
                
                case "NUnif2 Input":
                case "MUnif2 Input":
                    unifIntervalResult = (float)Convert.ToDouble(input);
                    unifIntervalResult = Mathf.Min(unifIntervalResult, formulas.graph.xAxisMaxExtent);
                    unifIntervalResult = Mathf.Max(unifIntervalResult, -formulas.graph.xAxisMaxExtent);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifIntervalResult, CultureInfo.InvariantCulture));
                    break;
                
                case "Theta Input":
                    theta = (float)Convert.ToDouble(input);
                    theta = Mathf.Max(0, theta);
                    
                    inputField.SetTextWithoutNotify(Convert.ToString(unifIntervalResult, CultureInfo.InvariantCulture));
                    break;
            }
            
            // a 2nd switch for assigning values, yes i need it
            switch (transform.parent.name)
            {
                case "PBern Input":
                    formulas.bernProbabilityValue = probabilityResult;
                    break;

                case "PBin Input":
                    formulas.binomialProbabilityValue = probabilityResult;
                    break;
                
                case "PGeom Input":
                    formulas.geometricProbabilityValue = probabilityResult;
                    break;
                
                case "Theta Input":
                    formulas.exponentialTheta = theta;
                    break;
                    
                case "NUnif0 Input":
                    formulas.unifNValueN = unifResult;
                    break;
                case "NUnif1 Input":
                    formulas.unifNMValueN = unifResult;
                    break;
                case "NUnif2 Input":
                    formulas.unifStartA = unifIntervalResult;
                    break;
                case "NBin Input":
                    formulas.binomialValueN = unifResult;
                    break;
                
                case "MUnif1 Input":
                    formulas.unifNMValueM = unifResult;
                    break;
                case "MUnif2 Input":
                    formulas.unifEndB = unifIntervalResult;
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
            formulas.unifNMValueN = 0;
            formulas.unifNMValueM = 10;
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
            formulas.unifStartA = 0;
            formulas.unifEndB = 10;
        }
    }
}
