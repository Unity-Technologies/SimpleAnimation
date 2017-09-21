using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationTransitions : MonoBehaviour {
    enum AnimationType
    {
        Legacy,
        SimplePlayable,
        StateMachine
    }
    AnimationType animationType;
    SimpleAnimation simpleAnimation;
    public AnimationClip clip;
	// Use this for initialization
	IEnumerator Start ()
    {
        var animationComponent = GetComponent<Animation>(); 
        var animatorComponent = GetComponent<Animator>();
        var simpleAnimationComponent = GetComponent<SimpleAnimation>();
        if (animationComponent)
            animationType = AnimationType.Legacy;
        else if (simpleAnimationComponent)
            animationType = AnimationType.SimplePlayable;
        else
            animationType = AnimationType.StateMachine;
        

        switch (animationType)
        {
            case AnimationType.Legacy:
                animationComponent.AddClip(clip, "A");
                animationComponent.AddClip(clip, "B");
                break;
            case AnimationType.SimplePlayable:
                simpleAnimationComponent.AddClip(clip, "A");
                simpleAnimationComponent.AddClip(clip, "B");
                break;
            case AnimationType.StateMachine:
                break;
            default:
                break;
        }
        

        while(true)
        {
            switch (animationType)
            {
                case AnimationType.Legacy:
                    animationComponent.Play("A");
                    break;
                case AnimationType.SimplePlayable:
                    simpleAnimationComponent.Play("A");
                    break;
                case AnimationType.StateMachine:
                    animatorComponent.Play("A");
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.5f);

            switch (animationType)
            {
                case AnimationType.Legacy:
                    animationComponent.Play("B");
                    break;
                case AnimationType.SimplePlayable:
                    simpleAnimationComponent.Play("B");
                    break;
                case AnimationType.StateMachine:
                    animatorComponent.Play("B");
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
	}

   
}
