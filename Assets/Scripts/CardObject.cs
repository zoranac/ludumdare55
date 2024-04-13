using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public int Value;

    public Image CardImage;
    public Sprite CardBack;

    public virtual void Setup(int value)
    {
        Value = value;
        CardImage.sprite = CardBack;
    }
}
