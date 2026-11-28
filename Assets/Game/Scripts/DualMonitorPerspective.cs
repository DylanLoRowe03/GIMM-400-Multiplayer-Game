using UnityEngine;

[ExecuteAlways]
public class DualMonitorPerspective : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera rightCamera;

    [Header("Projection Settings")]
    [SerializeField] private float verticalFOV = 40f;
    [SerializeField] private float monitorAspect = 16f / 9f;

    private void LateUpdate()
    {
        if (leftCamera == null || rightCamera == null)
            return;

        rightCamera.transform.SetPositionAndRotation(
            leftCamera.transform.position,
            leftCamera.transform.rotation
        );

        ApplyProjection(leftCamera, true);
        ApplyProjection(rightCamera, false);
    }

    private void ApplyProjection(Camera cam, bool isLeft)
    {
        cam.orthographic = false;
        cam.fieldOfView = verticalFOV;

        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        float top = near * Mathf.Tan(
            verticalFOV * 0.5f * Mathf.Deg2Rad
        );

        float bottom = -top;
        float halfWidth = top * monitorAspect * 2f;

        float left = isLeft ? -halfWidth : 0f;
        float right = isLeft ? 0f : halfWidth;

        cam.projectionMatrix = Matrix4x4.Frustum(
            left,
            right,
            bottom,
            top,
            near,
            far
        );
    }

    private void OnDisable()
    {
        if (leftCamera != null)
            leftCamera.ResetProjectionMatrix();

        if (rightCamera != null)
            rightCamera.ResetProjectionMatrix();
    }
}