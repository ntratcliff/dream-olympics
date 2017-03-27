using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OopMinigameController : MinigameBahaviour
{
    private PlayerController[] players;

    public override void Init(GameManager manager)
    {
        base.Init(manager);

    }

    public override void StartGame()
    {
        base.StartGame();

        players = GetComponentsInChildren<PlayerController>();
    }

    private void Update()
    {
        if (GameRunning)
        {
            PlayerController[] playersAlive = players.Where(p => p.GetComponent<OopPlayer>().IsAlive).ToArray();
            Debug.Log("Players Alive: " + playersAlive.Length);
            if (playersAlive.Length <= 1)
            {
                scoreAndEndGame(playersAlive);
            }
        }
    }

    private void scoreAndEndGame(PlayerController[] playersAlive)
    {
        for (int i = 0; i < playersAlive.Length; i++)
        {
            playersAlive[i].PlayerInfo.Score++;
        }

        Manager.EndMinigame();
    }
}
