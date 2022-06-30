using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private int numberOfStages = 1;
    [SerializeField]
    private GameObject stagePrefab = null;
    [SerializeField] 
    private StageSettings stageSettings;
    
    private List<GameObject> stages = new List<GameObject>();
    
    void Start()
    {
        InitStages();
        InitPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitStages()
    {
        if (stagePrefab is null)
            return;

        Transform lastTransform = null;
        Vector3 localScale = transform.localScale;
        
        for (int i = 0; i < numberOfStages; ++i)
        {
            if (lastTransform is null)
                lastTransform = transform.Find("head").transform;

            Vector3 pos = lastTransform.position - new Vector3(0f, 1f * localScale.y, 0);
            
            GameObject nStage = Instantiate(stagePrefab, pos, lastTransform.rotation);
            nStage.transform.localScale = localScale;
            nStage.transform.SetParent(transform);
            lastTransform = nStage.transform;
            stages.Add(nStage);
        }

        CapsuleCollider rocketCollider = transform.GetComponent<CapsuleCollider>();
        rocketCollider.center = new Vector3(0f,-numberOfStages * 0.5f, 0f);
        rocketCollider.height = numberOfStages + 1;
    }
    
    
    
    
    
    [Serializable]
    public struct StageSettings
    {
        [SerializeField] private List<float> mass; 
        [SerializeField] private List<float> fuelRatio;
        [SerializeField] private List<float> velocityBurn;
    }
}