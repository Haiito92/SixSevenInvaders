using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public enum GameState
{
    DEFAULT,
    MAIN_MENU,
    GAME,
    WAVE_STANDBY,
    GAME_OVER
}

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public enum DIRECTION { Right = 0, Up = 1, Left = 2, Down = 3 }
    
    public static GameManager Instance = null;

    internal Action onGameStateChange;

    [SerializeField] private Vector2 bounds;
    private Bounds Bounds => new Bounds(transform.position, new Vector3(bounds.x, bounds.y, 1000f));

    [SerializeField] private float gameOverHeight;

    [SerializeField] private GameState currentGameState;

    [SerializeField] private List<GameObject> listOfPatterns;
    private List<GameObject> listOfWaves;

    void Awake()
    {
        Instance = this;
        currentGameState = GameState.DEFAULT;
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        onGameStateChange?.Invoke();
    }

    public void SpawnWave(GameObject wavePatern)
    {
        GameObject newWave = Instantiate(wavePatern, new Vector3(0, 0, 0), Quaternion.identity);
        Wave waveScript = newWave.GetComponent<Wave>();
        if ( waveScript!= null)
        {
            waveScript.onWaveEnd += RemoveWaveFromList;
            listOfWaves.Add(newWave);
        }
    }

    private void RemoveWaveFromList(GameObject obj)
    {
        int indexOfObject = listOfWaves.IndexOf(obj);
        if (listOfWaves.Contains(obj))
        {
            listOfWaves.Remove(obj);
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
        Time.timeScale = 0f;
        currentGameState = GameState.GAME_OVER;
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
