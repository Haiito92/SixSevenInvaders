using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private InputActionReference m_moveAction;
    [SerializeField] private InputActionReference m_shootAction;
    
    [SerializeField] private float m_deadzone = 0.3f;
    [SerializeField] private float m_speed = 2f;
    private float m_direction = 0.0f;
    
    [SerializeField] private Bullet m_bulletPrefab = null;
    [SerializeField] private Transform m_shootAt = null;
    [SerializeField] private float m_shootCooldown = 1f;
    [SerializeField] private string m_collideWithTag = "Untagged";

    [SerializeField] private AudioClip m_shootSound;

    [SerializeField] private GameObject m_nuzzleParticleEffect;

    private float m_lastShootTimestamp = Mathf.NegativeInfinity;

    private void OnEnable()
    {
        if (m_moveAction)
        {
            m_moveAction.action.started += OnMove;
            m_moveAction.action.performed += OnMove;
            m_moveAction.action.canceled += OnMove;
        }

        if (m_shootAction)
        {
            m_shootAction.action.started += OnShoot;
        }
    }

    private void Update()
    {
        Move();
    }

    private void OnDisable()
    {
        if (m_moveAction)
        {
            m_moveAction.action.started -= OnMove;
            m_moveAction.action.performed -= OnMove;
            m_moveAction.action.canceled -= OnMove;
        }
        
        if (m_shootAction)
        {
            m_shootAction.action.started -= OnShoot;
        } 
    }
    
    private void OnDestroy()
    {
        if (m_moveAction)
        {
            m_moveAction.action.started -= OnMove;
            m_moveAction.action.performed -= OnMove;
            m_moveAction.action.canceled -= OnMove;
        }
        
        if (m_shootAction)
        {
            m_shootAction.action.started -= OnShoot;
        } 
    }

    public void ResetPlayer()
    {
        if (m_moveAction)
        {
            m_moveAction.action.started -= OnMove;
            m_moveAction.action.performed -= OnMove;
            m_moveAction.action.canceled -= OnMove;
        }
        
        if (m_shootAction)
        {
            m_shootAction.action.started -= OnShoot;
        } 
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        //float move = 0.0f;
        //if (Mathf.Abs(move) < deadzone) { return; }
        m_direction = Math.Sign(ctx.ReadValue<float>());
    }

    void Move()
    {
        float delta = m_direction * m_speed * Time.deltaTime;
        transform.position = GameManager.Instance.KeepInBounds(transform.position + Vector3.right * delta);
        Vector3 rotation = new Vector3(0.0f, 0.0f, -20.0f * m_direction);
        //transform.DORotate(rotation, 0.2f);
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (Time.time > m_lastShootTimestamp + m_shootCooldown )
            {
                if (ControllerManager.Instance != null) ControllerManager.Instance.RumblePulse(0.5f, 0.8f, 0.1f);
                SoundManager.Instance.PlaySFX3D(m_shootSound, transform.position, 1.0f, Random.Range(0.8f,1.2f));
                Shoot();
            } 
        }
    }

    void Shoot()
    {
        Instantiate(m_bulletPrefab, m_shootAt.position, Quaternion.identity);
        GameObject nuzzle = Instantiate(m_nuzzleParticleEffect, m_shootAt.position, Quaternion.identity);
        StartCoroutine(NuzzleDeath(nuzzle));
        // transform.DOScaleY(0.3f, 0.2f).OnComplete(() =>
        // {
        //     transform.DOScaleY(0.5f, 0.1f);
        // });

        m_lastShootTimestamp = Time.time;
    }

    IEnumerator NuzzleDeath(GameObject nuzzleParticle)
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(nuzzleParticle);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != m_collideWithTag) { return; }

        GameManager.Instance.PlayGameOver();
    }
}
