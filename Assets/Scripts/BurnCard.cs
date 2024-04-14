using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurnCard : MonoBehaviour
{

    public Image image;
    public void HideImage()
    {
        gameObject.SetActive(false);
        //image.enabled = false;
    }
}
