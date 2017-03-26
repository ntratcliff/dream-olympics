using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenRelayPlayer : MinigameBahaviour
{
    public FoodHolder FoodSpot;

    private void Start()
    {
        FoodSpot = GetComponentInChildren<FoodHolder>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameRunning)
        {

        }
    }
}