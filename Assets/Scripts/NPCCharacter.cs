using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCCharacter : Character
{
    public List<NPCMemory> Memory = new List<NPCMemory>();

    public int MemoryLength = 100;

    public void Start()
    {
        GameEventHandler.Instance.SiphonAttempt.AddListener(addToMemory);
    }

    public override void StartTurn()
    {
        StartCoroutine(assessTarget());
    }

    private void addToMemory(Hand character, Hand target, int value, int count)
    {
        Memory.Add(new NPCMemory(character, target, value, count));

        if (Memory.Count > MemoryLength)
        {
            Memory.RemoveAt(0);
        }
    }

    private IEnumerator assessTarget()
    {
        yield return new WaitForSeconds(1);

        var countedCards = countCards().OrderByDescending(x=>x.Value).ToList();

        List<CardTarget> TargetOptions = new List<CardTarget>(); 
        List<CardTarget> SkipTargetOptions = new List<CardTarget>();    

        foreach (var memory in Memory) { 
            if (memory.SiphoningHand != this && countedCards.Exists(x=>x.Key == memory.Value))
            {
                TargetOptions.Add(new CardTarget(memory.SiphoningHand, memory.Value, memory.Value+memory.Count+1));
            }
            
            if (memory.TargetHand != this && memory.Count == 0 && countedCards.Exists(x => x.Key == memory.Value))
            {
                SkipTargetOptions.Add(new CardTarget(memory.TargetHand, memory.Value, 0));
            }
        }

        foreach (var card in countedCards)
        {
            foreach (var character in TurnHandler.Instance.CharacterList)
            {
                if (character == this ) continue;

                if (SkipTargetOptions.Exists(x => x.Target == character && x.CardValue == card.Key)) continue;

                TargetOptions.Add(new CardTarget(character.Hand, card.Key, card.Key + 1));
            }
        }

        int index = GetRandomWeightedIndex(TargetOptions.Select(x => x.Weight).ToArray());

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