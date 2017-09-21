using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;

public partial class SimpleAnimationPlayable : PlayableBehaviour
{
    private int m_StatesVersion = 0;

    private void InvalidateStates() { m_StatesVersion++; }
    private class StateEnumerable: IEnumerable<IState>
    {
        private SimpleAnimationPlayable m_Owner;
        public StateEnumerable(SimpleAnimationPlayable owner)
        {
            m_Owner = owner;
        }

        public IEnumerator<IState> GetEnumerator()
        {
            return new StateEnumerator(m_Owner);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StateEnumerator(m_Owner);
        }

        class StateEnumerator : IEnumerator<IState>
        {
            private int m_Index = -1;
            private int m_Version;
            private SimpleAnimationPlayable m_Owner;
            public StateEnumerator(SimpleAnimationPlayable owner)
            {
                m_Owner = owner;
                m_Version = m_Owner.m_StatesVersion;
                Reset();
            }

            private bool IsValid() { return m_Owner != null && m_Version == m_Owner.m_StatesVersion; }

            IState GetCurrentHandle(int index)
            {
                if (!IsValid())
                    throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");

                if (index < 0 || index >= m_Owner.m_States.Count)
                    throw new InvalidOperationException("Enumerator is invalid");

                StateInfo state = m_Owner.m_States[index];
                if (state == null)
                    throw new InvalidOperationException("Enumerator is invalid");

                return new StateHandle(m_Owner, state.index, state.playable);
            }

            object IEnumerator.Current { get { return GetCurrentHandle(m_Index); } }

            IState IEnumerator<IState>.Current { get { return GetCurrentHandle(m_Index); } }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (!IsValid())
                    throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");

                do
                { m_Index++; } while (m_Index < m_Owner.m_States.Count && m_Owner.m_States[m_Index] == null);

                return m_Index < m_Owner.m_States.Count;
            }

