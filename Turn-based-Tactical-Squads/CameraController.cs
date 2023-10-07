using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float ROTATION_CHANGE = 45f;
    private const float ZOOM_SPEED = 5f;
    private const float ZOOM_CHANGE = 0.1f;
    private const float ZOOM_MIN = 0.2f;
    private const float ZOOM_MAX = 2.5f;

    private float m_MoveSpeed = 10f;
    private float m_TargetRotation = 0f;
    private float m_TargetZoom = 1.0f;

    private float m_MaxWidth;
    private float m_MaxHeight;

    private Vector3 m_BaseZoomOffset;

    [SerializeField] private Cinemachine.CinemachineVirtualCamera m_VirtualCamera;
    private Cinemachine.CinemachineTransposer m_Transposer;

    [SerializeField] private LevelGrid m_LevelGrid;

    private void Awake() 
    {
        m_Transposer = m_VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineTransposer>();
        m_BaseZoomOffset = m_Transposer.m_FollowOffset;
        m_MaxHeight = m_LevelGrid.GetHeight();
        m_MaxWidth = m_LevelGrid.GetWidth();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 l_InputDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
            l_InputDirection.z += 1f;
        
        if (Input.GetKey(KeyCode.S))
            l_InputDirection.z += -1f;

        if (Input.GetKey(KeyCode.A))
            l_InputDirection.x += -1f;

        if (Input.GetKey(KeyCode.D))
            l_InputDirection.x += 1f;

        Vector3 l_MoveVector = ((transform.forward * l_InputDirection.z) + (transform.right * l_InputDirection.x));
        transform.position += (l_MoveVector * Time.deltaTime * m_MoveSpeed);

        float l_BoundedPositionX = Mathf.Clamp(transform.position.x, 0f, m_MaxWidth);
        float l_BoundedPositionZ = Mathf.Clamp(transform.position.z, 0f, m_MaxHeight);
        transform.position = new Vector3(l_BoundedPositionX, 0, l_BoundedPositionZ);
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            m_TargetRotation += ROTATION_CHANGE;

        if (Input.GetKeyDown(KeyCode.E))
            m_TargetRotation -= ROTATION_CHANGE;

        transform.eulerAngles = new Vector3(0, m_TargetRotation, 0);
    }

    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y > 0f)
            m_TargetZoom -= ZOOM_CHANGE;

        if (Input.mouseScrollDelta.y < 0f)
            m_TargetZoom += ZOOM_CHANGE;

        m_TargetZoom = Mathf.Clamp(m_TargetZoom, ZOOM_MIN, ZOOM_MAX);
        m_Transposer.m_FollowOffset = Vector3.Lerp(m_Transposer.m_FollowOffset, m_BaseZoomOffset * m_TargetZoom, Time.deltaTime * ZOOM_SPEED);
    }
}
