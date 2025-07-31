using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionModule : MonoBehaviour
{
    [SerializeField] private IndicatorManager indicatorManager;
    public void Interact(InputAction.CallbackContext context)
    {
        //ATTEMPT INTERACT
        if (context.performed)
        {
            indicatorManager.InteractNearest();
        }
    }
}
