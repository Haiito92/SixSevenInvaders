using System;
using TMPro;
using UnityEngine;

public class HighScoresUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_highscoresText;

    private void Awake()
    {
        if (m_highscoresText)
        {
            m_highscoresText.text = "";
        }
    }

    private void Start()
    {
        UpdateHighScoresUI();
    }

    public void UpdateHighScoresUI()
    {
        if (!ScoreManager.Instance) return;
        if (!m_highscoresText) return;

        m_highscoresText.text = "High Scores :\n";
        
        foreach (UInt64 highScore in ScoreManager.Instance.HighScores)
        {
            m_highscoresText.text += highScore.ToString() + '\n';
        }
    }

}
