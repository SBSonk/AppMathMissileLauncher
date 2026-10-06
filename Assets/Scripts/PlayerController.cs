using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float timeBeforeMaxRotate = 1f;
    [SerializeField] float moveSpeed = 5f, rotateSpeed = 30f;

    float _xInput, _yInput;
    float _xHoldTimer, _yHoldTimer;

    void Update()
    {
        HandleStrafing();

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void HandleStrafing()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(_xInput) > 0.25f)
        {
            _xHoldTimer += Time.deltaTime;
            _xHoldTimer = Mathf.Clamp(_xHoldTimer, 0, timeBeforeMaxRotate);
        } else _xHoldTimer = 0;

        if (Mathf.Abs(_yInput) > 0.25f)
        {
            _yHoldTimer += Time.deltaTime;
            _yHoldTimer = Mathf.Clamp(_yHoldTimer, 0, timeBeforeMaxRotate);
        } else _yHoldTimer = 0;

        float xMoveFactor = _xInput * EaseOutQuart(Mathf.InverseLerp(0, timeBeforeMaxRotate, _xHoldTimer));

        Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * rotateSpeed * xMoveFactor, transform.up);
        transform.rotation *= rot;
    }

    static float EaseOutQuart(float t) => 1f - Mathf.Pow(1f - t, 4f);
}
