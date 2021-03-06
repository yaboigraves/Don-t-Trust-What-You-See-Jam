using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum TweenType
{
    Scale,
    Move
}

public class Tweener : MonoBehaviour
{

    public Tween[] tweenSettings;

    private void Start()
    {

        Transform rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = transform;
        }
        Vector3 initialPos = rect.position;
        Vector3 initialScale = rect.localScale;

        foreach (Tween t in tweenSettings)
        {
            t.SetGO(gameObject, initialPos, initialScale);

            if (t.TweenOnStart)
            {
                Debug.Log("Trying to do tween");
                t.DoTween();

                if (t.tweenType == TweenType.Move)
                {
                    rect.position = t.tweenStartPos.position;
                }
                else
                {
                    rect.localScale = Vector3.zero;
                }
            }

            if (t.tweenerName != "")
            {
                TweenManager.current.tweeners.Add(new KeyValuePair<string, Tween>(t.tweenerName, t));
            }
        }
    }
}


//TODO: make it so that this can work with normal transforms too

[System.Serializable]
public class Tween
{
    public string tweenerName = "";
    RectTransform rect;
    public TweenType tweenType;
    public LeanTweenType easeType;
    public AnimationCurve curve;
    public RectTransform tweenStartPos, tweenEndPos;
    Vector3 initialPos, endPos, initialScale;
    public float tweenTime = 1.0f;
    public bool TweenOnStart = false;
    GameObject gO;

    public bool scaleIn, scaleOut;

    public void SetGO(GameObject g, Vector3 initialPos, Vector3 initialScale)
    {
        gO = g;
        this.initialPos = initialPos;
        this.initialScale = initialScale;
        if (tweenEndPos == null)
        {
            this.endPos = initialPos;
        }
        else
        {
            this.endPos = tweenEndPos.position;
        }

    }


    //TODO: make it so that scaleing the tween has an option to either scaleIn or scaleOut with appropriate activation 
    //of the gameobject
    public virtual void DoTween()
    {
        // Debug.Log("doin da tween on " + gO.name + " to " + initialScale.x);

        //curve
        if (easeType == LeanTweenType.animationCurve)
        {
            if (tweenType == TweenType.Move)
            {

                Debug.Log("shmovin " + gO.name);
                LeanTween.move(gO, endPos, tweenTime).setEase(curve);
            }
            else
            {

                if (scaleIn)
                {
                    gO.SetActive(true);
                    gO.transform.localScale = Vector3.zero;
                    LeanTween.scale(gO, initialScale, tweenTime).setEase(curve);
                }
                else if (scaleOut)
                {

                    LeanTween.scale(gO, Vector3.zero, tweenTime).setEase(curve).setOnComplete(() => gO.SetActive(false));
                }
                else
                {
                    LeanTween.scale(gO, initialScale, tweenTime).setEase(curve);
                }

            }
        }
        //preset tween
        else
        {
            if (tweenType == TweenType.Move)
            {
                LeanTween.move(gO, initialPos, tweenTime).setEase(easeType);
            }
            else
            {
                LeanTween.scale(gO, initialScale, tweenTime).setEase(easeType);
            }
        }
    }
}


