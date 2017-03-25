using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMinigameBehaviour : IEventSystemHandler
{
    /// <summary>
    /// The game manager
    /// </summary>
    GameManager Manager { get;}

    /// <summary>
    /// Whether or not the game is currently running
    /// </summary>
    bool GameRunning { get; set; }

    /// <summary>
    /// Called when the secene is first loaded by the main scene
    /// </summary>
    /// <param name="manager">The global game manager</param>
    void Init(GameManager manager);

    /// <summary>
    /// Called when the game is first started
    /// </summary>
    void StartGame();

    /// <summary>
    /// Called when the game is paused
    /// </summary>
    void PauseGame();

    /// <summary>
    /// Called when the game is resumed from pause
    /// </summary>
    void ResumeGame();
}
