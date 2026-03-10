using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private UInt64 m_score;
    
    #region Singleton
    private static ScoreManager m_instance;
    public static ScoreManager Instance => m_instance;

    private void InitSingleton()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    #endregion
    
    private void Awake()
    {
        InitSingleton();
    }

    public void AddScore(UInt64 scoreToAdd)
    {
        m_score = InvadersMathUtils.Min(m_score + scoreToAdd, UInt64.MaxValue);
    }

    public void RemoveScore(UInt64 scoreToRemove)
    {
        m_score = InvadersMathUtils.Max(m_score - scoreToRemove, UInt64.MinValue);
    }

    public void SaveScore()
    {
        //TODO
    }
}
