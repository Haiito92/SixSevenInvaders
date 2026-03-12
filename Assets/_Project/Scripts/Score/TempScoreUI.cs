using System;
using TMPro;
using UnityEngine;

public class TempScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private float m_lifeTime = 2.0f;
    [SerializeField] private float m_speed = 10.0f;
    private float m_lifeTimer = 0.0f;

    private void Start()
    {
        
    }

    private void Update()
    {
        m_lifeTimer += Time.deltaTime;

        transform.position += Vector3.down * (m_speed * Time.deltaTime);

        if (m_scoreText)
        {
            Color color = m_scoreText.color;

            color.a -= Time.deltaTime / m_lifeTime;
            
            m_scoreText.color = color;
        }
        
        if (m_lifeTimer >= m_lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
