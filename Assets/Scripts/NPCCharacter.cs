using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class NPCCharacter : Character
{
    public List<NPCMemory> Memory = new List<NPCMemory>();

    public Animator animator;

    [TextArea]
    public List<string> SiphoningTexts = new List<string>();

    [TextArea]
    public List<string> SiphonedFromTexts = new List<string>();

    [TextArea]
    public List<string> MissedMeTexts = new List<string>();

    [TextArea]
    public List<string> MissedTexts = new List<string>();

    [TextArea]
    public List<string> PointsScoredTexts = new List<string>();

    [TextArea]
    public List<string> EnemyScoredTexts = new List<string>();

    [TextArea]
    public List<string> GameStartTexts = new List<string>();

    [TextArea]
    public List<string> WaitingTexts = new List<string>();

    public int MemoryLength = 100;


    private bool speaking = false;

    public void Start()
    {
        GameEventHandler.Instance.SiphonAttempt.AddListener(addToMemory);
        GameEventHandler.Instance.CardsBurned.AddListener(cardsBurned);
    }

    public override void StartTurn()
    {
        if (Hand.Cards.Count > 0)
        {
            StartCoroutine(assessTarget());
        }
        else
        {
            Deck.Instance.PullFromDeck(Hand);
        }
    }

    public void PlayWaitingText()
    {
        if (!speaking)
        {
            StartCoroutine(sayWaitingLine());
        }
    }
    public void PlayStartGameText()
    {
        if (!speaking)
        {
            StartCoroutine(sayStartGameText());
        }
    }

    private IEnumerator sayStartGameText()
    {
        speaking = true;

        CharacterTextBox.StartTypeWriterOnText(GameStartTexts[UnityEngine.Random.Range(0, GameStartTexts.Count)]);

        yield return new WaitForSeconds(3f);

        speaking = false;
    }


    private IEnumerator sayWaitingLine()
    {
        speaking = true;

        CharacterTextBox.StartTypeWriterOnText(WaitingTexts[UnityEngine.Random.Range(0, WaitingTexts.Count)]);

        yield return new WaitForSeconds(3f);

        speaking = false;
    }


    private IEnumerator saySiphonedFromMeLine(int value, int index)
    {
        speaking = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(.25f, 1f));

        animator.SetInteger("Target", index);
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(SiphonedFromTexts[UnityEngine.Random.Range(0, SiphonedFromTexts.Count)].Replace("#", valueToRoman(value)));

        yield return new WaitForSeconds(3f);

        speaking = false;
    }

    private IEnumerator sayMissMeLine(int value, int index)
    {
        speaking = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(.25f, 1f));

        animator.SetInteger("Target", index);
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(MissedMeTexts[UnityEngine.Random.Range(0, MissedMeTexts.Count)].Replace("#", valueToRoman(value)));

        yield return new WaitForSeconds(2.5f);

        speaking = false;
    }

    private IEnumerator sayMissedLine(int value, int index)
    {
        speaking = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2.5f));

        animator.SetInteger("Target", index);
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(MissedTexts[UnityEngine.Random.Range(0, MissedTexts.Count)].Replace("#", valueToRoman(value)));

        yield return new WaitForSeconds(2.5f);

        speaking = false;
    }

    private IEnumerator sayPointsScoredLine(int value, int index)
    {
        speaking = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(.25f, 1f));

        animator.SetInteger("Target", index);
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(PointsScoredTexts[UnityEngine.Random.Range(0, PointsScoredTexts.Count)].Replace("#", valueToRoman(value)));

        yield return new WaitForSeconds(2.5f);

        speaking = false;
    }

    private IEnumerator sayEnemyScoredLine(int value, int index)
    {
        speaking = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(1f,2.5f));

        animator.SetInteger("Target", index);
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(EnemyScoredTexts[UnityEngine.Random.Range(0, EnemyScoredTexts.Count)].Replace("#", valueToRoman(value)));

        yield return new WaitForSeconds(2.5f);

        speaking = false;
    }

    private void cardsBurned(int card, Hand hand)
    {
        if (speaking) return;

        int random = UnityEngine.Random.Range(0, 6);

        if (random != 0) return;

        if (hand == Hand)
        {
            speaking = true;
            StartCoroutine(sayPointsScoredLine(card, -1));
        }
        else
        {
            speaking = true;

            StartCoroutine(sayEnemyScoredLine(card, Deck.Instance.Hands.IndexOf(hand)));
        }
    }


    private void addToMemory(Hand character, Hand target, int value, int count)
    {
        Memory.Add(new NPCMemory(character, target, value, count));

        if (Memory.Count > MemoryLength)
        {
            Memory.RemoveAt(0);
        }


        if (speaking) return;
        int random = UnityEngine.Random.Range(0, 6);

        if (random != 0) return;

        if (target == Hand && count == 0)
        {
            speaking = true;

            StartCoroutine(sayMissMeLine(value, Deck.Instance.Hands.IndexOf(character)));
        }
        else if (character == Hand && count == 0)
        {
            speaking = true;

            StartCoroutine(sayMissedLine(value, Deck.Instance.Hands.IndexOf(character)));
        }
        else if (target == Hand && count > 0)
        {
            speaking = true;

            StartCoroutine(saySiphonedFromMeLine(value, Deck.Instance.Hands.IndexOf(character)));
        }
    }

    private IEnumerator assessTarget()
    {
        yield return new WaitForSeconds(1);

        var countedCards = countCards().OrderByDescending(x=>x.Value).ToList();

        List<CardTarget> TargetOptions = new List<CardTarget>(); 
        List<CardTarget> SkipTargetOptions = new List<CardTarget>();    

        foreach (var memory in Memory) { 
            if (memory.SiphoningHand != Hand && countedCards.Exists(x=>x.Key == memory.Value) && memory.SiphoningHand.Cards.Count > 0)
            {
                TargetOptions.Add(new CardTarget(memory.SiphoningHand, memory.Value, Mathf.RoundToInt(memory.Value/2)+((memory.Count + 1) * 2)));
            }
            
            if (memory.TargetHand != Hand && memory.Count == 0 && countedCards.Exists(x => x.Key == memory.Value))
            {
                SkipTargetOptions.Add(new CardTarget(memory.TargetHand, Mathf.RoundToInt(memory.Value / 2), 0));
            }
        }

        foreach (var card in countedCards)
        {
            foreach (var character in TurnHandler.Instance.CharacterList)
            {
                if (character == this ) continue;

                if (character.Hand.Cards.Count <= 0) continue;

                if (SkipTargetOptions.Exists(x => x.Target == character && x.CardValue == card.Key)) continue;

                TargetOptions.Add(new CardTarget(character.Hand, card.Key, card.Key + 1));
            }
        }

        int index = GetRandomWeightedIndex(TargetOptions.Select(x => x.Weight).ToArray());


        while (speaking) {
            yield return null;
        }

        speaking = true;


        animator.SetInteger("Target", Deck.Instance.Hands.IndexOf(TargetOptions[index].Target));
        animator.SetTrigger("Point");

        CharacterTextBox.StartTypeWriterOnText(SiphoningTexts[UnityEngine.Random.Range(0, SiphoningTexts.Count)].Replace("#", valueToRoman(TargetOptions[index].CardValue)));

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        speaking = false;

        TargetOptions[index].Target.SiphonCards(TargetOptions[index].CardValue, Hand);
    }
    private Dictionary<int, int> countCards()
    {
        var returnDict = new Dictionary<int, int>();

        foreach (CardObject card in Hand.Cards)
        {
            if (!returnDict.ContainsKey(card.Value))
            {
                returnDict.Add(card.Value, 1);
            }
            else
            {
                returnDict[card.Value]++;
            }
        }

        return returnDict;

    }

    private int GetRandomWeightedIndex(int[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = UnityEngine.Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }

    private string valueToRoman(int value)
    {
        return value.ToString();

        //switch (value)
        //{
        //    case 1: return "I";
        //    case 2: return "II";
        //    case 3: return "III";
        //    case 4: return "IV";
        //    case 5: return "V";
        //    case 6: return "VI";
        //    case 7: return "VII";
        //    case 8: return "VIII";
        //    case 9: return "IX";
        //    case 10: return "X";
        //    case 11: return "XI";
        //    case 12: return "XII";
        //    case 13: return "XIII";
        //    default:
        //        break;
        //}
        //return "";
    }

}

public class CardTarget
{
    public Hand Target;
    public int CardValue;
    public int Weight;

    public CardTarget(Hand target, int cardValue, int weight)
    {
        Target = target;
        CardValue = cardValue;
        Weight = weight;
    }
}