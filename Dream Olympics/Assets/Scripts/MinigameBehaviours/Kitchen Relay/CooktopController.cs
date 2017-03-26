using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooktopController : MinigameBahaviour
{
    /// <summary>
    /// The number of required presses before the activity is complete
    /// </summary>
    public int RequiredPresses = 100;

    [HideInInspector]
    public FoodHolder FoodSpot;

    private int currentPresses = 0;

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
            if (FoodSpot.HasFood && !FoodSpot.Food.IsCooked) // increment current presses if has food
            {
                currentPresses++;

                FoodSpot.Food.CookedScalar = (float)currentPresses / RequiredPresses;
            }
            else if (!FoodSpot.HasFood 
                && currentPresses < RequiredPresses
                && controller.FoodSpot.HasFood) // put food on object
            {
                FoodSpot.AddFood(controller.FoodSpot.RemoveFood());
            }
            else if (FoodSpot.HasFood && FoodSpot.Food.IsCooked) // give food to player
            {
                controller.FoodSpot.AddFood(FoodSpot.RemoveFood());
            }
        }
    }

    
}
