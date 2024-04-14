using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : Character
{
    bool endingGame = false;

    public FadeButton FadeScreen;

    public bool TriggerEnd = false;

    public static int FinalScore;
    public static int WinnersScore;

    public override void StartTurn()
    {
        if (Hand.Cards.Count > 0)
        {
            CardSelect.Instance.StartSelection();
        }
        else
        {
            Deck.Instance.PullFromDeck(Hand);
        }
    }

    private void Update()
    {
        if (!endingGame)
        {
            if ((Deck.Instance.Cards.Count == 0 && Deck.Instance.Hands.Where(x=>x.Cards.Count > 0).Count() == 0) || TriggerEnd)
            {
                endingGame = true;
                PlayerCharacter.FinalScore = SummoningPower;
                WinnersScore = SummoningPower;
                StartCoroutine(EndGame()); 
            }
        }
    }

    IEnumerator EndGame()
    {
        bool win = true;

        foreach (var character in TurnHandler.Instance.CharacterList)
        {
            if (character.SummoningPower > WinnersScore)
            {
                win = false;
                WinnersScore = character.SummoningPower;
            }
        }

        if (win)
        {
            var op = SceneManager.LoadSceneAsync(2);
            op.allowSceneActivation = false;

            FadeScreen.gameObject.SetActive(true);

            yield return new WaitForSeconds(3.5f);

            op.allowSceneActivation = true;
        }
        else
        {
            var op = SceneManager.LoadSceneAsync(3);
            op.allowSceneActivation = false;

            FadeScreen.gameObject.SetActive(true);

            yield return new WaitForSeconds(3.5f);

            op.allowSceneActivation = true;
        }
    }
}
