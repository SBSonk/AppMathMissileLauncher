using UnityEngine;

public class MissileSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HeatSeekingMissile missilePrefab;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Camera targetCamera;

    [Header("Spawn Timing")]
    [SerializeField] private float spawnInterval = 3f;

    [Header("Difficulty Scaling")]
    [SerializeField] private int baseMissilesPerWave = 1;
    [SerializeField] private float difficultyInterval = 10f;
    [SerializeField] private int extraMissilesPerTier = 1;

    [Header("Debug")]
    [SerializeField] private bool showDebugUI = true;

    private float _spawnTimer;
    private float _survivalTimer;

    public float SurvivalTime => _survivalTimer;
    public int CurrentDifficultyTier => Mathf.FloorToInt(_survivalTimer / difficultyInterval);
    public int CurrentMissilesPerWave => baseMissilesPerWave + CurrentDifficultyTier * extraMissilesPerTier;

    private void Awake()
    {
        if (playerTarget == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindAnyObjectByType<Camera>();
            }
        }

        if (missilePrefab == null)
        {
            var sceneMissile = FindAnyObjectByType<HeatSeekingMissile>();
            if (sceneMissile != null)
            {
                missilePrefab = sceneMissile;
                // Deactivate the scene template so it is only spawned dynamically
                sceneMissile.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (playerTarget == null || missilePrefab == null) return;

        _survivalTimer += Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnInterval)
        {
            _spawnTimer = 0f;
            SpawnMissileWave();
        }
    }

    private void SpawnMissileWave()
    {
        int missilesToSpawn = CurrentMissilesPerWave;

        for (int i = 0; i < missilesToSpawn; i++)
        {
            SpawnSingleMissile();
        }
    }

    private void SpawnSingleMissile()
    {
        Vector3 spawnPosition = CalculateSpawnPositionOutsideCamera();
        Vector3 directionToPlayer = (playerTarget.position - spawnPosition).normalized;
        Quaternion spawnRotation = directionToPlayer != Vector3.zero
            ? Quaternion.LookRotation(directionToPlayer)
            : Quaternion.identity;

        HeatSeekingMissile missile = Instantiate(missilePrefab, spawnPosition, spawnRotation);
        missile.gameObject.SetActive(true);
        missile.InitializeMissile(playerTarget);
    }

    private Vector3 CalculateSpawnPositionOutsideCamera()
    {
        if (targetCamera != null && playerTarget != null)
        {
            // Calculate player's depth along camera forward vector
            float playerDepth = Vector3.Dot(playerTarget.position - targetCamera.transform.position, targetCamera.transform.forward);
            float depth = Mathf.Max(playerDepth, 15f) + Random.Range(-5f, 15f);

            // Choose a viewport position outside screen rect [0, 1]
            int side = Random.Range(0, 4);
            float vx = 0f, vy = 0f;

            switch (side)
            {
                case 0: // Off Left
                    vx = Random.Range(-0.4f, -0.15f);
                    vy = Random.Range(-0.2f, 1.2f);
                    break;
                case 1: // Off Right
                    vx = Random.Range(1.15f, 1.4f);
                    vy = Random.Range(-0.2f, 1.2f);
                    break;
                case 2: // Off Top
                    vx = Random.Range(-0.2f, 1.2f);
                    vy = Random.Range(1.15f, 1.4f);
                    break;
                case 3: // Off Bottom
                    vx = Random.Range(-0.2f, 1.2f);
                    vy = Random.Range(-0.4f, -0.15f);
                    break;
            }

            return targetCamera.ViewportToWorldPoint(new Vector3(vx, vy, depth));
        }

        // Mathematical spherical fallback around the player
        Vector3 origin = playerTarget != null ? playerTarget.position : Vector3.zero;
        Vector3 randomDirection = Random.onUnitSphere;
        return origin + randomDirection * 35f;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSpawnerExists()
    {
        if (FindAnyObjectByType<MissileSpawner>() == null)
        {
            GameObject spawnerObj = new GameObject("MissileSpawner");
            spawnerObj.AddComponent<MissileSpawner>();
        }
    }

    private void OnGUI()
    {
        if (!showDebugUI) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.yellow;
        style.alignment = TextAnchor.UpperLeft;

        string info = $"Survival Time: {_survivalTimer:F1}s\nDifficulty Tier: +{CurrentDifficultyTier}\nMissiles / Wave: {CurrentMissilesPerWave}";
        GUI.Box(new Rect(10, 102, 260, 65), info, style);
    }
}
