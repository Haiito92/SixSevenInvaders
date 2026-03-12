using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class FullScreenPixel : MonoBehaviour
{
    [SerializeField, Range(0.1f, 100.0f)] public float m_effectSpeed = 1.0f; 
    [SerializeField] private SpriteRenderer m_spriteRenderer;
    [SerializeField] private GameObject m_67Particle;
    private GameObject _67Object;
    
    private Coroutine m_coroutine;

    private void OnDestroy()
    {
        StopEffect();
    }

    public void StartEffect()
    {
        StopEffect();
        _67Object = Instantiate(m_67Particle, transform.position, Quaternion.identity);
        m_coroutine = StartCoroutine(Effect());
    }

    private IEnumerator Effect()
    {
        while (m_spriteRenderer.color.a < 1.0f)
        {
            Color color = m_spriteRenderer.color;
            color.a = Mathf.Min(color.a + Time.deltaTime / m_effectSpeed, 1.0f);
            
            m_spriteRenderer.color =  color;
            yield return null;
        }
        
        while (m_spriteRenderer.color.a > 0.0f)
        {
            Color color = m_spriteRenderer.color;
            color.a = Mathf.Max(color.a - Time.deltaTime / m_effectSpeed, 0.0f);
            
            m_spriteRenderer.color =  color;
            yield return null;
        }
        Destroy(_67Object);
    }

    private void StopEffect()
    {
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }
    }


    [Button]
    public void TestEffect() => StartEffect();
}
