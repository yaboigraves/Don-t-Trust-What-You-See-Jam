using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectRotate : MonoBehaviour
{

    //TODO: sync this to bpm
    RectTransform rectTransform;
    public Vector3 rotation;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.Rotate(rotation * Time.deltaTime);
    }
}
