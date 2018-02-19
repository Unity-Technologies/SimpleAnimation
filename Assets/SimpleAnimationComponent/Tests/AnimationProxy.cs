using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationProxy : MonoBehaviour, IAnimation
{

    private class AnimationStateProxy: IAnimationState
    {
        public AnimationStateProxy(AnimationState state)
        {
            m_State = state;
        }

        private AnimationState m_State;

        public bool enabled
        {
            get { return m_State.enabled; }
            set { m_State.enabled = value; }
        }

        public bool isValid
        {
            get { return (bool)m_State; }
        }

        public float time
        {
            get { return m_State.time; }
            set { m_State.time = value; }
        }
        public float normalizedTime
        {
            get { return m_State.normalizedTime; }
            set { m_State.normalizedTime = value; }
        }
        public float speed
        {
            get { return m_State.speed; }
            set { m_State.speed = value; }
        }

        public string name
        {
            get { return m_State.name; }
            set { m_State.name = value; }
        }
        public float weight
        {
            get { return m_State.weight; }
            set { m_State.weight = value; }
        }
        public float length
        {
            get { return m_State.length; }
        }

        public AnimationClip clip
        {
            get { return m_State.clip; }
        }

        public WrapMode wrapMode
        {
            get { return m_State.wrapMode; }
            set { m_State.wrapMode = value; }
        }
    }

    private Animation m_Animation;

    new private Animation animation
    {
        get
        {
            if (m_Animation ==  null)
            {
                m_Animation = GetComponent<Animation>();
            }
            return m_Animation;
        }
    }

    public bool animatePhysics
    {
        get { return animation.animatePhysics; }
        set { animation.animatePhysics = value; }
    }

    public AnimatorCullingMode cullingMode
    {
        get
        {
            AnimatorCullingMode mode;
            switch (animation.cullingType)
            {
                case AnimationCullingType.AlwaysAnimate:
                    mode = AnimatorCullingMode.AlwaysAnimate;
                    break;
                case AnimationCullingType.BasedOnRenderers:
                    mode = AnimatorCullingMode.CullCompletely;
                    break;
                default:
                    mode = AnimatorCullingMode.CullUpdateTransforms;
                    break;
            }
            return mode;
        }

        set
        {
            AnimationCullingType type;
            switch(value)
            {
                case AnimatorCullingMode.AlwaysAnimate:
                    type = AnimationCullingType.AlwaysAnimate;
                    break;
                default:
                    type = AnimationCullingType.BasedOnRenderers;
                    break;

            }
            animation.cullingType = type;
        }
    }

    public bool isPlaying
    {
        get { return animation.isPlaying; }
    }

    public bool playAutomatically
    {
        get { return animation.playAutomatically; }
        set { animation.playAutomatically = value; }
    }

    public WrapMode wrapMode
    {
        get { return animation.wrapMode; }
        set { animation.wrapMode = value; }
    }

    public AnimationClip clip
    {
        get { return animation.clip; }
        set { animation.clip = value; }
    }

    public bool usesLegacy
    {
        get { return true; }
    }
    new public GameObject gameObject
    {
        get { return animation.gameObject; }
    }

    public void AddClip(AnimationClip clip, string newName)
    {
        animation.AddClip(clip, newName);
    }

    public void Blend(string state, float targetWeight, float fadeLength)
    {
        animation.Blend(state, targetWeight, fadeLength);
    }

    public void CrossFade(string state, float fadeLength)
    {
        animation.CrossFade(state, fadeLength);
    }

    public void CrossFadeQueued(string state, float fadeLength, QueueMode queueMode)
    {
        animation.CrossFadeQueued(state, fadeLength, queueMode);
    }

    public int GetClipCount()
    {
        return animation.GetClipCount();
    }

    public bool IsPlaying(string stateName)
    {
        return animation.IsPlaying(stateName);
    }

    public void Stop()
    {
        animation.Stop();
    }

    public void Stop(string stateName)
    {
        animation.Stop(stateName);
    }

    public void Sample()
    {
        animation.Sample();
    }

    public bool Play()
    {
        return animation.Play();
    }

    public bool Play(string stateName)
    {
        return animation.Play(stateName);
    }

    public void PlayQueued(string stateName, QueueMode queueMode)
    {
        animation.PlayQueued(stateName, queueMode);
    }

    public void RemoveClip(AnimationClip clip)
    {
        animation.RemoveClip(clip);
    }

    public void RemoveClip(string stateName)
    {
        animation.RemoveClip(stateName);
    }

    public void Rewind()
    {
        animation.Rewind();
    }

    public void Rewind(string stateName)
    {
        animation.Rewind(stateName);
    }

    public IAnimationState GetState(string stateName)
    {
        AnimationState state = animation[stateName];
        if (state != null)
            return new AnimationStateProxy(state);

        return null;
    }

    public IAnimationState this[string name]
    {
        get { return GetState(name); }
    }


}
