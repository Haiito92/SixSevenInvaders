using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    #region Fields
    private UInt64 m_score;
    [SerializeField] private List<UInt64> m_highScores;
    private int m_highScoreMaxAmount = 5;
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


    public void AddNewHighScore(UInt64 newHighScore)
    {
        bool highScoreChanged = false;
        
        if (m_highScores.Count == 0)
        {
            m_highScores.Add(newHighScore);
            highScoreChanged = true;
        }
        else if (m_highScores.Count == 1)
        {
            if (m_highScores[0] > newHighScore)
                m_highScores.Add(newHighScore);
            else
                m_highScores.Insert(0, newHighScore);
        }
        else
        {
            int low = 0;
            int high = m_highScores.Count;

            while (low < high)
            {
                int mid = ((high - low) / 2) + low;

                if (m_highScores[mid] > newHighScore)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid;
                }
            }

            if (low < m_highScores.Count)
            {
                m_highScores.Insert(low, newHighScore);
                highScoreChanged = true;
            }
            
            if(low < m_highScoreMaxAmount)
            {
                m_highScores.Add(newHighScore);
                highScoreChanged = true;
            }
            
        }

        int currentCount = m_highScores.Count;
        for (int i = currentCount - 1; i > m_highScoreMaxAmount - 1; i--)
        {
            m_highScores.RemoveAt(i);
        }

        if (highScoreChanged)
        {
            SaveScore();
        }
    }
    
    private void SaveScore()
    {
        
    }

    private void LoadScore()
    {
        
    }

    #region Tests
    [Header("Test Scores")] 
    [SerializeField] private UInt64 m_testHighScoreToAdd;

    [Button]
    public void AddTestHighScore() => AddNewHighScore(m_testHighScoreToAdd);

    #endregion
}
