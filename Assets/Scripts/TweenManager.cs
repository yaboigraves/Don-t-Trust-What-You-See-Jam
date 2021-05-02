using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class is basically just a chill singleton that helps functions invoke tweens by a simple stringID
//every tween currenty loaded is just added to a big dictionary, and then when you want to call a tween it just gets called iwth 
//TweenManager.InvokeTween("tweenID");

public class TweenManager : MonoBehaviour
{
    public static TweenManager current;

    public Dictionary<string, Tween> tweeners = new Dictionary<string, Tween>();

    private void Awake()
    {
        current = this;
    }


    public void InvokeTween(string tweenerName)
    {
        tweeners[tweenerName].DoTween();
    }


}
