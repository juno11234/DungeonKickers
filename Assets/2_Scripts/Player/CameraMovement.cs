using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    public float panSpeed = 20f;
    public float edgeThreshold = 20f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 10f;

    private Camera mainCam;

    private Vector2 mousePosition;

    private void Start()
    {
        mainCam = Camera.main;
    }
    private void Update()
    {
        // 매 프레임마다 마우스의 현재 위치를 가져옵니다.
        mousePosition = Mouse.current.position.ReadValue();
        HandleEdgePan();
    }

    public void HandleEdgePan()
    {
        Vector3 moveDirection = Vector3.zero;

        // 화면 가장자리 이동 감지
        if (mousePosition.x < edgeThreshold)
        {
            moveDirection.x = 1;
        }
        else if (mousePosition.x > Screen.width - edgeThreshold)
        {
            moveDirection.x = -1;
        }

        if (mousePosition.y < edgeThreshold)
        {
            moveDirection.z = 1;
        }
        else if (mousePosition.y > Screen.height - edgeThreshold)
        {
            moveDirection.z = -1;
        }

        // 이동 방향으로 카메라 이동
        mainCam.transform.Translate(moveDirection * panSpeed * Time.deltaTime, Space.World);
    }

    public void Zoom(float scrollDelta)
    {
        // 마우스 휠 스크롤 값에 따라 카메라 Size 변경
        float newSize = mainCam.orthographicSize - scrollDelta * zoomSpeed;

        // Size 값을 최소/최대 범위로 제한
        mainCam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}
