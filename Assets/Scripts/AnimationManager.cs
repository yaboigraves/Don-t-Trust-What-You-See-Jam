using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    //so every time we load into a new level we gotta get a reference to the animation controllers we need
    public Animator currentPlayerController, currentEnemyController;
    public static AnimationManager current;
    private void Awake()
    {
        current = this;
    }

    public void FindAnimationControllersInScene()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");

        if (player == null || enemy == null)
        {
            Debug.LogWarning("Couldnt find an animation controller in the scene");
            Debug.Break();
            return;
        }
        else
        {
            Debug.Log("Success!");
        }

        //check and find the animators attatcched to both of these boys 

        currentPlayerController = player.GetComponentInChildren<Animator>();
        currentEnemyController = enemy.GetComponentInChildren<Animator>();
    }


    //this is called roughly every bar to switch to a new animation;
    public void BeatSwitchAnimation()
    {
        currentPlayerController.SetTrigger("beatSwitch");
        currentEnemyController.SetTrigger("beatSwitch");
    }




}