using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MinigameBahaviour
{
    public float CookedScalar = 0f;
    public float EatenScalar = 0f;

    public Animator animator;

    public bool IsCooked
    {
        get { return CookedScalar >= 1f; }
    }

    public bool IsEaten
    {
        get { return EatenScalar >= 1f; }
    }

    // TODO: update texture based on CookedScalar here

}
