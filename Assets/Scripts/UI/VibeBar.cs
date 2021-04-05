using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VibeBar : MonoBehaviour
{
    public Gradient vibeBarZonesGradient;
    public Image fillArea;


    public void EvaluateFillAreaColor(float value)
    {
        fillArea.color = vibeBarZonesGradient.Evaluate(value);
    }


}
