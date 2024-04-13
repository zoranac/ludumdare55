using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<int> Cards = new List<int>();

    public List<Hand> Hands = new List<Hand>();

    public static Deck Instance;

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

        TurnHandler.Instance.NextTurn();
        
    }

    public void PullFromDeck(Hand toHand)
    {
        toHand.GainCard(Cards[Random.Range(0, Cards.Count)], true);
    }
}
