using UnityEngine;
using Nova;
using DG.Tweening;
using Interactables;
using System.Collections.Generic;

public class IndicatorManager : MonoBehaviour
{
    //VARIABLES
    [SerializeField] private UIBlock2D indicator;
    [SerializeField] float scaleDuration = 0.5f;

    private List<IInteractable> nearbyInteracts = new List<IInteractable>();

    void Start()
    {
        indicator.transform.localScale = Vector3.zero;
        indicator.transform.DOLocalMoveY(0.05f, 1f).SetLoops(-1, LoopType.Yoyo).From(-0.05f).SetEase(Ease.InOutQuad);
    }

    public void ShowIndicator()
    {
        indicator.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutCubic);
    }

    public void HideIndicator()
    {
        indicator.transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.OutCubic);
    }

    public void AddInteract(IInteractable nearbyInteract)
    {
        nearbyInteracts.Insert(0, nearbyInteract);
    }

    public void RemoveInteract(IInteractable nearbyInteract)
    {
        nearbyInteracts.Remove(nearbyInteract);
    }

    public void InteractNearest()
    {
        if (nearbyInteracts.Count > 0)
        {
            nearbyInteracts[0].OnInteract();
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

}
