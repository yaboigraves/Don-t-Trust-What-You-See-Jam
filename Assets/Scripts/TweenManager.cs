using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class is basically just a chill singleton that helps functions invoke tweens by a simple stringID
//every tween currenty loaded is just added to a big dictionary, and then when you want to call a tween it just gets called iwth 
//TweenManager.InvokeTween("tweenID");

public class TweenManager : MonoBehaviour
{
    public static TweenManager current;

    //TODO: rewrite this as maybe just a list of strings to tweens, it'll be easier to make them all be triggered whenever
    public List<KeyValuePair<string, Tween>> tweeners = new List<KeyValuePair<string, Tween>>();

    private void Awake()
    {
        current = this;
    }


    public void InvokeTween(string tweenerName)
    {
        //find any tweens with this name

        foreach (KeyValuePair<string, Tween> t in tweeners)
        {
            if (t.Key == tweenerName)
            {
                t.Value.DoTween();
            }
        }



    }


}
