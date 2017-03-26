using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public PlayerInfo Player;
    public Text Display;

    private void Update()
    {
        Display.text = Player.Score.ToString();
    }
}
