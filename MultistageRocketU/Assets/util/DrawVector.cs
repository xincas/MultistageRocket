using UnityEngine;

namespace util
{
    public static class DrawVector
    {
        public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero) return;
            Gizmos.DrawRay(pos, direction);
       
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
    
        public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction == Vector3.zero) return;
            Debug.DrawRay(pos, direction, color);
       
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
        }
    }

    public static class DrawCenterMass
    {
        
        public static void ForGizmo(Vector3 pos, float radius = 1f)
        {
            Gizmos.DrawSphere(pos, radius);
        }
    
        public static void ForDebug(Vector3 pos, Color color, float radius = 1f)
        {
            for (int i = 0; i < 8; ++i)
            {
                Vector3 point = new Vector3();

                if (i < 4) point.x = pos.x - radius;
                else point.x = pos.x + radius;

                if (i > 1 && i < 6) point.y = pos.y - radius;
                else point.y = pos.y + radius;

                if (i % 2 == 0) point.z = pos.z - radius;
                else point.z = pos.z + radius;
                
                Debug.DrawRay(point, pos.normalized * radius, color);
            }
        }
    }
}
