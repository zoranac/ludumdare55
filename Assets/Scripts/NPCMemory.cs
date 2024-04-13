using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCMemory
{
    public Hand SiphoningHand;
    public Hand TargetHand;
    public int Value;
    public int Count;

    public NPCMemory(Hand character, Hand target, int value, int count)
    {
        SiphoningHand = character;
        TargetHand = target;
        Value = value;
        Count = count;
    }
}
