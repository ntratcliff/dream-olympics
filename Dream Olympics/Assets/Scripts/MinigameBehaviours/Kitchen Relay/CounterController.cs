using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterController : MinigameBahaviour
{
    public FoodHolder FoodHolder;

    // Use this for initialization
    void Start()
    {
        FoodHolder = GetComponentInChildren<FoodHolder>();
        ButtonPressZone[] zones = GetComponentsInChildren<ButtonPressZone>();

        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].OnButtonPress += OnButtonPressed;
        }
    }

    public void OnButtonPressed(GameObject player)
    {
        if (GameRunning)
        {
            FoodHolder playerHolder = player.GetComponentInChildren<FoodHolder>();
            if (FoodHolder.HasFood) // give player the food
            {
                playerHolder.AddFood(FoodHolder.RemoveFood());
            }
            else if (playerHolder.HasFood) // take food from player
            {
                FoodHolder.AddFood(playerHolder.RemoveFood());
            }
        }
    }
}
