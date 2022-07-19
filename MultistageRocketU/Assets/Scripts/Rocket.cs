using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using util;
using Logger = util.Logger;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject stagePrefab;
    [Space] [SerializeField] private float massRocket;
    [SerializeField] private List<Stage> stages;

    [Space] [Space] [SerializeField] private Vector2 velocityVectorSize;
    private float height;

    private Rigidbody _rigidbody;
    private ConstantForce _force;
    private CapsuleCollider _collider;
    private Transform _flameTransform;
    private Transform _headTransform;
    private bool _isLaunched = false;
    private float _launchTime;

    private Logger _logger = new Logger();
    void Start()
    {
        _rigidbody = transform.GetComponent<Rigidbody>();
        _force = transform.GetComponent<ConstantForce>();
        _collider = transform.GetComponent<CapsuleCollider>();
        _flameTransform = transform.Find("Flame");
        _headTransform = transform.Find("Fairing");
        InitStages();
    }

    private void FixedUpdate()
    {
        if (!_isLaunched) return;

        if (stages.Count > 0)
        {
            Stage stage = stages.Last();
            if (!stage.IsStageEmpty())
            {
                stage.FuelBurn(Time.fixedDeltaTime);
                _rigidbody.mass -= stage.fuelСonsumption * Time.fixedDeltaTime;
                _force.force = new Vector3(0f, stage.fuelСonsumption * stage.fuelVelocity, 0f);
            }
            else
            {
                _force.force = Vector3.zero;
                if (!stages.Remove(stage))
                {
                    Debug.LogWarning("Can't delete last stage");
                }
                
                if (stages.Count != 0)
                    _flameTransform.position = stages.Last().gameObject.transform.position + new Vector3(0f, -1.75f, 0f);
                else 
                    _flameTransform.gameObject.SetActive(false);
                
                ColliderCentering(stages.Count);
                Undocking(stage);
                _rigidbody.mass -= stage.mass;
            }
        }
        else
        {
            _force.force = Vector3.zero;
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
        
        CapsuleCollider stCollider = stGameObject.GetComponent<CapsuleCollider>();
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

        Transform lastTransform = _headTransform;
        Vector3 localScale = transform.localScale;
        
        Vector3 pos = - new Vector3(0f, 2.2f, 0);
        GameObject nStage = Instantiate(stagePrefab, Vector3.zero, lastTransform.rotation);
        GameObject interStage = nStage.transform.GetChild(0).GetChild(3).gameObject;
        interStage.SetActive(false);
        nStage.transform.SetParent(transform);
        nStage.transform.localPosition = pos;
        stages[0].gameObject = nStage;
        
        for (int i = 1; i < stages.Count; ++i)
        {
            lastTransform = nStage.transform;
            pos = lastTransform.localPosition - new Vector3(0f, 4.09f, 0);
            
            nStage = Instantiate(stagePrefab, Vector3.zero, lastTransform.rotation);
            nStage.transform.SetParent(transform);
            nStage.transform.localPosition = pos;
            stages[i].gameObject = nStage;
        }
        
        ColliderCentering(stages.Count);

        //Center of mass
        //TODO make algorithm to calc center mass of the rocket
        _rigidbody.ResetCenterOfMass();
        
        //Calc mass of all rocket
        float stagesMass = 0f;
        foreach (var set in stages)
        {
            stagesMass += set.mass;
        }
        _rigidbody.mass = massRocket + stagesMass;

        //rocketRigidbody.centerOfMass = Vector3.Scale(rocketCollider.center, localScale);
        _flameTransform.position = stages.Last().gameObject.transform.position + new Vector3(0f, -1.75f, 0f);
        _flameTransform.gameObject.SetActive(false);
        
        transform.position -= _collider.bounds.min;
    }

    private void ColliderCentering(int numberOfStages)
    {
        //Capsule set to right place
        float height = 2.5f;
        if (numberOfStages == 1)
            height += 3.6f;
        else if (numberOfStages == 2)
            height += 2f + 5.5f;
        else
            height += 2f + (numberOfStages - 2) * 4.25f + 5.5f;
        
        _collider.center = new Vector3(0f, 2.5f - height / 2, 0f);
        _collider.height = height;
    }

    public float CurrentHeight()
    {
        return height + transform.position.y;
    }
    
    public float CurrentFuel()
    {
        float mass = 0;
        foreach (var st in stages)
        {
            mass += st.mass * st.fuelRatio;
        }
        return mass;
    }

    public void Launch()
    {
        _isLaunched = true;
        _launchTime = Time.time;
        _flameTransform.gameObject.SetActive(true);

        StartCoroutine(nameof(Log));
    }

    public void ReSettingRocket(List<Stage> list, float usefulMass)
    {
        if (list.Count == 0) return;
        StopCoroutine(nameof(Log));
        
        _isLaunched = false;
        DestroyAllStages();

        massRocket = usefulMass;
        stages = list;

        height = 0f;
        _force.force = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        
        InitStages();
    }

    void DestroyAllStages()
    {
        foreach (var stage in stages)
        {
            Destroy(stage.gameObject);
        }
    }
    
    IEnumerator Log()
    {
        _logger.Log(_rigidbody.velocity.y, _rigidbody.mass, CurrentHeight(), Time.time - _launchTime);
        while(true)
        {
            yield return new WaitForFixedUpdate();
            _logger.LogAppend(_rigidbody.velocity.y, _rigidbody.mass, CurrentHeight(), Time.time - _launchTime);
        }
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