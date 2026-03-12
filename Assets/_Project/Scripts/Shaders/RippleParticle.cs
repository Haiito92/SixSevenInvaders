using System;
using UnityEngine;

public class RippleParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_rippleParticleSystem;
    
    private void Start()
    {
        Destroy(this.gameObject, m_rippleParticleSystem.main.duration);
    }
}
