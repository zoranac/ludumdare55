using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeButton : MonoBehaviour
{
    public CanvasGroup ButtonGroup;

    public bool FadeInOnAwake = true;

    public float FadeTime = 2f;

    public void Awake()
    {
        if (FadeInOnAwake)
            FadeIn();
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(fadeOut());
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(fade());
    }

    IEnumerator fade()
    {
        float step = 0;

        while (true)
        {
            if (step < FadeTime)
            {
                step += Time.deltaTime;
                float t = step / FadeTime;
                ButtonGroup.alpha = Mathf.Lerp(0, 1, t);

                yield return null;
            }
            else
            {
                ButtonGroup.alpha = 1;
                break;
            }
        }
    }

    IEnumerator fadeOut()
    {
        float step = 0;

        while (true)
        {
            if (step < FadeTime)
            {
                step += Time.deltaTime;
                float t = step / FadeTime;
                ButtonGroup.alpha = Mathf.Lerp(1, 0, t);

                yield return null;
            }
            else
            {
                ButtonGroup.alpha = 0;
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
