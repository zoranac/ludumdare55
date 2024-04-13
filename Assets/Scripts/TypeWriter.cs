using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypeWriter : MonoBehaviour
{

    public TMP_Text textComponent;

    WaitForSeconds _delayBetweenCharactersYieldInstruction;

    public void StartTypeWriterOnText(string stringToDisplay, float delayBetweenCharacters = 0.05f)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        StartCoroutine(TypeWriterCoroutine(textComponent, stringToDisplay, delayBetweenCharacters));
    }

    IEnumerator TypeWriterCoroutine(TMP_Text textComponent, string stringToDisplay, float delayBetweenCharacters)
    {
        // Cache the yield instruction for GC optimization
        _delayBetweenCharactersYieldInstruction = new WaitForSeconds(delayBetweenCharacters);

        bool italics = false;

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

        

            // Retrieves part of the text from string[0] to string[i]
            textComponent.text = stringToDisplay.Substring(0, i);

            if (italics)
            {
                textComponent.text = textComponent.text + "</i>";
            }

            // We wait x seconds between characters before displaying them
            yield return _delayBetweenCharactersYieldInstruction;
        }

        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
    
}
