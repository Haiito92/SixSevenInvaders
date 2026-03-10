using System;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    #region Fields
    private UInt64 m_score;
    #endregion

    #region Properties
    public UInt64 Score => m_score;
    #endregion

    #region Actions
    // UInt64 is m_score value after change
    public event Action<UInt64> ScoreChanged;
    
    // UInt64 is added score
    public event Action<UInt64> ScoreAdded;
    
    // UInt64 is removed score
    public event Action<UInt64> ScoreRemoved; 
    #endregion
    
    
    
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
        UInt64 oldScore = m_score;
        m_score = InvadersMathUtils.Min(m_score + scoreToAdd, UInt64.MaxValue);

        if (oldScore != m_score)
        {
            ScoreAdded?.Invoke(m_score - oldScore);
            ScoreChanged?.Invoke(m_score);
        }
    }

    public void RemoveScore(UInt64 scoreToRemove)
    {
        UInt64 oldScore = m_score;
        m_score = InvadersMathUtils.Max(m_score - scoreToRemove, UInt64.MinValue);
        
        if (oldScore != m_score)
        {
            ScoreRemoved?.Invoke(oldScore - m_score);
            ScoreChanged?.Invoke(m_score);
        }
    }

    public void SaveScore()
    {
        //TODO
    }
}
