using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Zone))]
public class OopOutOfBounds : MinigameBahaviour
{
    Zone zone;

    public override void Init(GameManager manager)
    {
        base.Init(manager);
        zone = GetComponent<Zone>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameRunning)
        {
            if (zone.HasObjects)
            {
                for (int i = 0; i < zone.ObjectsInZone.Count; i++)
                {
                    if (zone.ObjectsInZone[i].GetComponent<OopPlayer>().IsAlive)
                        zone.ObjectsInZone[i].GetComponent<OopPlayer>().DoRagdoll();
                }
            }
        }
       
    }
}
