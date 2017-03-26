using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvasGroup : MonoBehaviour
{
    public float FadeOutTime;
    public float FadeInTime;

    private float elapsedTime;
    private float lerpTime;

    private float target;
    private float from;

    private CanvasGroup group;

    private void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(group.alpha != target)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, target, elapsedTime / lerpTime);
        }
    }

    private void startFade()
    {
        from = group.alpha;
        elapsedTime = 0f;
    }

    public void FadeOut()
    {
        target = 0f;
        lerpTime = FadeOutTime;
        startFade();
    }

    public void FadeIn()
    {
        target = 1f;
        lerpTime = FadeInTime;
        startFade();
    }
}
