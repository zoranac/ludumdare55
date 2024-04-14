using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TypeWriter : MonoBehaviour
{
    public List<AudioClip> SoundBits = new List<AudioClip>();
    public AudioSource AudioSource;
    public TMP_Text textComponent;

    WaitForSeconds _delayBetweenCharactersYieldInstruction;

    public List<Coroutine> coroutines = new List<Coroutine>();

    public void StartTypeWriterOnText(string stringToDisplay, float delayBetweenCharacters = 0.05f)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        CoroutineBox box = new CoroutineBox();
        IEnumerator coroutine = TypeWriterCoroutine(textComponent, stringToDisplay, delayBetweenCharacters, box);
        box.coroutine = StartCoroutine(coroutine);
    }

    IEnumerator TypeWriterCoroutine(TMP_Text textComponent, string stringToDisplay, float delayBetweenCharacters, CoroutineBox box)
    {
        Coroutine self = box.coroutine;
        coroutines.Add(self);

        while (coroutines[0] != self)
        {
            yield return null;
        }

        // Cache the yield instruction for GC optimization
        _delayBetweenCharactersYieldInstruction = new WaitForSeconds(delayBetweenCharacters);

        bool italics = false;

        int count = 0;

        // Iterating(looping) through the string's characters
        for (int i = 1; i <= stringToDisplay.Length; i++)
        {
            if (stringToDisplay[i-1] == '<')
            {
                if (italics)
                {
                    i += 3;
                }
                else
                {
                    i += 2;
                }

                italics = !italics;
                continue;
            }

            if (i == 1 || stringToDisplay[i - 1] == ' ' || count == 5)
            {
                count = 0;
                AudioSource.clip = SoundBits[Random.Range(0, SoundBits.Count)];
                AudioSource.Play();
            }

            // Retrieves part of the text from string[0] to string[i]
            textComponent.text = stringToDisplay.Substring(0, i);

            if (italics)
            {
                textComponent.text = textComponent.text + "</i>";
            }

            count++;

            // We wait x seconds between characters before displaying them
            yield return _delayBetweenCharactersYieldInstruction;
        }

        yield return new WaitForSeconds(2f);

        coroutines.Remove(self);

        if (coroutines.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }
    
}
