using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteSwap : MonoBehaviour
{
    public Sprite[] Sprites;
    public float Delay;

    private Image image;
    private int currentSprite;
    private float elapsed;

    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();

        image.sprite = Sprites[currentSprite];
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;

        if(elapsed >= Delay)
        {
            elapsed = 0f;
            incrementCurrentSprite();

            image.sprite = Sprites[currentSprite];
        }

    }

    private void incrementCurrentSprite()
    {
        if (++currentSprite == Sprites.Length)
            currentSprite = 0;
    }
}
