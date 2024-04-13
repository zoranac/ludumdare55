using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    public int CurrentTurnCharacterIndex;

    public List<Character> CharacterList;

    public static TurnHandler Instance;

    public Animator LazerAnim;

    private bool delay;

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
        CurrentTurnCharacterIndex = -1;

        GameEventHandler.Instance.SiphonAttempt.AddListener(LazerCheck);
    }

    private void LazerCheck(Hand siphoner, Hand target, int value, int count)
    {
        if (count == 0)
        {
            delay = true;
            LazerAnim.SetInteger("Target", Deck.Instance.Hands.FindIndex(x => x == siphoner));
            LazerAnim.SetTrigger("Fire");

        }
    }

    public void NextTurn()
    {
        if (delay)
        {
            StartCoroutine(waitForDelay());
            return;
        }

        CurrentTurnCharacterIndex++;
        if (CurrentTurnCharacterIndex >= CharacterList.Count)
        {
            CurrentTurnCharacterIndex = 0;
        }

        CharacterList[CurrentTurnCharacterIndex].StartTurn();
    }
   
    IEnumerator waitForDelay()
    {
        while (!LazerAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }

        LazerAnim.SetInteger("Target", -1);
        delay = false;

        NextTurn();
    }

}
