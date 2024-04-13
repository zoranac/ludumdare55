using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
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
}
