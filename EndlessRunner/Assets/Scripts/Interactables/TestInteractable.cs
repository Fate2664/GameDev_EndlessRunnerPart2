using Interactables;
using UnityEngine;
using UnityEngine.Events;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] UnityEvent interactTrigger;
    [SerializeField] IndicatorManager indicatorManager;

    public void OnInteract()
    {
        interactTrigger.Invoke();
    }

    //TEST METHODS
    public void Message1()
    {
        Debug.Log("Message 1");
    }

    public void Message2()
    {
        Debug.Log("Message 2");
    }

    //COLLISION
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player Hitbox")
        {
            indicatorManager.AddInteract(this);
            indicatorManager.ShowIndicator();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player Hitbox")
        {
            indicatorManager.RemoveInteract(this);
            indicatorManager.HideIndicator();
        }
    }
}
