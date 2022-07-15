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

    private void FixedUpdate()
    {
        if (!isLaunched) return;

        if (stages.Count > 0)
        {
            Stage stage = stages.Last();
            if (!stage.IsStageEmpty())
            {
                stage.FuelBurn(Time.fixedDeltaTime);
                _rigidbody.mass -= stage.fuelСonsumption * Time.fixedDeltaTime;
                _force.relativeForce = new Vector3(0f, stage.fuelСonsumption * stage.fuelVelocity, 0f);
            }
            else
            {
                _force.relativeForce = Vector3.zero;
                if (!stages.Remove(stage))
                {
                    Debug.LogWarning("Can't delete last stage");
                }
                
                ColliderCentering(stages.Count);
                Undocking(stage);
                _rigidbody.mass -= stage.mass;
            }
        }
        else
        {
            _force.relativeForce = Vector3.zero;
            Time.timeScale = 0f;
            Debug.Log("Max speed: " + Vector3.Distance(Vector3.zero, _rigidbody.velocity) + "m/s");
        }

        Vector3 pos = transform.position;
        if (pos.y >= 50_000f)
        {
            height += pos.y;
            transform.position = new Vector3(pos.x, 1000f,pos.z);
        }
    }

    private void Undocking(Stage stage)
    {
        GameObject stGameObject = stage.gameObject;
        Transform stTransform = stGameObject.transform;
        
        stTransform.SetParent(null);
        
        BoxCollider stCollider = stGameObject.GetComponent<BoxCollider>();
        Rigidbody stRigidbody = stGameObject.GetComponent<Rigidbody>();

        stCollider.enabled = true;
        stRigidbody.mass = stage.mass;
        stRigidbody.velocity = _rigidbody.velocity;
        stRigidbody.isKinematic = false;
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
    public class Stage
    {
        [NonSerialized] public GameObject gameObject; 
        [SerializeField] public float mass; 
        [SerializeField] public float fuelRatio;
        [SerializeField] public float fuelСonsumption;
        [SerializeField] public float fuelVelocity;

        
        public bool IsStageEmpty()
        {
            return fuelRatio <= 0f;
        }

        public void FuelBurn(float time)
        {
            float newMass = mass - fuelСonsumption * time;
            float newFuelMass = mass * fuelRatio - fuelСonsumption * time;

            mass = newMass;
            fuelRatio = newFuelMass / newMass;
        }
    }
    
    #region Debug
    
    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            //Center of mass
            Gizmos.color = Color.red;
            DrawCenterMass.ForGizmo(_rigidbody.worldCenterOfMass, 0.1f);
        
            //Vector of velocity
            Gizmos.color = Color.black;
            Vector3 worldCenterOfMass = _rigidbody.worldCenterOfMass;
            Vector3 velocityDirection = _rigidbody.GetPointVelocity(worldCenterOfMass);
            float distance = Vector3.Distance(worldCenterOfMass, velocityDirection + worldCenterOfMass);
            DrawVector.ForGizmo(worldCenterOfMass, velocityDirection.normalized * Math.Clamp(distance, velocityVectorSize.x, velocityVectorSize.y));    
        }
    }
    
    #endregion
}