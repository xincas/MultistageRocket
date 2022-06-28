using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RocketPrefabGenerator : MonoBehaviour
{
    [SerializeField]
    private int numberOfStages = 1;
    [SerializeField]
    private GameObject stagePrefab = null;

    // Start is called before the first frame update
    void Start()
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
        }
    }
}
