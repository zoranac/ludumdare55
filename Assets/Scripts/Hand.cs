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


    private void Start()
    {
        StartCoroutine(jiggle());
    }


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
                card.transform.position = Vector3.Lerp(startPoint, moveTo, step);
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
            (Cards.Find(x => x.Value == card.Value) as PlayerCardObject).IncreaseCount();
            Cards.Remove(card);
            Destroy(card.gameObject);
        }
        else
        {
            card.transform.SetParent(CardParent);
        }

        coroutines.Remove(self);

        if (endTurn && coroutines.Count == 0)
        {
            TurnHandler.Instance.NextTurn();
        }
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
                float step = 150 * Time.deltaTime;
                while (Vector3.Distance(movingCard.transform.position, MoveToPosition.position) > 1)
                {
                    movingCard.transform.position = Vector3.MoveTowards(movingCard.transform.position, MoveToPosition.position, step);
                    yield return null;
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

            TurnHandler.Instance.NextTurn();
        }
        else
        {
            toHand.PullFromDeck();
        }

        GameEventHandler.Instance.SiphonAttempt.Invoke(toHand, this, value, cards.Count());
    }


    IEnumerator jiggle()
    {
        bool up = true;
        Vector3 origin = transform.localPosition;
        while (true)
        {
            float lerpTime = 0f;
            Vector3 start = transform.localPosition;
            Vector3 end = up ? origin + new Vector3(0, 20, 0) : origin;

            while (transform.localPosition != end)
            {
                transform.localPosition = Vector3.Lerp(start, end, lerpTime * Mathf.Sin(360 * lerpTime));
                lerpTime += Time.deltaTime;

                yield return null;
            }

            up = !up;
        }
    }

}



class CoroutineBox
{
    public Coroutine coroutine;
}