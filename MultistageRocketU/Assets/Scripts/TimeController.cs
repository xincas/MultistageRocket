using UnityEngine;

public class TimeController : MonoBehaviour
{
    private class TimeState
    {
        public bool isPaused { get; set; }
        public float scale { get; set; }
    }

    private TimeState _state = new TimeState
    {
        isPaused = false,
        scale = 1
    };
    
    public void PauseTime()
    {
        if (_state.isPaused) return;
        
        _state.isPaused = true;
        Time.timeScale = 0f;
    }
    
    public void UnPauseTime()
    {
        if (!_state.isPaused) return;
        
        _state.isPaused = false;
        Time.timeScale = _state.scale;
    }

    public void OnChangeTimeScale(float val)
    {
        _state.scale = val;
        if (_state.isPaused) return;
        
        Time.timeScale = _state.scale;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
