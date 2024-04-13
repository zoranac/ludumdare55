using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.XR;

public class Hand : MonoBehaviour
{
    public List<CardObject> Cards = new List<CardObject>();

    public CardObject CardPrefab;
    public Transform CardParent;

    public Transform MoveToPosition;

    public List<Coroutine> coroutines = new List<Coroutine>();

    public bool HorizontalMove = false;

    public Character Owner;

    public void GainCard(int card, bool endTurn = false)
    {
        var g = Instantiate(CardPrefab, MoveToPosition);
        g.Setup(card);
        Cards.Add(g);
        CoroutineBox box = new CoroutineBox();
        IEnumerator coroutine = addCard(g, endTurn, box);
        box.coroutine = StartCoroutine(coroutine);
    }

    public void SiphonCards(int value, Hand toHand)
    {
        Debug.Log(toHand.gameObject.name + " Siphoning From " + gameObject.name);
        StartCoroutine(removeCards(Cards.Where(x => x.Value == value).ToList(), value, toHand));
    }

    public void PullFromDeck()
    {
        Deck.Instance.PullFromDeck(this);
    }

    private IEnumerator addCard(CardObject card, bool endTurn, CoroutineBox box)
    {
        yield return null;
        Coroutine self = box.coroutine;
        coroutines.Add(self);

        while (coroutines[0] != self)
        {
            yield return null;
        }

        Debug.Log("Adding Card " + card.Value + " To " + gameObject.name);

        card.transform.SetParent(transform.parent);
        card.transform.position = MoveToPosition.position;

        Vector3 moveTo = transform.position;
        bool cardExists = false;


        if (CardPrefab is PlayerCardObject && Cards.Exists(x => x.Value == card.Value && x != card))
        {
            cardExists = true;
            moveTo = Cards.Find(x => x.Value == card.Value && x != card).transform.position;
        }


        float step = 0;
        //Vector3 dir = Vector3.Cross(MoveToPosition.position, moveTo);
        //Vector3 controlPoint = MoveToPosition.position + (moveTo - MoveToPosition.position) / 2 + dir.normalized*50;
        Vector3 startPoint = new Vector3(moveTo.x, card.transform.position.y);

        while (true)
        {
            if (step < 1f)
            {
                step += Time.deltaTime;
                float t = step / 1f;
                //Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, step);
                //Vector3 m2 = Vector3.Lerp(controlPoint, moveTo, step);
                //card.transform.position = Vector3.Lerp(m1, m2, step);
                card.transform.position = Vector3.Lerp(startPoint, moveTo, t);
                yield return null;
            }
            else
            {
                card.transform.position = moveTo;
                break;
            }
        }

        if (cardExists)
        {
            Cards.Remove(card);

            var currentCard = (Cards.Find(x => x.Value == card.Value) as PlayerCardObject);

            currentCard.IncreaseCount();
            Destroy(card.gameObject);

            //Burn Check
            if (currentCard.Count == 4)
            {
                Owner.AddSummonPower(currentCard.Value);
                GameEventHandler.Instance.CardsBurned.Invoke(currentCard.Value, this);
                currentCard.Burn();
            }
        }
        else
        {
            card.transform.SetParent(CardParent);
            card.OnDestroy.AddListener(removeCard);

            //Burn Check
            if (Cards.Where(x => x.Value == card.Value).Count() == 4 )
            {
                Owner.AddSummonPower(card.Value);
                GameEventHandler.Instance.CardsBurned.Invoke(card.Value, this);
                foreach (var item in Cards.Where(x => x.Value == card.Value))
                {
                    item.Burn();
                }

               
            }
        }

        coroutines.Remove(self);

        if (endTurn && coroutines.Count == 0)
        {
            TurnHandler.Instance.NextTurn();
        }
    }

    private void removeCard(CardObject card)
    {
        Cards.Remove(card);
        card.OnDestroy?.RemoveListener(removeCard);
    }

    private IEnumerator removeCards(List<CardObject> cards, int value, Hand toHand = null)
    {
        if (cards.Count > 0)
        {
            foreach (CardObject card in cards)
            {
                var movingCard = card;

                if (CardPrefab is PlayerCardObject && card != null && (card as PlayerCardObject).Count > 10)
                {
                    (card as PlayerCardObject).DecreaseCount();
                    movingCard = Instantiate(CardPrefab, transform);
                    movingCard.transform.localPosition = card.transform.position;
                }

                movingCard.transform.SetParent(transform.parent);
                //Vector2 controlPoint = card.transform.position + (MoveToPosition - card.transform.position) / 2 + Vector3.up * 5.0f;
                //Vector2 spawnPos = MoveToPosition.position; //new Vector2(GetComponent<RectTransform>().rect.xMax, MoveToPosition.position.y);
                float step = 0;

                Vector3 moveTo = HorizontalMove ?
                    new Vector3(MoveToPosition.position.x, movingCard.transform.position.y, movingCard.transform.position.z)
                    : new Vector3(movingCard.transform.position.x, MoveToPosition.position.y, movingCard.transform.position.z);

                if (step < 1f)
                {
                    step += Time.deltaTime;
                    float t = step / 2f;
                    //Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, step);
                    //Vector3 m2 = Vector3.Lerp(controlPoint, moveTo, step);
                    //card.transform.position = Vector3.Lerp(m1, m2, step);
                    card.transform.position = Vector3.Lerp(movingCard.transform.position, moveTo, t);
                    yield return null;
                }
                else
                {
                    card.transform.position = moveTo;
                    break;
                }

                if (movingCard == card)
                {
                    Cards.Remove(card);
                    Destroy(card.gameObject);
                }

                Debug.Log("Removing Card " + card.Value + " From " + gameObject.name);

                if (toHand != null)
                {
                    toHand.GainCard(value, false);
                }
            }

            //After all the cards have been removed, wait for coroutines to finish before passing the turn
            while (true)
            {
                if (toHand.coroutines.Count == 0)
                {
                    yield return new WaitForSeconds(.1f);

                    if (toHand.coroutines.Count == 0)
                    {
                        break;
                    }
                }
                yield return null;
            }

            GameEventHandler.Instance.SiphonAttempt.Invoke(toHand, this, value, cards.Count());

            TurnHandler.Instance.NextTurn();
        }
        else
        {
            GameEventHandler.Instance.SiphonAttempt.Invoke(toHand, this, value, cards.Count());

            toHand.PullFromDeck();
        }
    }
}



class CoroutineBox
{
    public Coroutine coroutine;
}