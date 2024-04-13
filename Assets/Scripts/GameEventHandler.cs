using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventHandler : MonoBehaviour
{
    public static GameEventHandler Instance;

    [HideInInspector]
    public UnityEvent<Hand, Hand, int, int> SiphonAttempt = new UnityEvent<Hand, Hand, int, int>();

    public List<Sprite> CardFaces = new List<Sprite>();

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

   

}
