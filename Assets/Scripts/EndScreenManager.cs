using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public TypeWriter typeWriter;

    [TextArea]
    [SerializeField]
    List<string> DialogLines = new List<string>();

    [TextArea]
    [SerializeField]
    List<string> WaitingLines = new List<string>();

    public GameObject ButtonsHolder;

    public TMP_Text FinalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        FinalScoreText.text = "Final Power: " + PlayerCharacter.FinalScore.ToString();
        StartCoroutine(EndingSequence());
    }


    IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(5f);

       for (int i = 0; i < DialogLines.Count; i++) 
        {
            if (i == DialogLines.Count - 1)
            {
                ButtonsHolder.SetActive(true);
            }

            typeWriter.StartTypeWriterOnText(DialogLines[i], .1f);
            yield return new WaitForSeconds(5f);
        }

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            typeWriter.StartTypeWriterOnText(WaitingLines[Random.Range(0, WaitingLines.Count)], .1f);
            yield return new WaitForSeconds(5f);
        }

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(1);
    }


}
