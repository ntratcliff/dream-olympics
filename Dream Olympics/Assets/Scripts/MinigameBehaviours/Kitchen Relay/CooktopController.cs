using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooktopController : MinigameBahaviour
{
    /// <summary>
    /// The number of required presses before the activity is complete
    /// </summary>
    public int RequiredPresses = 100;

    private int currentPresses;
}
