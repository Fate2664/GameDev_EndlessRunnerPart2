using DG.Tweening;
using Nova;
using UnityEngine;

public class DialoguePopup
{
    private UIBlock2D root;
    private float startingWidth;
    private float startingHeight;
    private float endingWidth;
    private float endingHeight;
    private float duration;

    public DialoguePopup(UIBlock2D root, float startingWidth, float startingHeight, float endingWidth, float endingHeight, float duration)
    {
        this.root = root;
        this.startingWidth = startingWidth;
        this.startingHeight = startingHeight;
        this.endingWidth = endingWidth;
        this.endingHeight = endingHeight;
        this.duration = duration;
    }

    public void PopIn()
    {
        if (root != null)
        {
            root.Size.X.Value = startingHeight;
            root.Size.Y.Value = startingWidth;

            DOTween.Kill(root);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => root.Size.X.Value, x => root.Size.X.Value = x, endingWidth, duration));
            sequence.Join(DOTween.To(() => root.Size.Y.Value, y => root.Size.Y.Value = y, endingHeight, duration));

            sequence.SetEase(Ease.OutBack);

        }
    }

    public void PopOut()
    {
        if (root != null)
        {
            root.Size.X.Value = startingHeight;
            root.Size.Y.Value = startingWidth;

            DOTween.Kill(root);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(DOTween.To(() => root.Size.X.Value, x => root.Size.X.Value = x, startingWidth, duration));
            sequence.Join(DOTween.To(() => root.Size.Y.Value, y => root.Size.Y.Value = y, startingHeight, duration));

            sequence.SetEase(Ease.InBack);

        }
    }
}
