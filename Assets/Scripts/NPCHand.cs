using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPCHand : Hand, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {
        CardSelect.Instance.TargetSelect(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
