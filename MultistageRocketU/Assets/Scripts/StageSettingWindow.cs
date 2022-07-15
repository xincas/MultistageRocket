using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StageSettingWindow : MonoBehaviour
{
    [SerializeField] private GameObject buttonsLayout;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject settingPrefab;
    [Space][SerializeField] private Rocket rocket;

    private void Start()
    {
        var t = transform.GetChild(0).GetChild(1);
        t.gameObject.SetActive(false);
    }

    public void AddSetting()
    {
        if (buttonsLayout == null) return;
        GameObject nSetting = Instantiate(settingPrefab);
        nSetting.GetComponent<StageSettingPrefab>().SetNumberOfStage(content.transform.childCount - 1);
        nSetting.transform.SetParent(content.transform);
        nSetting.transform.localScale = Vector3.one;
        buttonsLayout.transform.SetAsLastSibling();
    }

    public void DeleteSetting()
    {
        if (content == null) return;
        
        int idLastSetting = content.transform.childCount - 2;
        if (idLastSetting < 0) return;
        
        GameObject lastSettingGameObject = content.transform.GetChild(idLastSetting).gameObject;
        Destroy(lastSettingGameObject);
    }

    public void ReSettingRocket()
    {
        if (content == null) return;
        Transform contentTransform = content.transform;

        List<Rocket.Stage> stages = new List<Rocket.Stage>();
        float usefulMass = float.Parse(contentTransform.GetChild(0).GetComponentInChildren<TMP_InputField>().text) * 1000f;

        for (int i = 1; i < contentTransform.childCount - 1; ++i)
        {
            StageSettingPrefab settingPrefab = contentTransform.GetChild(i).GetComponent<StageSettingPrefab>();
            
            if (settingPrefab == null) return;

            Rocket.Stage nStage = settingPrefab.ParseData();
            if (nStage == null) return;
            
            stages.Add(nStage);
        }
        stages.Reverse();

        for (int i = 0; i < stages.Count; ++i)
        {
            Debug.Log(
                "Stage"+i+":\n"+
                      "mass = "+stages[i].mass+"\n"+
                      "fuel mass"+stages[i].mass*stages[i].fuelRatio+"\n"+
                      "fuel com"+stages[i].fuelСonsumption+"\n"+
                      "fuel vel"+stages[i].fuelVelocity+"\n"
                );
        }
        
        rocket.ReSettingRocket(stages, usefulMass);
    }
}
