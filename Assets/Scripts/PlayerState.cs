using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerState : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private bool showDebugUI = true;

    private int _health;

    public int Health => _health;
    public int CurrentHealth => _health;
    public int MaxHealth => maxHealth;
    public int CurrentHits => Mathf.Max(0, maxHealth - _health);
    public int MaxHits => maxHealth;
    public bool IsDead => _health <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnHitsChanged;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        _health = maxHealth;
    }

    public void TakeDamage(int damage = 1)
    {
        if (IsDead) return;

        _health = Mathf.Max(0, _health - damage);
        
        OnHealthChanged?.Invoke(_health, maxHealth);
        OnHitsChanged?.Invoke(CurrentHits, maxHealth);

        if (_health <= 0)
        {
            OnPlayerDeath?.Invoke();
            RestartGame();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;

        _health = Mathf.Min(_health + amount, maxHealth);
        OnHealthChanged?.Invoke(_health, maxHealth);
        OnHitsChanged?.Invoke(CurrentHits, maxHealth);
    }

    public void ResetState()
    {
        _health = maxHealth;
        OnHealthChanged?.Invoke(_health, maxHealth);
        OnHitsChanged?.Invoke(CurrentHits, maxHealth);
    }

    public void RestartGame()
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }

    private void OnGUI()
    {
        if (!showDebugUI) return;

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.green;
        style.alignment = TextAnchor.UpperLeft;

        string info = $"Health: {_health} / {maxHealth}";
        GUI.Box(new Rect(10, 10, 260, 45), info, style);
    }
}
