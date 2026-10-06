using System;
using UnityEngine;

public class HeatSeekingMissile : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateLerpSpeed = 5f;

    [Header("Tracking & Lifetime Settings")]
    [SerializeField] float detectionRadius = 1.5f;
    [SerializeField] float trackTime = 5f;
    [SerializeField] float lifetime = 5f;
    [SerializeField] Transform _target;

    public Action OnHitPlayer;

    float _lifetime, _trackingTime;

    public void InitializeMissile(Transform target)
    {
        _target = target;
        _lifetime = 0f;
        _trackingTime = 0f;
    }

    void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (_target != null)
        {
            _trackingTime += Time.deltaTime;
            if (_trackingTime <= trackTime)
            {
                Vector3 dir = _target.position - transform.position;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targDir = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targDir, rotateLerpSpeed * Time.deltaTime);
                }
            }

            if (IsCollidingWithTarget())
            {
                OnHitPlayer?.Invoke();

                var playerState = _target.GetComponent<PlayerState>();
                if (playerState == null)
                {
                    playerState = _target.GetComponentInParent<PlayerState>();
                }

                if (playerState != null)
                {
                    playerState.TakeDamage();
                }

                Destroy(gameObject);
                return;
            }
        }
        
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    bool IsCollidingWithTarget()
    {
        if (_target == null) return false;
        return Vector3.Distance(transform.position, _target.position) <= detectionRadius;
    }
}
