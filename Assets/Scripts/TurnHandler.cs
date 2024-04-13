using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    public int CurrentTurnCharacterIndex;

    public List<Character> CharacterList;

    public static TurnHandler Instance;

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
    }

    public void NextTurn()
    {
        CurrentTurnCharacterIndex++;
        if (CurrentTurnCharacterIndex >= CharacterList.Count)
        {
            CurrentTurnCharacterIndex = 0;
        }

        CharacterList[CurrentTurnCharacterIndex].StartTurn();
    }
   
}
