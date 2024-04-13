using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public Hand Hand;

    public int SummoningPower = 0;

    public TMP_Text SummonScoreText;
    public TypeWriter CharacterTextBox;

    public void AddSummonPower(int power)
    {
        SummoningPower += power;
        SummonScoreText.text = SummoningPower.ToString();
    }

    public abstract void StartTurn();
}
