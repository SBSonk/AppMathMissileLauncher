using UnityEngine;

[RequireComponent(typeof(ShipVisuals), typeof(PlayerState))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float timeBeforeMaxRotate = 1f;
    [SerializeField] float moveSpeed = 5f, rotateSpeed = 30f;
    [SerializeField] bool showDebugUI = true;

    float _xInput;
    float _turnFactor;
    PlayerState _playerState;
    ShipVisuals _shipVisuals;

    public PlayerState State => _playerState;
    public ShipVisuals Visuals => _shipVisuals;
    public int CurrentHealth => _playerState.CurrentHealth;
    public int MaxHealth => _playerState.MaxHealth;
    public int CurrentHits => _playerState.CurrentHits;
    public int MaxHits => _playerState.MaxHits;
    public float HorizontalInput => _xInput;
    public float TurnFactor => _turnFactor;

    private void Awake()
    {
        _playerState = GetComponent<PlayerState>();
        if (_playerState == null)
        {
            _playerState = gameObject.AddComponent<PlayerState>();
        }

        _shipVisuals = GetComponent<ShipVisuals>();
        if (_shipVisuals == null)
        {
            _shipVisuals = gameObject.AddComponent<ShipVisuals>();
        }
    }

    void Update()
    {
        HandleStrafing();

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void HandleStrafing()
    {
        _xInput = Input.GetAxisRaw("Horizontal");

        float target = 0f;
        if (Mathf.Abs(_xInput) > 0.25f)
        {
            target = Mathf.Sign(_xInput);
        }

        float rate = (timeBeforeMaxRotate > 0f) ? (1f / timeBeforeMaxRotate) : 100f;
        _turnFactor = Mathf.MoveTowards(_turnFactor, target, rate * Time.deltaTime);

        Quaternion rotYaw = Quaternion.AngleAxis(Time.deltaTime * rotateSpeed * _turnFactor, Vector3.up);
        transform.rotation *= rotYaw;
    }

    public void TakeDamage(int damage = 1)
    {
        if (_playerState != null)
        {
            _playerState.TakeDamage(damage);
        }
    }

    private void OnGUI()
    {
        if (!showDebugUI) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        string info = "Controls: A/D or Left/Right (Turn)";
        GUI.Box(new Rect(10, 60, 260, 40), info, style);
    }

    static float EaseOutQuart(float t) => 1f - Mathf.Pow(1f - t, 4f);
}
