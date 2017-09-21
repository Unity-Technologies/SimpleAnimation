using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IAnimationState
{
    bool enabled { get; set; }
    bool isValid { get; }
    float time { get; set; }
    float normalizedTime { get; set; }
    float speed { get; set; }
    string name { get; set; }
    float weight { get; set; }
    float length { get; }
    AnimationClip clip { get; }
    WrapMode wrapMode { get; set; }

}
public interface IAnimation
{
    //Should animations apply velocities to physics objects
    bool animatePhysics  { get; set;  }
    //How should animations mode updated based on renderer visibility
    AnimatorCullingMode cullingMode { get; set; } 
    //Are we currently playing animations
    bool isPlaying { get;}
    //Should we start playing the default state
    bool playAutomatically { get; set; }
    //For clips where wrap mode is default, how should clips behave after they are done
    WrapMode wrapMode { get; set; }
    bool usesLegacy { get; }
    GameObject gameObject { get; }
    AnimationClip clip { get; set; }

    //Adds a new state named newName, which uses the AnimationClip clip
    void AddClip(AnimationClip clip, string newName);

    //Starts blending the state animation, towards weight targetWeight, fading over fadeLength seconds
    void Blend(string animation, float targetWeight, float fadeLength);
    
    //Over the next fadeLength seconds, start fading in state animation, fading out all other states
    void CrossFade(string animation, float fadeLength);
    
    //Queue a crossfade after the currently playing states are done playing.
    void CrossFadeQueued(string animation, float fadeLength, QueueMode queueMode);

    //Gets the number of AnimationClips attached to the component
    int GetClipCount();

    //Returns true if state stateName is playing
    bool IsPlaying(string stateName);
    
    //Stops all playing animations on this component
    void Stop();

    //Stops state named stateName
    void Stop(string stateName);

    //Evaluates the animations at the current time
    void Sample();
    
    //Plays the default clip. Returns false if there is no clip attached
    bool Play();

    //Plays the specified clip. Returns false if the state does not exist
    bool Play(string stateName);
    
    //Queue a Play after the current states are done playing
    void PlayQueued(string stateName, QueueMode queueMode);

    //Removes the specified clip, and any states using it
    void RemoveClip(AnimationClip clip);

    //Removes the specified state from the list of states,
    void RemoveClip(string stateName);

    //Rewinds all states
    void Rewind();

    //Rewinds state named stateName
    void Rewind(string stateName);

    //Returns a handle on a state. Returns null if the state doesn't exist
    IAnimationState GetState(string stateName);

    IAnimationState this[string name] { get; }

}
