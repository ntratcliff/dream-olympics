﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public string Name;
    public int PlayerNum = 1;
    public GameObject Mesh;
    public int Score;
    
    public float GetAxis(string axis)
    {
        string playerAxis = string.Format("P{0} {1}", PlayerNum, axis);
        Debug.Log("Getting axis: " + playerAxis);
        return Input.GetAxis(playerAxis);
    }   
}