            public void Reset()
            {
                if (!IsValid())
                    throw new InvalidOperationException("The collection has been modified, this Enumerator is invalid");
                m_Index = -1;
            }
        }
    }
    
    public interface IState
    {
        bool IsValid();

        bool enabled { get; set; }

        float time { get; set; }

        float normalizedTime { get; set; }

        float speed { get; set; }

        string name { get; set; }

        float weight { get; set; }

        float length { get; }

        AnimationClip clip { get; }

        WrapMode wrapMode { get; }
    }

    public class StateHandle : IState
    {
        public StateHandle(SimpleAnimationPlayable s, int index, Playable target)
        {
            m_Parent = s;
            m_Index = index;
            m_Target = target;
        }

        public bool IsValid()
        {
            return m_Parent.ValidateInput(m_Index, m_Target);
        }

        public bool enabled
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States[m_Index].enabled;
            }

            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                if (value)
                    m_Parent.m_States.EnableState(m_Index);
                else
                    m_Parent.m_States.DisableState(m_Index);

            }
        }

        public float time
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return (float)m_Parent.m_States.GetStateTime(m_Index);
            }
            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                m_Parent.m_States.SetStateTime(m_Index, value);
            }
        }

        public float normalizedTime
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");

                float length = m_Parent.m_States.GetStateLength(m_Index);
                if (length == 0f)
                    length = 1f;

                return (float)m_Parent.m_States.GetStateTime(m_Index) / length;
            }
            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");

                float length = m_Parent.m_States.GetStateLength(m_Index);
                if (length == 0f)
                    length = 1f;

                m_Parent.m_States.SetStateTime(m_Index, value *= length);
            }
        }

        public float speed
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States.GetStateSpeed(m_Index);
            }
            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                m_Parent.m_States.SetStateSpeed(m_Index, value);
            }
        }

        public string name
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States.GetStateName(m_Index);
            }
            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                if (value == null)
                    throw new System.ArgumentNullException("A null string is not a valid name");
                m_Parent.m_States.SetStateName(m_Index, value);
            }
        }

        public float weight
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States[m_Index].weight;
            }
            set
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                if (value < 0)
                    throw new System.ArgumentException("Weights cannot be negative");

                m_Parent.m_States[m_Index].weight = value;
            }
        }

        public float length
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States.GetStateLength(m_Index);
            }
        }

        public AnimationClip clip
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States.GetStateClip(m_Index);
            }
        }

        public WrapMode wrapMode
        {
            get
            {
                if (!IsValid())
                    throw new System.InvalidOperationException("This StateHandle is not valid");
                return m_Parent.m_States.GetStateWrapMode(m_Index);
            }
        }

        public int index { get { return m_Index; } }

        private SimpleAnimationPlayable m_Parent;
        private int m_Index;
        private Playable m_Target;
        private StateInfo state;
    }
    private class StateInfo
    {
        public bool enabled;
        public int index;
        public string stateName;
        public bool fading;
        public float time;
        public float targetWeight;
        public float weight;
        public float fadeSpeed;
        public float speed;
        public AnimationClip clip;
        public Playable playable;
        public WrapMode wrapMode;

        //Clone information
        public bool isClone;
        public StateHandle parentState;

        //
        public bool weightDirty;
        public bool enabledDirty;
        public bool timeIsUpToDate;
    }

    private StateHandle StateInfoToHandle(StateInfo info)
    {
        return new StateHandle(this, info.index, info.playable);
    }

    private class StateManagement
    {
        private List<StateInfo> m_States;

        public int Count { get { return m_Count; } }

        private int m_Count;

        public StateInfo this[int i]
        {
            get
            {
                return m_States[i];
            }
        }

        public StateManagement()
        {
            m_States = new List<StateInfo>();
        }

        public StateInfo InsertState()
        {
            StateInfo state = new StateInfo();

            int firstAvailable = m_States.FindIndex(s => s == null);
            if (firstAvailable == -1)
            {
                firstAvailable = m_States.Count;
                m_States.Add(state);
            }
            else
            {
                m_States.Insert(firstAvailable, state);
            }

            state.index = firstAvailable;
            m_Count++;
            return state;
        }
        public bool AnyStatePlaying()
        {
            return m_States.FindIndex(s => s != null && s.enabled) != -1;
        }

        public bool IsStateDone(int index)
        {
            StateInfo state = m_States[index];
            if (state == null)
                return true;

            return !state.enabled;
        }

        public void RemoveAtIndex(int index)
        {
            StateInfo removed = m_States[index];
            m_States[index] = null;
            if (removed.playable.IsValid())
            {
                removed.playable.GetGraph().DestroyPlayable(removed.playable);
            }
            m_Count = m_States.Count;
        }

        public bool RemoveClip(AnimationClip clip)
        {
            bool removed = false;
            for (int i = 0; i < m_States.Count; i++)
            {
                StateInfo state = m_States[i];
                if (state != null &&state.clip == clip)
                {
                    RemoveAtIndex(i);
                    removed = true;
                }
            }
            return removed;
        }

        public StateInfo FindState(string name)
        {
            int index = m_States.FindIndex(s => s != null && s.stateName == name);
            if (index == -1)
                return null;

            return m_States[index];
        }

        public void EnableState(int index)
        {
            StateInfo state = m_States[index];
            if (state.enabled)
                return;

            state.enabledDirty = true;
            state.enabled = true;
        }

        public void DisableState(int index)
        {
            StateInfo state = m_States[index];
            if (state.enabled == false)
                return;

            state.enabledDirty = true;
            state.enabled = false;
        }

        public void SetInputWeight(int index, float weight)
        {
            StateInfo state = m_States[index];
            state.targetWeight = weight;
            state.weight = weight;
            state.fading = false;
            state.weightDirty = true;
        }

        public void SetStateTime(int index, float time)
        {
            StateInfo state = m_States[index];
            state.time = time;

            state.playable.SetTime(time);
            state.playable.SetDone(time >= state.playable.GetDuration());
        }

        public bool IsCloneOf(int potentialCloneIndex, int originalIndex)
        {
            StateInfo potentialClone = m_States[potentialCloneIndex];
            return potentialClone.isClone && potentialClone.parentState.index == originalIndex;
        }

        public float GetStateTime(int index)
        {
            StateInfo state = m_States[index];
            if (state.timeIsUpToDate)
                return state.time;
            state.time = (float)state.playable.GetTime();
            state.timeIsUpToDate = true;
            return state.time;
        }

        public float GetStateSpeed(int index)
        {
            return (float)m_States[index].playable.GetSpeed();
        }
        public void SetStateSpeed(int index, float value)
        {
            m_States[index].playable.SetSpeed(value);
        }

        public float GetInputWeight(int index)
        {
            return m_States[index].weight;
        }

        public float GetStateLength(int index)
        {
            AnimationClip clip = m_States[index].clip;
            if (clip == null)
                return 0f;
            float speed = (float)m_States[index].playable.GetSpeed();
            if (m_States[index].playable.GetSpeed() == 0f)
                return Mathf.Infinity;

            return clip.length / speed;
        }

        public float GetStatePlayableDuration(int index)
        {
            Playable playable = m_States[index].playable;
            if (!playable.IsValid())
                return 0f;
            return (float)playable.GetDuration();
        }

        public AnimationClip GetStateClip(int index)
        {
            AnimationClip clip = m_States[index].clip;
            if (clip == null)
                return null;

            return clip;
        }

        public WrapMode GetStateWrapMode(int index)
        {
            return m_States[index].wrapMode;
        }

        public string GetStateName(int index)
        {
            return m_States[index].stateName;
        }

        public void SetStateName(int index, string name)
        {
            m_States[index].stateName = name;
        }

        public void StopState(int index, bool cleanup)
        {
            if (cleanup)
            {
                RemoveAtIndex(index);
            }
            else
            {
                StateInfo state = m_States[index];
                state.fadeSpeed = 0f;
                state.weight = 0f;
                state.targetWeight = 0f;
                state.time = 0f;
                state.enabled = false;
                state.enabledDirty = true;
                state.weightDirty = true;
                state.playable.SetTime(0f);
                state.playable.SetDone(false);
            }
        }

    }

    private class QueuedState
    {
        public QueuedState(StateHandle s, float t)
        {
            state = s;
            fadeTime = t;
        }

        public StateHandle state;
        public float fadeTime;
    }

}