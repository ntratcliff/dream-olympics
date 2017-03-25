using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MinigameBahaviour
{
    public int PlayerNum = 1;
    public PlayerInfo PlayerInfo;

    public override void Init(GameManager manager)
    {
        base.Init(manager);

        // get playerinfo for this player
        getPlayerInfo();
    }

    private void getPlayerInfo()
    {
        // search manager for playerinfo
        PlayerInfo[] allInfo = Manager.GetComponentsInChildren<PlayerInfo>();

        PlayerInfo = allInfo.Where(i => i.PlayerNum == PlayerNum).FirstOrDefault();

        
        if (PlayerInfo == null) // destroy self if no playerinfo in manager
            Destroy(this.gameObject);
    }
}
