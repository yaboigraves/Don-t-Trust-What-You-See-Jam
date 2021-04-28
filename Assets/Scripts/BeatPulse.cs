using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatPulse : MonoBehaviour
{
    //why must i do this :3
   

    bool pulsing = false;

    public float pulseAmount = 1.5f;

    public float maxPulseFadeTime = 3f;
    float pulseFadeTime = 0;

    Vector3 startingScale,pulsedScale;







    RectTransform rectTransform;

    
    private void Start() {
        UIManager.current.pulseBois.Add(this);

        rectTransform = GetComponent<RectTransform>();

        if(rectTransform != null){
            startingScale = rectTransform.localScale;
        }
        else{
            startingScale = transform.localScale;
        }

        pulsedScale = startingScale * pulseAmount;
        pulsedScale.z = startingScale.z;
    }

    public void Pulse(){
        //so depending on if we have a recttransform or a normal transform, just mark that we're pulsing 
        pulsing = true;
        pulseFadeTime = 0f;
    }

    private void Update() {
        if(pulsing){
            
            //lerp the starting scale to the pulsed scale

            float lerpProgress = pulseFadeTime/maxPulseFadeTime;

            if(rectTransform != null){
                rectTransform.localScale = Vector3.Lerp(pulsedScale,startingScale,lerpProgress );
            }
            else{
                transform.localScale = Vector3.Lerp(pulsedScale,startingScale,lerpProgress );
            }


            pulseFadeTime += Time.deltaTime;

            if(pulseFadeTime >= maxPulseFadeTime){
                pulsing = false;
                pulseFadeTime = 0;
            }
        }
       
    }


}
