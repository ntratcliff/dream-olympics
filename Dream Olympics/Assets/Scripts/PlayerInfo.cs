using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public string Name;
    public int PlayerNum = 1;
    public GameObject Mesh;
    public int Score;
    public GameObject ScoreboardObject;
    public Text ScoreboardName, ScoreboardNumber;
    
    public float GetAxis(string axis)
    {
        string playerAxis = string.Format("P{0} {1}", PlayerNum, axis);
        return Input.GetAxis(playerAxis);
    }   
}
