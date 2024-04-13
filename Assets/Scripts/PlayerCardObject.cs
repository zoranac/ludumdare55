using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCardObject : CardObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text CountText;

    public GameObject Card;

    private int count = 0;
    public bool selected = false;

    public int Count
    {
        get { return count; }
    }

    public override void Setup(int value)
    {
        base.Setup(value);
        CardImage.sprite = GameEventHandler.Instance.CardFaces[value - 1];
        IncreaseCount();
    }

    public void IncreaseCount()
    {
        count++;
        CountText.text = Count.ToString() + "x";
    }

    public void DecreaseCount()
    {
        count--;
        CountText.text = Count.ToString() + "x";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TurnHandler.Instance.CurrentTurnCharacterIndex == 0 && CardSelect.Instance.selectionMode == SelectionMode.Card)
        {
            selected = true;
            CardSelect.Instance.OnCardSelect(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardSelect.Instance.selectionMode == SelectionMode.Card)
        {
            StopAllCoroutines();
            StartCoroutine(Scale(true));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            StopAllCoroutines();
            StartCoroutine(Scale(false));
        }
    }

    public void ScaleDown()
    {
        StartCoroutine(Scale(false));
    }

    public IEnumerator Scale(bool up)
    {
        float lerpTime = 0f;
        Vector3 start = Card.transform.localPosition;
        Vector3 end = up ? new Vector3(0, 20, 0) : new Vector3(0, 0, 0);

        while (Card.transform.localPosition != end)
        {
            Card.transform.localPosition = Vector3.Lerp(start, end, lerpTime);
            lerpTime += Time.deltaTime * 10;

            yield return null;
        }
    }

   
}
