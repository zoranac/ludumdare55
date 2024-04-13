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
                StartSelection();
            }
        }
    }

    public void StartSelection()
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
    }

    public void TargetSelect(Hand hand)
    {
        if (selectionMode == SelectionMode.Target)
        {
            hand.SiphonCards(cardSelected.Value, PlayerHand);
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
