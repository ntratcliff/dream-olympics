using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Zone))]
public class ButtonPressZone : MinigameBahaviour
{
    public GameObject Player;
    public UnityAction<GameObject> OnButtonPress;

    private PlayerController playerController;
    private Zone zone;
    private float lastActionVal;

    // Use this for initialization
    public override void Init(GameManager manager)
    {
        base.Init(manager);

        zone = GetComponent<Zone>();
        playerController = Player.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameRunning)
        {
            // handle button presses if player is in zone
            if (zone.HasObjects
                && Player
                && zone.ObjectsInZone.Contains(Player)
                && playerController.PlayerInfo.GetAxis("Action") > lastActionVal)
            {
                Debug.Log("Button press!");
                if (OnButtonPress != null)
                    OnButtonPress(Player);
            }

            lastActionVal = playerController.PlayerInfo.GetAxis("Action");
        }
    }
}
