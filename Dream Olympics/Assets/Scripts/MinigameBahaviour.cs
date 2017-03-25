using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBahaviour : MonoBehaviour, IMinigameBehaviour
{
    private bool _gameRunning;
    private GameManager _manager;

    /// <summary>
    /// Whether or not the game is currently running
    /// </summary>
    public bool GameRunning
    {
        get
        {
            return _gameRunning;
        }

        set
        {
            _gameRunning = value;
        }
    }

    /// <summary>
    /// The game manager
    /// </summary>
    public GameManager Manager
    {
        get
        {
            return _manager;
        }
    }

    /// <summary>
    /// Called when the secene is first loaded by the main scene
    /// </summary>
    /// <param name="manager">The global game manager</param>
    public virtual void Init(GameManager manager)
    {
        // set manager
        _manager = manager;
    }

    /// <summary>
    /// Called when the game is first started
    /// </summary>
    public virtual void StartGame()
    {
        _gameRunning = true;
    }

    /// <summary>
    /// Called when the game is paused
    /// </summary>
    public virtual void PauseGame()
    {
        _gameRunning = false;
    }

    /// <summary>
    /// Called when the game is resumed from pause
    /// </summary>
    public virtual void ResumeGame()
    {
        _gameRunning = true;
    }
}
