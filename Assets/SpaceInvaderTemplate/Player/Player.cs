using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Particle Effect")]
    [SerializeField] private GameObject m_nuzzleParticleEffect;

    private float m_lastShootTimestamp = Mathf.NegativeInfinity;

    [SerializeField] private UnityEvent PlayerHitEvent;
    
    //DoTween
    private Tweener m_movementDoTween;
    private Tweener m_shootDoTween;
    private Tweener m_shootDoTweenEnd;
    
    
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

        SubscribeToDebugToggleActions();
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

        if (m_movementDoTween != null)
        {
            m_movementDoTween.Kill();
        }

        if (m_shootDoTween != null)
        {
            m_shootDoTween.Kill();
        }
        
        if (m_shootDoTweenEnd != null)
        {
            m_shootDoTweenEnd.Kill();
        }
        UnsubscribeToDebugToggleActions();
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
        
        if (m_movementDoTween != null)
        {
            m_movementDoTween.Kill();
        }

        if (m_shootDoTween != null)
        {
            m_shootDoTween.Kill();
        }
        
        if (m_shootDoTweenEnd != null)
        {
            m_shootDoTweenEnd.Kill();
        }
        UnsubscribeToDebugToggleActions();
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
        UnsubscribeToDebugToggleActions();
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
        if (!VfxDebug.BlockPlayerEffects)
        {
            m_movementDoTween = transform.DORotate(rotation, 0.2f);
        }
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (Time.time > m_lastShootTimestamp + m_shootCooldown )
            {
                if (!VfxDebug.BlockPlayerEffects)
                {
                    if (ControllerManager.Instance != null) ControllerManager.Instance.RumblePulse(0.5f, 0.8f, 0.1f);
                }
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
        m_shootDoTween = transform.DOScaleY(0.3f, 0.2f).OnComplete(() =>
        {
            m_shootDoTweenEnd = transform.DOScaleY(0.5f, 0.1f);
        });

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

        if (!VfxDebug.BlockPlayerEffects)
        {
            PlayerHitEvent.Invoke();
        }
        GameManager.Instance.PlayGameOver();
    }

    #region Debug VFX
    //Debug Vfx
    [Header("Debug VFX Inputs Actions")]
    [SerializeField] private InputActionReference m_toggleAllEffects;
    [SerializeField] private InputActionReference m_toggleRipple;
    [SerializeField] private InputActionReference m_toggleFullScreenPixel;
    [SerializeField] private InputActionReference m_togglePlayerEffects;
    [SerializeField] private InputActionReference m_toggleEnemiesEffects;

    private bool m_isSubscribedToDebugToggleActions = false;
    
    private void SubscribeToDebugToggleActions()
    {
        if(m_isSubscribedToDebugToggleActions) return;
        
        m_isSubscribedToDebugToggleActions = true;
        
        if (m_toggleAllEffects)
        {
            m_toggleAllEffects.action.started += OnToggleAllEffects;
        }
        
        if (m_toggleRipple)
        {
            m_toggleRipple.action.started += OnToggleRipple;
        }
        
        if (m_toggleFullScreenPixel)
        {
            m_toggleFullScreenPixel.action.started += OnToggleFullScreenPixel;
        }
        
        if (m_togglePlayerEffects)
        {
            m_togglePlayerEffects.action.started += OnTogglePlayerEffects;
        }
        
        if (m_toggleEnemiesEffects)
        {
            m_toggleEnemiesEffects.action.started += OnToggleEnemiesEffects;
        }
    }

    private void UnsubscribeToDebugToggleActions()
    {
        if(!m_isSubscribedToDebugToggleActions) return;
        
        m_isSubscribedToDebugToggleActions = false;
        
        if (m_toggleAllEffects)
        {
            m_toggleAllEffects.action.started -= OnToggleAllEffects;
        }
        
        if (m_toggleRipple)
        {
            m_toggleRipple.action.started -= OnToggleRipple;
        }
        
        if (m_toggleFullScreenPixel)
        {
            m_toggleFullScreenPixel.action.started -= OnToggleFullScreenPixel;
        }
        
        if (m_togglePlayerEffects)
        {
            m_togglePlayerEffects.action.started -= OnTogglePlayerEffects;
        }
        
        if (m_toggleEnemiesEffects)
        {
            m_toggleEnemiesEffects.action.started -= OnToggleEnemiesEffects;
        }
    }
    
    private void OnToggleAllEffects(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            VfxDebug.ToggleAllEffects();
    }

    private void OnToggleRipple(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            VfxDebug.ToggleRipple();
    }

    private void OnToggleFullScreenPixel(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            VfxDebug.ToggleFullScreenPixel();
    }

    private void OnTogglePlayerEffects(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            VfxDebug.TogglePlayerEffects();
    }

    private void OnToggleEnemiesEffects(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            VfxDebug.ToggleEnemiesEffects();
    }
    #endregion
}
