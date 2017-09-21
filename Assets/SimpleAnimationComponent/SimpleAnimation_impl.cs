using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public partial class SimpleAnimation: MonoBehaviour
{
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
    private class EditorState
    {
        public AnimationClip clip;
        public string name;
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
            m_States = new EditorState[0];

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
        if ( m_Playable != null && m_Clip != null && m_Playable.GetState(m_Clip.name) == null )
        {
            m_Playable.AddClip(m_Clip, m_Clip.name);
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
        }
        m_States = list.ToArray();
    }

    private void OnValidate()
    {
        //Don't mess with runtime data
        if (Application.isPlaying)
            return;

        if (m_States == null)
            m_States = new EditorState[0];

        //make sure default state is first
        if (m_Clip)
        {
            if (m_States.Length == 0
                || m_States[0].name != "Default"
                || m_States[0].clip != m_Clip)
            {
                var oldArray = m_States;
                m_States = new EditorState[oldArray.Length + 1];
                var defaultState = new EditorState();
                defaultState.name = "Default";
                defaultState.clip = m_Clip;
                m_States[0] = defaultState;
                oldArray.CopyTo(m_States, 1);
            }
        }
        
        //Ensure state names are unique
        var uniqueNames = new Dictionary<string, bool>();
        int stateCount = m_States.Length;
        for (int i = 0; i < stateCount; i++)
        {
            EditorState state = m_States[i];
            if (state.name == "" && state.clip)
            {
                state.name = state.clip.name;
            }
            
            int instanceNum = 0;
            bool exists = false;
            string name = state.name;
            string newName = state.name;
            while (uniqueNames.TryGetValue(newName, out exists))
            {
                instanceNum++;
                newName = string.Format("{0} {1}", name, instanceNum);
                
            }
            state.name = newName;
            uniqueNames.Add(newName, true);
        }

        Reset();
        Initialize();
    }

}
