using System;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreText;
    private bool m_isSubscribedToScoreManager;
    [SerializeField] private RectTransform m_tempScoreSpawn;
    [SerializeField] private GameObject m_tempScoreTextPrefab;
    
    private void OnEnable()
    {
        SubscribeToScoreManagerEvents();
    }
    private void OnDisable()
    {
        UnsubscribeToScoreManagerEvents();
    }

    private void Start()
    {
        SubscribeToScoreManagerEvents();
        if (ScoreManager.Instance)
        {
            FormatScore(ScoreManager.Instance.Score);
        }
    }

    private void OnScoreChanged(UInt64 score)
    {
        FormatScore(score);
    }

    private void OnScoreAdded(UInt64 scoreAdded)
    {
        if (!VfxDebug.BlockScoreEffects)
        {
            GameObject go = Instantiate(m_tempScoreTextPrefab, m_tempScoreSpawn);
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = "+" + scoreAdded.ToString();
        }
    }
    
    private void OnScoreRemoved(UInt64 scoreRemoved)
    {
        if (!VfxDebug.BlockScoreEffects)
        {
            GameObject go = Instantiate(m_tempScoreTextPrefab, m_tempScoreSpawn);
            TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = "+" + scoreRemoved.ToString();
        }
    }

    private void FormatScore(UInt64 score)
    {
        m_scoreText.text = "Score: " + score.ToString();
    }

    private void SubscribeToScoreManagerEvents()
    {
        if (ScoreManager.Instance && !m_isSubscribedToScoreManager)
        {
            m_isSubscribedToScoreManager = true;
            ScoreManager.Instance.ScoreChanged += OnScoreChanged;
            ScoreManager.Instance.ScoreAdded += OnScoreAdded;
            ScoreManager.Instance.ScoreRemoved += OnScoreRemoved;
        }  
    }
    
    private void UnsubscribeToScoreManagerEvents()
    {
        if (ScoreManager.Instance && m_isSubscribedToScoreManager)
        {
            m_isSubscribedToScoreManager = false;
            ScoreManager.Instance.ScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.ScoreAdded -= OnScoreAdded;
            ScoreManager.Instance.ScoreRemoved -= OnScoreRemoved;
        }  
    }
}
