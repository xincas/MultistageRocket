using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeScaleSlider : MonoBehaviour
{
     [SerializeField] private TimeController timeController;
     
     private TMP_Text _text;
     private Slider _slider;

     private void Awake()
     {
          _text = GetComponentInChildren<TMP_Text>();
          _slider = GetComponentInChildren<Slider>();

          if (_slider != null)
          {
               _slider.onValueChanged.AddListener(UpdateText);
               _slider.onValueChanged.AddListener(UpdateTimeScale);
          }
     }

     private void OnDestroy()
     {
          if (_slider != null)
               _slider.onValueChanged.RemoveAllListeners();
     }

     private void Start()
     {
          UpdateText(_slider.value);
     }

     void UpdateText(float val)
     {
          if (_text != null)
               _text.text = "x" + val.ToString("F1");
     }

     void UpdateTimeScale(float val)
     {
          if (timeController != null)
               timeController.OnChangeTimeScale(val);
     }
}
