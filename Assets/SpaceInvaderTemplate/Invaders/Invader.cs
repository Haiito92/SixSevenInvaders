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

    [SerializeField] private GameObject m_onHitParticle;

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
        if (GameManager.Instance == null) return;
        m_onDestroyUnityEvent.Invoke();
        ScoreManager.Instance?.AddScore(m_scoreOnDeath);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != collideWithTag) { return; }

        GameObject hitParticle = Instantiate(m_onHitParticle, collision.transform.position, Quaternion.identity);
        StartCoroutine(DeathTimer(1.0f, hitParticle));
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(collision.gameObject);
    }

    IEnumerator DeathTimer(float timerDeath, GameObject hitParticle)
    {
        yield return new WaitForSeconds(timerDeath);
        Destroy(hitParticle);
        Destroy(gameObject);
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
    }
}
