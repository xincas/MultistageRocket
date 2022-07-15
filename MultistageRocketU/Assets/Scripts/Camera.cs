using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float sensitivity = 4f;
    [SerializeField] private float limit = 80f;
    [SerializeField] private float zoomSensitivity = 0.3f;
    [SerializeField] private float zoomMin = 3f;
    [SerializeField] private float zoomMax = 10f;
    
    private Rigidbody _rigidbody;
    private Vector3 _lastMousePosition;
    
    void Start()
    {
        if (target is null)
            return;

        _rigidbody = target.GetComponent<Rigidbody>();

        Transform t = transform;
        t.LookAt(target);
        t.position = target.position + offset;
    }

    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoomSensitivity;
        else if(Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoomSensitivity;
        offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
        
        if (Input.GetMouseButton(1))
        {
            _lastMousePosition.x = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
            _lastMousePosition.y += Input.GetAxis("Mouse Y") * sensitivity;
            _lastMousePosition.y = Mathf.Clamp(_lastMousePosition.y, -limit, limit);
            transform.localEulerAngles = new Vector3(-_lastMousePosition.y, _lastMousePosition.x, 0);
        }
        
        transform.position = transform.localRotation * offset + target.position;

        transform.LookAt(target);
    }
}
