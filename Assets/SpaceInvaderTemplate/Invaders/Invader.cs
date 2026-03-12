using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Invader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform shootAt = null;
    [SerializeField] private string collideWithTag = "Player";
    [SerializeField] private UInt64 m_scoreOnDeath = 100;
    
    internal Action<Invader> onDestroy;
    [SerializeField] internal UnityEvent m_onDestroyUnityEvent;
    [SerializeField] internal UnityEvent m_ShootEvent; 
    
    [Header("Particle Effect")]
    [SerializeField] private GameObject m_onHitParticle;
    [SerializeField] private GameObject m_onDieParticle;
    [SerializeField] private GameObject m_explosionRippleParticleEffect;
    [SerializeField] private GameObject m_sixSevenParticleEffect;

    
    // Tweening
    [Header("Tweening")]
    private Vector2 m_pivot;
    [SerializeField] private float m_offsetRadius;
    [SerializeField] private float m_offsetMoveSpeed = 1.0f;
    [SerializeField] private Vector2 m_offsetPower = Vector2.one;
    
    
    private Vector2 m_originalScale;
    [SerializeField] private Vector2 m_squashPower;
    [SerializeField, Range(0.01f, 100.0f)] private float m_squashSpeed = 2.0f;
    
    private float m_randomCosOffset;
    private float m_randomSinOffset;

    private Animator m_animator;
    
    public Vector2Int GridIndex { get; private set; }

    private void Awake()
    {
        m_onDestroyUnityEvent.AddListener(() => { onDestroy?.Invoke(this); });
        m_animator = GetComponent<Animator>();
    }

    public void Initialize(Vector2Int gridIndex, Sprite invaderSprite)
    {
        this.GridIndex = gridIndex;
        if (m_spriteRenderer && invaderSprite)
        {
            m_spriteRenderer.sprite = invaderSprite;
        }
    }

    private void Start()
    {
        m_randomCosOffset = Random.Range(-1.0f, 1.0f);
        m_randomSinOffset = Random.Range(-1.0f, 1.0f);
        m_pivot = transform.localPosition;
        m_originalScale = transform.localScale;
    }

    private void Update()
    {
        if (VfxDebug.BlockEnemiesEffects)
        {
            transform.localScale = m_pivot;
            transform.localScale = m_originalScale;
            return;
        }
        
        Vector2 offset = new Vector2(
            Mathf.Cos((Time.time + m_randomCosOffset) * m_offsetPower.x * Mathf.PI / m_offsetMoveSpeed), 
            Mathf.Sin((Time.time + m_randomSinOffset) * m_offsetPower.y * Mathf.PI / m_offsetMoveSpeed));
        offset *= m_offsetRadius;
        
        transform.localPosition = m_pivot + offset;
        
        float x = m_squashPower.x * Mathf.PingPong((Time.time + m_randomCosOffset) / 2 / m_squashSpeed, 1);
        float y = m_squashPower.y * Mathf.PingPong((Time.time + m_randomCosOffset) / 2 / m_squashSpeed, 1);

        Vector2 scale = new Vector2(
            Mathf.Max(m_originalScale.x - x, Mathf.Epsilon),
            Mathf.Max(m_originalScale.y - y, Mathf.Epsilon));
        transform.localScale = scale;
        
    }

    public void OnDestroy()
    {
        
        if (GameManager.Instance == null) return;
        ScoreManager.Instance?.AddScore(m_scoreOnDeath);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != collideWithTag) { return; }

        GameObject hitParticle = null;
        GameObject deathParticle = null;
        if (!VfxDebug.BlockEnemiesEffects)
        {
            hitParticle = Instantiate(m_onHitParticle, collision.transform.position, Quaternion.identity);
            deathParticle = Instantiate(m_onDieParticle, transform.position, Quaternion.identity);
            
            Destroy(hitParticle, hitParticle.GetComponent<ParticleSystem>().main.duration);
            Destroy(deathParticle, deathParticle.GetComponent<ParticleSystem>().main.duration);
        }

        if (!VfxDebug.BlockScoreEffects)
        {
            GameObject sixSevenVFX = Instantiate(m_sixSevenParticleEffect, transform.position, Quaternion.identity);
            Destroy(sixSevenVFX, sixSevenVFX.GetComponent<ParticleSystem>().main.duration);
        }

        if (!VfxDebug.BlockRipple)
        {
            GameObject deathRipple = Instantiate(m_explosionRippleParticleEffect, transform.position, Quaternion.identity);
        }
        
        m_onDestroyUnityEvent.Invoke();
        StartCoroutine(DeathTimer(1.0f, hitParticle, deathParticle));
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(collision.gameObject);
    }

    IEnumerator DeathTimer(float timerDeath, GameObject hitParticle, GameObject deathParticle)
    {
        yield return new WaitForSeconds(timerDeath);
        if (hitParticle) Destroy(hitParticle);
        if (deathParticle) Destroy(deathParticle);
        Destroy(gameObject);
    }

    public void Shoot()
    {
        m_animator.SetTrigger("Shoot");
    }

    public void ShootAnim()
    {
        Instantiate(bulletPrefab, shootAt.position, Quaternion.identity);
        m_ShootEvent.Invoke();
    }
}
