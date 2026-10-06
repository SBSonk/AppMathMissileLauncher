using UnityEngine;

public class ShipVisuals : MonoBehaviour
{
    [SerializeField] private Transform visualTransform;
    [SerializeField] private PlayerController playerController;

    [Header("Roll")]
    [SerializeField] private float maxRollAngle = 45f;

    private Quaternion _initialLocalRotation = Quaternion.identity;

    public Transform VisualTransform
    {
        get => visualTransform;
        set
        {
            visualTransform = value;
            if (visualTransform != null)
            {
                _initialLocalRotation = visualTransform.localRotation;
            }
        }
    }

    public float MaxRollAngle
    {
        get => maxRollAngle;
        set => maxRollAngle = value;
    }

    private void Awake()
    {
        if (visualTransform != null)
        {
            _initialLocalRotation = visualTransform.localRotation;
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerController>();
            }
        }
    }

    private void LateUpdate()
    {
        ApplyShipBank();
    }

    public void ApplyBank(float turnFactor)
    {
        if (visualTransform == null) return;
        
        if (turnFactor >= 0f)
        {
            Quaternion maxRightBank = _initialLocalRotation * Quaternion.AngleAxis(-maxRollAngle, Vector3.forward);
            visualTransform.localRotation = Quaternion.Slerp(_initialLocalRotation, maxRightBank, Mathf.Clamp01(turnFactor));
        }
        else
        {
            Quaternion maxLeftBank = _initialLocalRotation * Quaternion.AngleAxis(maxRollAngle, Vector3.forward);
            visualTransform.localRotation = Quaternion.Slerp(_initialLocalRotation, maxLeftBank, Mathf.Clamp01(-turnFactor));
        }
    }

    private void ApplyShipBank()
    {
        if (visualTransform == null) return;

        float turnFactor = 0f;
        if (playerController != null)
        {
            turnFactor = playerController.TurnFactor;
        }
        else
        {
            turnFactor = Input.GetAxisRaw("Horizontal");
        }

        ApplyBank(turnFactor);
    }
}
