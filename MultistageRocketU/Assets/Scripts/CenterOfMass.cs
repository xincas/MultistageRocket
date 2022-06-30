using System;
using UnityEngine;
using util;

public class CenterOfMass : MonoBehaviour
{
    [SerializeField] private Vector2 maxMinVelocity;
    private void OnDrawGizmos()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rigidbody.worldCenterOfMass, 0.1f);
        
        Gizmos.color = Color.black;
        Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
        Vector3 velocityDirection = rigidbody.GetPointVelocity(worldCenterOfMass);
        float distance = Vector3.Distance(worldCenterOfMass, velocityDirection + worldCenterOfMass);
        
        DrawVector.ForGizmo(worldCenterOfMass, velocityDirection.normalized * Math.Clamp(distance, maxMinVelocity.x, maxMinVelocity.y));
    }
}
