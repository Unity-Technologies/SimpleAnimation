using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public partial class SimpleAnimation: MonoBehaviour, IAnimationClipSource
{
    const string kDefaultStateName = "Default";
    private class StateEnumerable : IEnumerable<State>
    {
        private SimpleAnimation m_Owner;
        public StateEnumerable(SimpleAnimation owner)
        {
            m_Owner = owner;
        }

        public IEnumerator<State> GetEnumerator()
        {
            return new StateEnumerator(m_Owner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StateEnumerator(m_Owner);
        }

        class StateEnumerator : IEnumerator<State>
        {
            private SimpleAnimation m_Owner;
            private IEnumerator<SimpleAnimationPlayable.IState> m_Impl;
            public StateEnumerator(SimpleAnimation owner)
            {
                m_Owner = owner;
                m_Impl = m_Owner.m_Playable.GetStates().GetEnumerator();
                Reset();
            }

            State GetCurrent()
            {
                return new StateImpl(m_Impl.Current, m_Owner);
            }

            object IEnumerator.Current { get { return GetCurrent(); } }

            State IEnumerator<State>.Current { get { return GetCurrent(); } }

            public void Dispose() { }

            public bool MoveNext()
            {
                return m_Impl.MoveNext();
            }

            public void Reset()
            {
                m_Impl.Reset();
            }
        }
    }
    private class StateImpl : State
    {
        public StateImpl(SimpleAnimationPlayable.IState handle, SimpleAnimation component)
        {
            m_StateHandle = handle;
            m_Component = component;
        }

        private SimpleAnimationPlayable.IState m_StateHandle;
        private SimpleAnimation m_Component;

        bool State.enabled
        {
            get { return m_StateHandle.enabled; }
            set
            {
                m_StateHandle.enabled = value;
                if (value)
                {
                    m_Component.Kick();
                }
            }
        }

        bool State.isValid
        {
            get { return m_StateHandle.IsValid(); }
        }
        float State.time
        {
            get { return m_StateHandle.time; }
            set { m_StateHandle.time = value;
                m_Component.Kick(); }
        }
        float State.normalizedTime
        {
            get { return m_StateHandle.normalizedTime; }
            set { m_StateHandle.normalizedTime = value;
                  m_Component.Kick();}
        }
        float State.speed
        {
            get { return m_StateHandle.speed; }
            set { m_StateHandle.speed = value;
                  m_Component.Kick();}
        }

        string State.name
        {
            get { return m_StateHandle.name; }
            set { m_StateHandle.name = value; }
        }
        float State.weight
        {
            get { return m_StateHandle.weight; }
            set { m_StateHandle.weight = value;
                m_Component.Kick();}
        }
        float State.length
        {
            get { return m_StateHandle.length; }
        }

        AnimationClip State.clip
        {
            get { return m_StateHandle.clip; }
        }

        WrapMode State.wrapMode
        {
            get { return m_StateHandle.wrapMode; }
            set { Debug.LogError("Not Implemented"); }
        }
    }

    [System.Serializable]
    public class EditorState
    {
        public AnimationClip clip;
        public string name;
        public bool defaultState;
    }

    protected void Kick()
    {
        if (!m_IsPlaying)
        {
            m_Graph.Play();
            m_IsPlaying = true;
        }
    }

    protected PlayableGraph m_Graph;
    protected PlayableHandle m_LayerMixer;
    protected PlayableHandle m_TransitionMixer;
    protected Animator m_Animator;
    protected bool m_Initialized;
    protected bool m_IsPlaying;

    protected SimpleAnimationPlayable m_Playable;

    [SerializeField]
    protected bool m_PlayAutomatically = true;

    [SerializeField]
    protected bool m_AnimatePhysics = false;

    [SerializeField]
    protected AnimatorCullingMode m_CullingMode = AnimatorCullingMode.CullUpdateTransforms;

    [SerializeField]
    protected WrapMode m_WrapMode;

    [SerializeField]
    protected AnimationClip m_Clip;

    [SerializeField]
    private EditorState[] m_States;

    protected virtual void OnEnable()
    {
        Initialize();
        m_Graph.Play();
        if (m_PlayAutomatically)
        {
            Stop();
            Play();
        }
    }

    protected virtual void OnDisable()
    {
        if (m_Initialized)
        {
            Stop();
            m_Graph.Stop();
        }
    }

    private void Reset()
    {
        if (m_Graph.IsValid())
            m_Graph.Destroy();
        
        m_Initialized = false;
    }

    private void Initialize()
    {
        if (m_Initialized)
            return;

        m_Animator = GetComponent<Animator>();
        m_Animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
        m_Animator.cullingMode = m_CullingMode;
        m_Graph = PlayableGraph.Create();
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        SimpleAnimationPlayable template = new SimpleAnimationPlayable();

        var playable = ScriptPlayable<SimpleAnimationPlayable>.Create(m_Graph, template, 1);
        m_Playable = playable.GetBehaviour();
        m_Playable.onDone += OnPlayableDone;
        if (m_States == null)
        {
            m_States = new EditorState[1];
            m_States[0] = new EditorState();
            m_States[0].defaultState = true;
            m_States[0].name = "Default";
        }


        if (m_States != null)
        {
            foreach (var state in m_States)
            {
                if (state.clip)
                {
                    m_Playable.AddClip(state.clip, state.name);
                }
            }
        }

        EnsureDefaultStateExists();

        AnimationPlayableUtilities.Play(m_Animator, m_Playable.playable, m_Graph);
        Play();
        Kick();
        m_Initialized = true;
    }

    private void EnsureDefaultStateExists()
    {
        if ( m_Playable != null && m_Clip != null && m_Playable.GetState(kDefaultStateName) == null )
        {
            m_Playable.AddClip(m_Clip, kDefaultStateName);
            Kick();
        }
    }

    protected virtual void Awake()
    {
        Initialize();
    }

    protected void OnDestroy()
    {
        if (m_Graph.IsValid())
        {
            m_Graph.Destroy();
        }
    }

    private void OnPlayableDone()
    {
        m_Graph.Stop();
        m_IsPlaying = false;
    }

    private void RebuildStates()
    {
        var playableStates = GetStates();
        var list = new List<EditorState>();
        foreach (var state in playableStates)
        {
            var newState = new EditorState();
            newState.clip = state.clip;
            newState.name = state.name;
            list.Add(newState);
        }
        m_States = list.ToArray();
    }

    EditorState CreateDefaultEditorState()
    {
        var defaultState = new EditorState();
        defaultState.name = "Default";
        defaultState.clip = m_Clip;
        defaultState.defaultState = true;

        return defaultState;
    }

    static void LegacyClipCheck(AnimationClip clip)
    {
        if (clip && clip.legacy)
        {
            throw new ArgumentException(string.Format("Legacy clip {0} cannot be used in this component. Set .legacy property to false before using this clip", clip));
        }
    }
    
    void InvalidLegacyClipError(string clipName, string stateName)
    {
        Debug.LogErrorFormat(this.gameObject,"Animation clip {0} in state {1} is Legacy. Set clip.legacy to false, or reimport as Generic to use it with SimpleAnimationComponent", clipName, stateName);
    }

    private void OnValidate()
    {
        //Don't mess with runtime data
        if (Application.isPlaying)
            return;

        if (m_Clip && m_Clip.legacy)
        {
            Debug.LogErrorFormat(this.gameObject,"Animation clip {0} is Legacy. Set clip.legacy to false, or reimport as Generic to use it with SimpleAnimationComponent", m_Clip.name);
            m_Clip = null;
        }

        //Ensure at least one state exists
        if (m_States == null || m_States.Length == 0)
        {
            m_States = new EditorState[1];   
        }

        //Create default state if it's null
        if (m_States[0] == null)
        {
            m_States[0] = CreateDefaultEditorState();
        }

        //If first state is not the default state, create a new default state at index 0 and push back the rest
        if (m_States[0].defaultState == false || m_States[0].name != "Default")
        {
            var oldArray = m_States;
            m_States = new EditorState[oldArray.Length + 1];
            m_States[0] = CreateDefaultEditorState();
            oldArray.CopyTo(m_States, 1);
        }

        //If default clip changed, update the default state
        if (m_States[0].clip != m_Clip)
            m_States[0].clip = m_Clip;


        //Make sure only one state is default
        for (int i = 1; i < m_States.Length; i++)
        {
            if (m_States[i] == null)
            {
                m_States[i] = new EditorState();
            }
            m_States[i].defaultState = false;
        }

        //Ensure state names are unique
        int stateCount = m_States.Length;
        string[] names = new string[stateCount];

        for (int i = 0; i < stateCount; i++)
        {
            EditorState state = m_States[i];
            if (state.name == "" && state.clip)
            {
                state.name = state.clip.name;
            }

#if UNITY_EDITOR
            state.name = ObjectNames.GetUniqueName(names, state.name);
#endif
            names[i] = state.name;

            if (state.clip && state.clip.legacy)
            {
                InvalidLegacyClipError(state.clip.name, state.name);
                state.clip = null;
            }
        }

        m_Animator = GetComponent<Animator>();
        m_Animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
        m_Animator.cullingMode = m_CullingMode;
    }

    public void GetAnimationClips(List<AnimationClip> results)
    {
        foreach (var state in m_States)
        {
            if (state.clip != null)
                results.Add(state.clip);
        }
    }
}
