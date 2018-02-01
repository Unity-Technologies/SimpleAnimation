using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimation))]
public class SimpleAnimationProxy : MonoBehaviour, IAnimation
{
    private class SimpleAnimationStateProxy: IAnimationState
    {
        public SimpleAnimationStateProxy(SimpleAnimation.State state)
        {
            m_State = state;
        }

        private SimpleAnimation.State m_State;

        public bool enabled
        {
            get { return m_State.enabled; }
            set { m_State.enabled = value; }
        }

        public bool isValid
        {
            get { return m_State.isValid; }
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

    private SimpleAnimation m_SimpleAnimation;

    private SimpleAnimation impl
    {
        get
        {
            if (m_SimpleAnimation ==  null)
            {
                m_SimpleAnimation = GetComponent<SimpleAnimation>();
            }
            return m_SimpleAnimation;
        }
    }

    public bool animatePhysics
    {
        get { return impl.animatePhysics; }
        set { impl.animatePhysics = value; }
    }

    public AnimatorCullingMode cullingMode
    {
        get
        {
            return impl.cullingMode;
        }

        set
        {
            impl.cullingMode = value;
        }
    }

    public bool isPlaying
    {
        get { return impl.isPlaying; }
    }

    public bool playAutomatically
    {
        get { return impl.playAutomatically; }
        set { impl.playAutomatically = value; }
    }

    public WrapMode wrapMode
    {
        get { return impl.wrapMode; }
        set { impl.wrapMode = value; }
    }

    public AnimationClip clip
    {
        get { return impl.clip; }
        set { impl.clip = value; }
    }

    public bool usesLegacy
    {
        get { return false; }
    }
    new public GameObject gameObject
    {
        get { return impl.gameObject; }
    }

    public void AddClip(AnimationClip clip, string newName)
    {
        impl.AddClip(clip, newName);
    }

    public void Blend(string state, float targetWeight, float fadeLength)
    {
        impl.Blend(state, targetWeight, fadeLength);
    }

    public void CrossFade(string state, float fadeLength)
    {
        impl.CrossFade(state, fadeLength);
    }

    public void CrossFadeQueued(string state, float fadeLength, QueueMode queueMode)
    {
        impl.CrossFadeQueued(state, fadeLength, queueMode);
    }

    public int GetClipCount()
    {
        return impl.GetClipCount();
    }

    public bool IsPlaying(string stateName)
    {
        return impl.IsPlaying(stateName);
    }

    public void Stop()
    {
        impl.Stop();
    }

    public void Stop(string stateName)
    {
        impl.Stop(stateName);
    }

    public void Sample()
    {
        impl.Sample();
    }

    public bool Play()
    {
        return impl.Play();
    }

    public bool Play(string stateName)
    {
        return impl.Play(stateName);
    }

    public void PlayQueued(string stateName, QueueMode queueMode)
    {
        impl.PlayQueued(stateName, queueMode);
    }

    public void RemoveClip(AnimationClip clip)
    {
        impl.RemoveClip(clip);
    }

    public void RemoveClip(string stateName)
    {
        impl.RemoveState(stateName);
    }

    public void Rewind()
    {
        impl.Rewind();
    }

    public void Rewind(string stateName)
    {
        impl.Rewind(stateName);
    }

    public IAnimationState GetState(string stateName)
    {
        SimpleAnimation.State state = impl[stateName];
        if (state != null)
            return new SimpleAnimationStateProxy(state);

        return null;
    }

    public IAnimationState this[string name]
    {
        get { return GetState(name); }
    }


}
