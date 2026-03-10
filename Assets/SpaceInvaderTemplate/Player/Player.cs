using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
            m_moveAction.action.started -= OnShoot;
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
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (Time.time > m_lastShootTimestamp + m_shootCooldown )
            {
                if (ControllerManager.Instance != null) ControllerManager.Instance.RumblePulse(0.5f, 0.8f, 0.1f);
                Shoot();
            } 
        }
    }

    void Shoot()
    {
        Instantiate(m_bulletPrefab, m_shootAt.position, Quaternion.identity);
        m_lastShootTimestamp = Time.time;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != m_collideWithTag) { return; }

        GameManager.Instance.PlayGameOver();
    }
}
