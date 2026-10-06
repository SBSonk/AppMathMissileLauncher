using System;
using UnityEngine;

public class HeatSeekingMissile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f, rotateLerpSpeed = 10f;
    [SerializeField] float detectionRadius = 1f, trackTime = 5, lifetime = 10;
    [SerializeField] Transform _target;

    public Action OnHitPlayer;

    float _lifetime, _trackingTime;
    public void InitializeMissile(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (!_target) return;

        _trackingTime += Time.deltaTime; // stop tracking after x secs
        if (_trackingTime <= trackTime)
        {
            Vector3 dir = _target.position - transform.position;
            Quaternion targDir = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targDir, rotateLerpSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;        

        _lifetime += Time.deltaTime;
        if (_lifetime > lifetime) Destroy(gameObject);

        if (IsCollidingWithTarget())
        {
            OnHitPlayer?.Invoke();
            Destroy(gameObject);
        }
    }

    bool IsCollidingWithTarget()
    {
        return Vector3.Distance(transform.position, _target.position) <= detectionRadius;
    }
}
