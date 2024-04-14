using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCHand : Hand, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image OutlineImage;
    public Color HoverColor;

    public void OnPointerClick(PointerEventData eventData)
    {
        CardSelect.Instance.TargetSelect(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OutlineImage.isActiveAndEnabled && CardSelect.Instance.selectionMode == SelectionMode.Target)
        {
            OutlineImage.color = HoverColor;// new Color(0.6132076f, 0.2632165f, 0.5177556f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OutlineImage.isActiveAndEnabled)
        {
            OutlineImage.color = Color.white;
        }
    }
}
