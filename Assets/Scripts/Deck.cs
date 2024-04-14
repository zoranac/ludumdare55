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

    bool noCardsInHandTextShown = false;
    bool noCardsInDeckTextShown = false;
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
        yield return new WaitForSeconds(Random.Range(2f, 6f));

        foreach (var character in TurnHandler.Instance.CharacterList)
        {
            if (character is PlayerCharacter) continue;

            int random = Random.Range(0, 4);

            if (random != 0) continue;

            (character as NPCCharacter).PlayStartGameText();

            break;

        }


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
            if (TurnHandler.Instance.CurrentTurnCharacterIndex == 0)
            {
                if (toHand.Cards.Count > 0)
                {
                    if (!noCardsInDeckTextShown)
                    {
                        noCardsInDeckTextShown = true;
                        DeckTextbox.StartTypeWriterOnText("There Are No More Cards, Use What You Have To Summon Me");
                    }
                    else
                    {
                        int rand = Random.Range(0, 3);
                        if (rand == 0)
                        {
                            DeckTextbox.StartTypeWriterOnText("Nothing");
                        }
                        else if (rand == 1)
                        {
                            DeckTextbox.StartTypeWriterOnText("Missed");
                        }
                        else if (rand == 2)
                        {
                            DeckTextbox.StartTypeWriterOnText("Empty");
                        }
                    }
                }
                else if (!noCardsInHandTextShown)
                {
                    noCardsInHandTextShown = true;
                    DeckTextbox.StartTypeWriterOnText("You Fool, You Have Nothing Left To Summon Me");
                }

                StartCoroutine(delayNextTurn());
            }
            else
            {
                TurnHandler.Instance.NextTurn();
            }
        }
    }

    IEnumerator delayNextTurn()
    {
        yield return new WaitForSeconds(3f);

        TurnHandler.Instance.NextTurn();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        NumberOfCardsText.transform.parent.gameObject.SetActive(true);
        NumberOfCardsText.transform.parent.GetComponent<FadeButton>().FadeIn();
        NumberOfCardsText.text = Cards.Count.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NumberOfCardsText.transform.parent.GetComponent<FadeButton>().FadeOut();//.gameObject.SetActive(false);

    }
}
