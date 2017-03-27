using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FadeCanvasGroup : MonoBehaviour
{
    public float FadeOutTime;
    public float FadeInTime;

    public bool Interactable = true;

    private float elapsedTime;
    private float lerpTime;

    private float target;
    private float from;

    private CanvasGroup group;

    private void Start()
    {
        group = GetComponent<CanvasGroup>();
        target = group.alpha;
    }

    private void Update()
    {
        if(group.alpha != target)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, target, elapsedTime / lerpTime);
        }

        group.blocksRaycasts = group.alpha == 1f;
        group.interactable = Interactable && group.alpha == 1f;
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

        // select first element on this form
        Selectable[] selectables = GetComponentsInChildren<Selectable>();
        if (selectables.Length > 0)
        {
            Debug.Log("Selecting: " + selectables[0].name);
            StartCoroutine(waitSelect(selectables[0]));
        }
    }

    IEnumerator waitSelect(Selectable s)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(s.gameObject);
        if (s is Button)
            ((Button)s).Select();
    }

    public void SetAlpha(float a)
    {
        target = a;
        group.alpha = a;
    }
}
