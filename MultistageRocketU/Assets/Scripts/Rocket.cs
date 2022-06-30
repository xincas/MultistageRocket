using System;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private int numberOfStages = 1;
    [SerializeField]
    private GameObject stagePrefab;
    [SerializeField] 
    private List<StageSettings> stageSettings;
    
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

    private void InitStages()
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
        
        //Capsule set to right place
        CapsuleCollider rocketCollider = transform.GetComponent<CapsuleCollider>();
        rocketCollider.center = new Vector3(0f,-numberOfStages * 0.5f, 0f);
        rocketCollider.height = numberOfStages + 1;
        
        //Center of mass
        //TODO make algorithm to calc center mass of the rocket
        Rigidbody rocketRigidbody = transform.GetComponent<Rigidbody>();
        rocketRigidbody.ResetCenterOfMass();
        //rocketRigidbody.centerOfMass = Vector3.Scale(rocketCollider.center, localScale);
    }

    //Calc start position of rocket
    //TODO - calc position according to different height of each stages
    private void InitPosition()
    {
        Transform t = transform;
        Vector3 localScale = t.localScale;
        float yPos = (t.childCount - 1) * (1f * localScale.y) + (localScale.y * 1f) / 2f;
        t.position = new Vector3(0f, yPos, 0f);
    }
    
    
    
    
    
    [Serializable]
    public struct StageSettings
    {
        [SerializeField] public float mass; 
        [SerializeField] public float fuelRatio;
        [SerializeField] public float velocityBurn;
    }
}