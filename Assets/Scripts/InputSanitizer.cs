using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class InputSanitizer : MonoBehaviour
{
    [Header("Text variables")] 
    [SerializeField] public TMP_InputField inputField;
    [SerializeField] private Formulas formulas;
    
    private string _pattern;

    private void Start()
    {
        _pattern = @"[a-zA-Z,!@#$%^&*()_+\-=\\\/]|^[2-9]|^1\.|(?<=\.)\.{1,}";

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
            inputField.onEndEdit.AddListener(ValidateInput);
        }
    }
    
    private void OnValueChanged(string input)
    {
        Debug.Log(Regex.Replace(input, _pattern, string.Empty));
        string sanitized = Regex.Replace(input, _pattern, string.Empty);
        
        inputField.SetTextWithoutNotify(sanitized);
    }

    private void ValidateInput(string input)
    {
        try
        {
            float result = (float)Convert.ToDouble(input);
            formulas.UpdateProbability(result);
        }
        catch (Exception e)
        {
            inputField.SetTextWithoutNotify(string.Empty);
            Debug.Log(e.Message);
        }
    }
}
