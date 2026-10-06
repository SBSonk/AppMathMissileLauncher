using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 3, -10);
    [SerializeField] Vector3 lookOffset = Vector3.zero;
    [SerializeField] float lerpSpeed = 100f;
    [SerializeField] float rotLerpSpeed = 10f;

    void LateUpdate()
    {
        if (!cameraTarget) return;

        Vector3 targetPos = cameraTarget.position + cameraTarget.rotation * cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);

        Vector3 lookTarget = cameraTarget.position + cameraTarget.rotation * lookOffset;
        Vector3 lookDir = lookTarget - transform.position;
        if (lookDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir, cameraTarget.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotLerpSpeed * Time.deltaTime);
        }
    }
}