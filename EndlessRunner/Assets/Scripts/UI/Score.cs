using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI ValueText;

    private float score;

  
    public void IncrementScore()
    {
        score++;    //increment the score value
        if (score > 0)
        {
            ValueText.text = score.ToString("0");       //Change the text to show the new score
        }
    }

   
}
