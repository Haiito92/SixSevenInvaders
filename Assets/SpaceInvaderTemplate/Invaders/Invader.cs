using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Invader : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";
    [SerializeField] private UInt64 m_scoreOnDeath = 100;
    
    internal Action<Invader> onDestroy;
    [SerializeField] internal UnityEvent m_onDestroyUnityEvent;

    public Vector2Int GridIndex { get; private set; }

    private void Awake()
    {
        m_onDestroyUnityEvent.AddListener(() => { onDestroy.Invoke(this); });
    }

    public void Initialize(Vector2Int gridIndex)
    {
        this.GridIndex = gridIndex;
    }

    public void OnDestroy()
    {
        m_onDestroyUnityEvent.Invoke();
        ScoreManager.Instance?.AddScore(m_scoreOnDeath);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != collideWithTag) { return; }

        Destroy(gameObject);
        Destroy(collision.gameObject);
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
    }
}
