using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Invader : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";
    [SerializeField] private UInt64 m_scoreOnDeath = 100;
    
    internal Action<Invader> onDestroy;
    [SerializeField] internal UnityEvent m_onDestroyUnityEvent;

    [Header("Particle Effect")]
    [SerializeField] private GameObject m_onHitParticle;
    [SerializeField] private GameObject m_onDieParticle;
    [SerializeField] private GameObject m_explosionRippleParticleEffect;

    [Header("SFX")] [SerializeField] private AudioClip m_onDieAudio;

    // DOTween
    [SerializeField] private Vector3 m_scaleSquashOffset;
    [SerializeField, Range(0.1f, 100.0f)] private float m_squashAnimDuration = 1.0f;
    private Tweener m_squashTweener;

    public Vector2Int GridIndex { get; private set; }

    public void Initialize(Vector2Int gridIndex)
    {
        this.GridIndex = gridIndex;
    }

    private void Start()
    {
        Vector3 targetScale = transform.localScale + m_scaleSquashOffset;
        m_squashTweener = transform.DOScale(targetScale, m_squashAnimDuration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void OnDestroy()
    {
        
        if(m_squashTweener != null) m_squashTweener.Kill();
        if (GameManager.Instance == null) return;
        m_onDestroyUnityEvent.Invoke();
        ScoreManager.Instance?.AddScore(m_scoreOnDeath);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != collideWithTag) { return; }

        GameObject hitParticle = Instantiate(m_onHitParticle, collision.transform.position, Quaternion.identity);
        GameObject deathParticle = Instantiate(m_onDieParticle, transform.position, Quaternion.identity);
        GameObject deathRipple = Instantiate(m_explosionRippleParticleEffect, transform.position, Quaternion.identity);
        onDestroy?.Invoke(this);
        StartCoroutine(DeathTimer(1.0f, hitParticle, deathParticle));
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(collision.gameObject);
        SoundManager.Instance.PlaySFX3D(m_onDieAudio, transform.position, 1.0f, Random.Range(0.8f,1.2f));
    }

    IEnumerator DeathTimer(float timerDeath, GameObject hitParticle, GameObject deathParticle)
    {
        yield return new WaitForSeconds(timerDeath);
        Destroy(hitParticle);
        Destroy(deathParticle);
        Destroy(gameObject);
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
    }
}
