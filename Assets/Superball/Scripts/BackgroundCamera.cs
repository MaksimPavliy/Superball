using UnityEngine;

public class BackgroundCamera: MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    Vector3 _mainCameraLastPosition;
    [SerializeField] private float offsetMultiplier = 5f;
    private Transform _transform;
    private void Start()
    {
        _mainCameraLastPosition = _mainCamera.transform.position;
        _transform = transform;
    }
    private void Update()
    {
        var currMainCameraPosition = _mainCamera.transform.position;
        Vector3 offset = currMainCameraPosition - _mainCameraLastPosition;
        var pos = _transform.position;
        // pos.x += offset.x / offsetMultiplier;
        // pos.y += offset.y;
        pos += offset / offsetMultiplier;
        _transform.position = pos;
        _mainCameraLastPosition = currMainCameraPosition;
    }

}
