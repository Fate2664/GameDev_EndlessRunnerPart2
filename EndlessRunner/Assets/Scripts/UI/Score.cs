using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    //This script manages the scoring of the player

    public TextMeshProUGUI ValueText;

    private float score;

    public bool DoublePointsActive;


    public void IncrementScore()
    {
        if (!DoublePointsActive)
        {
            score++;    //increment the score value
        }

        if (DoublePointsActive)
        {
            score += 2;    //increment the score value 
        }


        ValueText.text = score.ToString("0");       //Change the text to show the new score

    }


}
