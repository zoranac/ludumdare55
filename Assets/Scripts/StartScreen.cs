using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    public CanvasGroup PressToBegin;
    public GameObject Title;
    public Transform TitleEndPos;

    private AsyncOperation loading = null;

    private void Start()
    {
        StartCoroutine(flux());
        StartCoroutine(dropDownTitle());
    }

    public void StartGame()
    {
        if (loading == null)
            loading = SceneManager.LoadSceneAsync(1);
    }

    IEnumerator dropDownTitle()
    {
        float step = 0;
        Vector3 startPoint = Title.transform.position;
        while (true)
        {
            if (step < 2)
            {
                step += Time.deltaTime;
                float t = step / 2;
                Title.transform.position = Vector3.Lerp(startPoint, TitleEndPos.position, t);

                yield return null;
            }
            else
            {
                Title.transform.position = TitleEndPos.position;
                break;
            }
        }
    }

    IEnumerator flux()
    {
        bool up = true;

        float step = 0;

        while (true)
        {
            if (up)
            {
                if (step < 2f)
                {
                    step += Time.deltaTime;
                    float t = step / 2;
                    PressToBegin.alpha = Mathf.Lerp(0, 1, t);

                    yield return null;
                }
                else
                {
                    PressToBegin.alpha = 1;
                    up = !up;
                    step = 0;

                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                if (step < 2f)
                {
                    step += Time.deltaTime;
                    float t = step / 2;
                    PressToBegin.alpha = Mathf.Lerp(1, 0, t);

                    yield return null;
                }
                else
                {
                    PressToBegin.alpha = 0;
                    up = !up;
                    step = 0;

                    yield return new WaitForSeconds(1);
                }
            }
        }
    }
}
