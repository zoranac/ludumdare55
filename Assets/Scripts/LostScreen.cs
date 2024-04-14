using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LostScreen : MonoBehaviour
{
    public TMP_Text PlayerPowerText;
    public TMP_Text WinnerPowerText;

    // Start is called before the first frame update
    void Start()
    {
        PlayerPowerText.text = "Your Power: " + PlayerCharacter.FinalScore.ToString();
        WinnerPowerText.text = "Winner's Power: " + PlayerCharacter.WinnersScore.ToString();
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
