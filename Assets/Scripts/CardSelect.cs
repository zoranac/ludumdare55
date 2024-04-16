using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum SelectionMode
{
    Card,
    Target,
    None
}

public class CardSelect : MonoBehaviour
{
    public GameObject SelectCardText;
    public GameObject SelectTargetText;

    private bool targetSelected = false;

    public static CardSelect Instance;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }


    public SelectionMode selectionMode = SelectionMode.None;

    public Hand PlayerHand;

    PlayerCardObject cardSelected;

    private void Update()
    {
        if (selectionMode == SelectionMode.Target)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                StartSelection(false);
            }
        }

      
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(12f);

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(12f, 24f));

            if (TurnHandler.Instance.CurrentTurnCharacterIndex == 0 && !targetSelected)
            {
                foreach (var character in TurnHandler.Instance.CharacterList)
                {
                    if (character is PlayerCharacter) continue;

                    int random = Random.Range(0, 6);

                    if (random != 0) continue;

                    (character as NPCCharacter).PlayWaitingText();

                    break;

                }
            }
        }
    }

    public void StartSelection(bool restartWaiting = true)
    {
        if (cardSelected != null)
        {
            cardSelected.ScaleDown();
            cardSelected.selected = false;
        }

        foreach (var hand in Deck.Instance.Hands.Where(x => x is NPCHand))
        {
            (hand as NPCHand).OutlineImage.enabled = false;
        }

        SelectCardText.SetActive(true);
        SelectTargetText.SetActive(false);
        selectionMode = SelectionMode.Card;
        cardSelected = null;
        targetSelected = false;

        if (restartWaiting)
        {
            StopAllCoroutines();
            StartCoroutine(Waiting());
        }
    }

    public void TargetSelect(Hand hand)
    {
        if (selectionMode == SelectionMode.Target && hand.Cards.Count > 0)
        {
            hand.SiphonCards(cardSelected.Value, PlayerHand);
            targetSelected = true;
            Close();
        }
    }

    public void OnCardSelect(PlayerCardObject card)
    {
        if (selectionMode == SelectionMode.Card)
        {
            SelectCardText.SetActive(false);
            SelectTargetText.SetActive(true);
            cardSelected = card;
            cardSelected.selected = true;
            selectionMode = SelectionMode.Target;

            foreach (var hand in Deck.Instance.Hands.Where(x => x is NPCHand))
            {
                if (hand.Cards.Count() <= 0) continue;

                (hand as NPCHand).OutlineImage.enabled = true;
                (hand as NPCHand).OutlineImage.color = Color.white;
            }

        }
    }

    public void Close()
    {
        if (cardSelected != null)
        {
            cardSelected.ScaleDown();
            cardSelected.selected = false;
        }

        foreach (var hand in Deck.Instance.Hands.Where(x => x is NPCHand))
        {
            (hand as NPCHand).OutlineImage.enabled = false;
            
        }

        SelectCardText.SetActive(false);
        SelectTargetText.SetActive(false);
        selectionMode = SelectionMode.None;
        cardSelected = null;
    }


}
