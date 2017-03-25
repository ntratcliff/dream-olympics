using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLog : MinigameBahaviour
{
    public override void Init(GameManager manager)
    {
        base.Init(manager);

        Debug.Log(string.Format("{0}: Init!", name));
    }

    public override void StartGame()
    {
        base.StartGame();

        Debug.Log(string.Format("{0}: Init!", name));
    }

    public override void PauseGame()
    {
        base.PauseGame();

        Debug.Log(string.Format("{0}: Init!", name));
    }

    public override void ResumeGame()
    {
        base.ResumeGame();

        Debug.Log(string.Format("{0}: Init!", name));
    }
}
