using System;
using System.Collections;
using System.Collections.Generic;
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
        
        for (int i = 0; i < numberOfStages; ++i)
        {
            if (lastTransform is null)
                lastTransform = transform.Find("head").transform;

            Vector3 pos = lastTransform.position - new Vector3(0f, 1f, 0);
            
            GameObject nStage = Instantiate(stagePrefab, pos, lastTransform.rotation);
            nStage.transform.SetParent(transform);
            lastTransform = nStage.transform;
            stages.Add(nStage);
        }

        CapsuleCollider collider = transform.GetComponent<CapsuleCollider>();
        collider.center = new Vector3(0f,-numberOfStages * 0.5f, 0f);
        collider.height = numberOfStages + 1;
    }
    
    [Serializable]
    public struct StageSettings
    {
        [SerializeField] private float[] mass; 
        [SerializeField] private float[] fuelRatio;
    }
}