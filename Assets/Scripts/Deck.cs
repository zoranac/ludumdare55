using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Deck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<int> Cards = new List<int>();

    public List<Hand> Hands = new List<Hand>();

    public static Deck Instance;

    public bool GameStart = false;

    public Image BackgroundImage;
    
    public List<Sprite> BackgroundStages = new List<Sprite>();

    public TMP_Text NumberOfCardsText;

    public TypeWriter DeckTextbox;

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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 1; i <= 13; i++) {
            Cards.Add(i);
            Cards.Add(i);
            Cards.Add(i);
            Cards.Add(i);
        }

        DealHands();
    }

    public void DealHands()
    {
        foreach (Hand hand in Hands)
        {
            for(int i = 0; i < 5 ; i++)
            {
                int random = Random.Range(0, Cards.Count);

                hand.GainCard(Cards[random]);

                Cards.RemoveAt(random);
            }
        }

        StartCoroutine(StartGameAfterDeal());
    }

    public IEnumerator StartGameAfterDeal()
    {
        while (true)
        {
            if (Hands[0].coroutines.Count == 0)
            {
                yield return new WaitForSeconds(.1f);

                if (Hands[0].coroutines.Count == 0)
                {
                    break;
                }
            }
            yield return null;
        }

        GameStart = true;
        TurnHandler.Instance.NextTurn();
        
    }

    public void PullFromDeck(Hand toHand)
    {
        if (Cards.Count > 0)
        {
            int randomIndex = Random.Range(0, Cards.Count);

            toHand.GainCard(Cards[randomIndex], true);

            Cards.RemoveAt(randomIndex);

            NumberOfCardsText.text = Cards.Count.ToString();

            if (Cards.Count == 16)
            {
                BackgroundImage.sprite = BackgroundStages[1];
            }
            else if (Cards.Count == 8)
            {
                BackgroundImage.sprite = BackgroundStages[2];
            }
            else if (Cards.Count == 4)
            {
                BackgroundImage.sprite = BackgroundStages[3];
            }
            else if (Cards.Count == 1)
            {
                BackgroundImage.sprite = BackgroundStages[4];
            }
            else if (Cards.Count == 0)
            {
                BackgroundImage.sprite = BackgroundStages[5];
            }
        }
        else
        {
            if (toHand.Cards.Count > 0)
            {
                DeckTextbox.StartTypeWriterOnText("There are no more cards, use what you have to summon me.");
            }
            else
            {
                DeckTextbox.StartTypeWriterOnText("You fool, you have nothing left to summon me.");
            }

            StartCoroutine(delayNextTurn());
        }
    }

    IEnumerator delayNextTurn()
    {
        yield return new WaitForSeconds(2.5f);

        TurnHandler.Instance.NextTurn();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        NumberOfCardsText.transform.parent.gameObject.SetActive(true);
        NumberOfCardsText.text = Cards.Count.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NumberOfCardsText.transform.parent.gameObject.SetActive(false);

    }
}
