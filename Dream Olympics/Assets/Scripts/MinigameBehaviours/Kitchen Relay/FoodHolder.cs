using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHolder : MinigameBahaviour
{
    public bool HasFood
    {
        get { return Food; }
    }

    [HideInInspector]
    public Food Food;

    private void Start()
    {
        Food = GetComponentInChildren<Food>();
    }

    public GameObject RemoveFood()
    {
        Food = null;

        Transform food = transform.GetChild(0);
        return food.gameObject;
    }

    public void AddFood(GameObject food)
    {
        Food = food.GetComponent<Food>();

        food.transform.SetParent(transform);
        food.transform.localPosition = Vector3.zero;
    }
}
