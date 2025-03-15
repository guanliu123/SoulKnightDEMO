using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupNum : Item
{
    private CanvasGroup canvasGroup;
    private TMP_Text text;
    
    public PopupNum(GameObject obj) : base(obj)
    {
        canvasGroup = obj.GetComponent<CanvasGroup>();
        text = obj.GetComponentInChildren<TMP_Text>();
    }

    public void SetText(int damage)
    {
        text.text = "-"+damage;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        text.color = Color.red;
        DoAnimation();
    }

    private void DoAnimation()
    {
        canvasGroup.alpha = 0;
        canvasGroup.transform.localScale = Vector3.one*0.5f;
        transform.position=transform.position+Vector3.left*Random.Range(-1f,1f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(canvasGroup.DOFade(1, 0.2f));
        sequence.Join(canvasGroup.transform.DOMoveY(transform.position.y+Random.Range(1f,1.5f), 0.2f));
        sequence.Join(canvasGroup.transform.DOScale(Vector3.one, 0.2f));
        sequence.OnComplete(() =>
        {
            Remove();
            ObjectPoolManager.Instance.GetPool(PoolName).DeSpawn(gameObject,PoolName);
        });
        sequence.Play();
    }
}
