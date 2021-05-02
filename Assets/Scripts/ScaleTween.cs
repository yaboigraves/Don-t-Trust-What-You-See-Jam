using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScaleTween : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 startSize, startPos;
    RectTransform rect;
    public LeanTweenType easeType;
    public AnimationCurve curve;

    public RectTransform startPosition;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.position;

        startSize = rect.localScale;
        // rect.localScale = Vector3.zero;
        rect.position = startPosition.position;


        if (easeType == LeanTweenType.animationCurve)
        {
            LeanTween.moveX(gameObject, startPos.x, 4.5f).setEase(curve);
        }
        else
        {
            LeanTween.moveX(gameObject, startPos.x, 4.5f).setEase(easeType);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
