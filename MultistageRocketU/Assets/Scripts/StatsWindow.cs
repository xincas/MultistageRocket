using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsWindow : MonoBehaviour
{
    [SerializeField] private Rocket target;

    private List<TMP_InputField> _inputFields = new List<TMP_InputField>();
    private Rigidbody _rigidBody;
    void Start()
    {
        _rigidBody = target.GetComponent<Rigidbody>();
        var obj = transform.GetChild(0);
        obj = obj.GetChild(1);

        for (int i = 0; i < obj.childCount; ++i)
        {
            _inputFields.Add(obj.GetChild(i).GetChild(1).GetComponent<TMP_InputField>());
        }
        
        obj.gameObject.SetActive(false);
    }

    private void Update()
    {
        _inputFields[0].text = Mathf.Floor(_rigidBody.velocity.y) + " м/с";
        _inputFields[1].text = Mathf.Floor(target.CurrentHeight()) + " м";
        _inputFields[2].text = Mathf.Floor(_rigidBody.mass / 1000f) + " т";
        _inputFields[3].text = Mathf.Floor(target.CurrentFuel() / 1000f) + " т";
    }
}
