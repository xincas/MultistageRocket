using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StageSettingPrefab : MonoBehaviour
{
    private TMP_Text _headerText;
    private GameObject _content;

    private void Awake()
    {
        _headerText = transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        _content = transform.GetChild(1).gameObject;
    }
    
    public void SetNumberOfStage(int number)
    {
        _headerText.text = "Ступень " + number;
    }
    
    [CanBeNull]
    public Rocket.Stage ParseData()
    {
        Transform contentTransform = _content.transform;
        float[] floatInputs = new float[4];
        for (int i = 0; i < contentTransform.childCount; ++i)
        {
            TMP_InputField inputField = contentTransform.GetChild(i).GetComponentInChildren<TMP_InputField>();
            float value = float.Parse(inputField.text);
            if (value < 0f) return null;
            
            if (i != 2) value *= 1000f;
            floatInputs[i] = value;
        }

        if (floatInputs[0] < floatInputs[1]) return null;
        
        Rocket.Stage nStage = new Rocket.Stage
        {
            mass = floatInputs[0],
            fuelRatio = floatInputs[1] / floatInputs[0],
            fuelVelocity = floatInputs[2],
            fuelСonsumption = floatInputs[3]
        };

        return nStage;
    }
}
