using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Hand Hand;

    public int SummoningPower = 0;
    public abstract void StartTurn();
}
