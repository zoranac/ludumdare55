using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{
    public int Value;

    public Image CardImage;
    public Sprite CardBack;

    public Animator animator;

    public UnityEvent<CardObject> OnDestroy = new UnityEvent<CardObject> ();

    private void Start()
    {
        //StartCoroutine(jiggle());

        CardImage.transform.position = new Vector3(CardImage.transform.position.x, Random.Range(-5,10) + CardImage.transform.position.y, CardImage.transform.position.z);
    }


    public virtual void Setup(int value)
    {
        Value = value;
        CardImage.sprite = CardBack;
    }

    public void Burn()
    {
        transform.SetParent(transform.parent.parent);

        CardImage.gameObject.SetActive(false);
        animator.gameObject.SetActive(true);

        StartCoroutine(awaitDestruction());
    }

    private IEnumerator awaitDestruction()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null;
        }

        OnDestroy.Invoke(this);

        Destroy(gameObject);
    }

    IEnumerator jiggle()
    {
        while (!Deck.Instance.GameStart && transform.parent.GetComponent<Hand>() == null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(.5f, 2f));

        bool up = true;
        Vector3 origin = CardImage.transform.localPosition;
        while (true)
        {
            if (transform.parent.GetComponent<Hand>() == null)
                continue;

            float lerpTime = 0f;
            Vector3 start = CardImage.transform.localPosition;
            Vector3 end = up ? origin + new Vector3(0, Random.Range(5f,10f), 0) : origin;

            while (CardImage.transform.localPosition != end)
            {
                if (CardImage.transform.parent.GetComponent<Hand>() == null)
                    break;

                CardImage.transform.localPosition = Vector3.Lerp(start, end, lerpTime);
                lerpTime += Time.deltaTime;

                yield return null;
            }

            up = !up;

            yield return new WaitForSeconds(Random.Range(.5f, 2f));
        }
    }
}
