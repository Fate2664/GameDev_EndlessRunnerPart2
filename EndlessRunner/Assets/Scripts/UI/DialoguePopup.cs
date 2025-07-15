using DG.Tweening;
using Nova;
using System;
using UnityEngine;

public class DialoguePopup
{
    private UIBlock2D root;
    private Vector3 startingScale;
    private Vector3 endingScale;
    private float duration;

    public DialoguePopup(UIBlock2D root, float duration)
    {
        this.root = root;
        this.startingScale = Vector3.zero;
        this.endingScale = Vector3.one;
        this.duration = duration;
    }

    public void PopIn()
    {
        if (root != null)
        {

            root.transform.localScale = startingScale;

            DOTween.Kill(root.transform);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(root.transform.DOScale(endingScale, duration));

            sequence.SetEase(Ease.OutBack);

        }
    }

    public void PopOut()
    {
        if (root != null && root.gameObject.activeSelf == true)
        {
            DOTween.Kill(root.transform);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(root.transform.DOScale(startingScale, duration));

            sequence.SetEase(Ease.InBack).OnComplete(() =>
            {
                root.gameObject.SetActive(false);
            });
        }
    }
}
