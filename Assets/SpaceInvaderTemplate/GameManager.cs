using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public enum GameState
{
    DEFAULT,
    MAIN_MENU,
    START_GAME,
    GAME,
    WAVE_STANDBY,
    GAME_OVER
}


public class GameManager : MonoBehaviour
{
    public enum DIRECTION { Right = 0, Up = 1, Left = 2, Down = 3 }
    
    public static GameManager Instance = null;

    internal Action onGameStateChange;

    [Header("GameManager Data")]

    [SerializeField] private Vector2 bounds;
    private Bounds Bounds => new Bounds(transform.position, new Vector3(bounds.x, bounds.y, 1000f));

    [SerializeField] private float gameOverHeight;

    [SerializeField] private GameState currentGameState;

    [SerializeField] private List<GameObject> listOfPatterns;
    private List<GameObject> _listOfWaves = new List<GameObject>();
    private int _indexOfPatterns = 0;

    private IEnumerator _waitForWavesCoroutine;
    
    
    //UI
    [Header("UI")]
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject EndMenuUI;
    [SerializeField] private HighScoresUI HighScoresUI;
    
    //Player
    [Header("Player")]
    [SerializeField] private Player currentPlayer;

    [Header("Sounds / Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip highScoreMusic;

    void Awake()
    {
        // if (Instance != null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     return;
        // }
        Instance = this;
        currentGameState = GameState.MAIN_MENU;
        onGameStateChange += OnGameStateChange;
        _waitForWavesCoroutine = WaitForNextWave(2.0f);
    }

    public void StartGame()
    {
        ChangeGameState(GameState.START_GAME);
    }

    public void ResetGame()
    {
        currentPlayer.ResetPlayer();
        StopAllCoroutines();
        if(onGameStateChange != null) onGameStateChange -= OnGameStateChange;
        SceneManager.LoadScene(1);
    }

    private void OnDestroy()
    {
        if(onGameStateChange != null) onGameStateChange -= OnGameStateChange;
    }

    public void QuitGame()
    {
        if(onGameStateChange != null) onGameStateChange -= OnGameStateChange;
        QuitGame();
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        onGameStateChange?.Invoke();
    }

    private void OnGameStateChange()
    {
        switch (currentGameState)
        {
            case GameState.DEFAULT:
                break;
            case GameState.MAIN_MENU: //MainMenu 
                EndMenuUI.SetActive(false);
                MainMenuUI.SetActive(true);
                SoundManager.Instance.StopMusic();
                SoundManager.Instance.PlayMusic(mainMenuMusic);
                break;
            case GameState.START_GAME:
                MainMenuUI.SetActive(false);
                ChangeGameState(GameState.GAME);
                SoundManager.Instance.StopMusic();
                SoundManager.Instance.PlayMusic(gameMusic);
                break;
            case GameState.GAME: //GameRunning main game WITH WAVE
                StopCoroutine(_waitForWavesCoroutine);
                Time.timeScale = 1f;
                if (_indexOfPatterns < listOfPatterns.Count) SpawnWave(listOfPatterns[_indexOfPatterns]);
                else
                {
                    //END GAME it was last wave
                    ChangeGameState(GameState.GAME_OVER);
                }
                break;
            case GameState.WAVE_STANDBY: //Between 2 waves
                StartCoroutine(_waitForWavesCoroutine);
                _indexOfPatterns++;
                break;
            case GameState.GAME_OVER: //EndGame if player dead OR lastWave is done
                StopAllCoroutines();
                Time.timeScale = 0f;
                _indexOfPatterns = 0;
                currentPlayer.ResetPlayer();

                if (ScoreManager.Instance)
                {
                    ScoreManager.Instance.AddGameScoreToHighScores();
                    ScoreManager.Instance.ResetScore();
                }
                
                EndMenuUI.SetActive(true);
                HighScoresUI?.UpdateHighScoresUI();
                
                SoundManager.Instance.StopMusic();
                SoundManager.Instance.PlayMusic(highScoreMusic);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SpawnWave(GameObject wavePatern)
    {
        GameObject newWave = Instantiate(wavePatern, new Vector3(0, 0, 0), Quaternion.identity);
        Wave waveScript = newWave.GetComponent<Wave>();
        if ( waveScript!= null)
        {
            waveScript.onWaveEnd += RemoveWaveFromList;
            _listOfWaves.Add(newWave);
        }
        waveScript.StartWave();
    }

    private void RemoveWaveFromList(GameObject obj)
    {
        int indexOfObject = _listOfWaves.IndexOf(obj);
        if (_listOfWaves.Contains(obj))
        {
            _listOfWaves.Remove(obj);
            Destroy(obj);
        }
        obj.GetComponent<Wave>().onWaveEnd -= RemoveWaveFromList;
        ChangeGameState(GameState.WAVE_STANDBY);
    }

    public Vector3 KeepInBounds(Vector3 position)
    {
        return Bounds.ClosestPoint(position);
    }

    public float KeepInBounds(float position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: return Mathf.Min(position, Bounds.max.x);
            case DIRECTION.Up: return Mathf.Min(position, Bounds.max.y);
            case DIRECTION.Left: return Mathf.Max(position, Bounds.min.x);
            case DIRECTION.Down: return Mathf.Max(position, Bounds.min.y);
            default: return position;
        }
    }

    public bool IsInBounds(Vector3 position)
    {
        return Bounds.Contains(position);
    }

    public bool IsInBounds(Vector3 position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: case DIRECTION.Left: return IsInBounds(position.x, side);
            case DIRECTION.Up: case DIRECTION.Down: return IsInBounds(position.y, side);
            default: return false;
        }
    }

    public bool IsInBounds(float position, DIRECTION side)
    {
        switch (side)
        {
            case DIRECTION.Right: return position <= Bounds.max.x;
            case DIRECTION.Up: return position <= Bounds.max.y;
            case DIRECTION.Left: return position >= Bounds.min.x;
            case DIRECTION.Down: return position >= Bounds.min.y;
            default: return false;
        }
    }

    public bool IsBelowGameOver(float position)
    {        
        return position < transform.position.y + (gameOverHeight - bounds.y * 0.5f);
    }

    public void PlayGameOver()
    {
        Debug.Log("Game Over");

        ChangeGameState(GameState.GAME_OVER);
    }

    private IEnumerator WaitForNextWave(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ChangeGameState(GameState.GAME);
    } 

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, new Vector3(bounds.x, bounds.y, 0f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position + Vector3.up * (gameOverHeight - bounds.y * 0.5f) - Vector3.right * bounds.x * 0.5f,
            transform.position + Vector3.up * (gameOverHeight - bounds.y * 0.5f) + Vector3.right * bounds.x * 0.5f);
    }
}
