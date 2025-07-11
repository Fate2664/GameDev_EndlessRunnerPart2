using Nova;
using UnityEngine;
using NovaSamples.UIControls;

public class PopinUI : MonoBehaviour
{
    private UIBlock2D UIBlock;
    private Button novaButton;
    private void Awake()
    {
        UIBlock = GetComponent<UIBlock2D>();
        novaButton = GetComponent<NovaSamples.UIControls.Button>();

        if (novaButton != null)
        {
            novaButton.OnHover.AddListener(() =>
            {
                if (UIBlock != null)
                {
                    UIBlock.BodyEnabled = true;
                }
            });
        }

        novaButton.OnUnhover.AddListener(() =>
        {
            if (UIBlock != null)
            {
                UIBlock.BodyEnabled = false;
            }
        });
    }
}
