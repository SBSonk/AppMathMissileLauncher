using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] Vector3 cameraOffset = Vector3.up;
    [SerializeField] float lerpSpeed = 100f;

    void LateUpdate()
    {
        if (!cameraTarget) return;

        Vector3 camTargetPos = cameraTarget.position;
        camTargetPos.x = 0;
        camTargetPos.y = 0;

        Vector3 targPos = camTargetPos + cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targPos, lerpSpeed * Time.deltaTime);
    }
}
