using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    
    public static ControllerManager Instance = null;

    private Gamepad _currentPad;

    private Coroutine _stopRumbleAfterTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        Instance = this;
    }

    private void OnDisable()
    {
        if (_stopRumbleAfterTime != null) StopCoroutine(_stopRumbleAfterTime);
        if (_currentPad == null) return;
        _currentPad.SetMotorSpeeds(0.0f,0.0f);
        _stopRumbleAfterTime = null;
    }

    public void RumblePulse(float lFrequency, float hFrequency, float duration)
    {
        _currentPad = Gamepad.current;

        if (_currentPad != null)
        {
            _currentPad.SetMotorSpeeds(lFrequency, hFrequency);

            _stopRumbleAfterTime = StartCoroutine(StopRumbleAfterDelay(duration));
        }
    }

    private IEnumerator StopRumbleAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (_currentPad != null)
        {
            _currentPad.SetMotorSpeeds(0.0f,0.0f);
            
        }
    } 
    
}
