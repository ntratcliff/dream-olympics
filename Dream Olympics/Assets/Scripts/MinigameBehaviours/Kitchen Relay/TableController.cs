using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableController : MinigameBahaviour
{
    /// <summary>
    /// The number of required presses before the activity is complete
    /// </summary>
    public int RequiredPresses = 100;

    public int Team = 1;

    [HideInInspector]
    public FoodHolder FoodSpot;

    private int currentPresses = 0;
    private KitchenRelayController minigameController;

    private void Start()
    {
        FoodSpot = GetComponentInChildren<FoodHolder>();

        GetComponentInChildren<ButtonPressZone>().OnButtonPress += OnButtonPressed;
    }

    public void OnButtonPressed(GameObject player)
    {
        if (GameRunning)
        {
            KitchenRelayPlayer controller = player.GetComponent<KitchenRelayPlayer>();
            if (FoodSpot.HasFood && currentPresses < RequiredPresses) // increment current presses if has food
            {
                currentPresses++;

                FoodSpot.Food.EatenScalar = (float)currentPresses / RequiredPresses;
            }
            else if (!FoodSpot.HasFood 
                && controller.FoodSpot.HasFood
                && controller.FoodSpot.Food.IsCooked) // put food on object
            {
                FoodSpot.AddFood(controller.FoodSpot.RemoveFood());
            }

            if (FoodSpot.HasFood && FoodSpot.Food.IsEaten)
            {
                GetComponentInParent<KitchenRelayController>().GameOver(Team);
            }
        }
    }
}
