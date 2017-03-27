using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenRelayController : MinigameBahaviour
{
    public PlayerController[] Team1;
    public PlayerController[] Team2;

    public override void Init(GameManager manager)
    {
        base.Init(manager);

        if (manager.ActivePlayers < 4) // skip this gamemode if too few players
        {
            manager.SkipCurrentMinigame();
        }
    }

    /// <summary>
    /// Ends the minigame, and declares a winner
    /// </summary>
    /// <param name="winner">The winning team</param>
    public void GameOver(int winner)
    {
        // stop game
        Manager.EndMinigame();

        // declare winner
        PlayerController[] team = Team1;
        if(winner == 2)
            team = Team2;

        string winMessage = string.Format(
            "{0} and {1} win!", 
            team[0].PlayerInfo.Name, 
            team[1].PlayerInfo.Name);

        Debug.Log(winMessage);

        // increment player scores
        for (int i = 0; i < team.Length; i++)
        {
            team[i].PlayerInfo.Score++;
        }
    }
}
