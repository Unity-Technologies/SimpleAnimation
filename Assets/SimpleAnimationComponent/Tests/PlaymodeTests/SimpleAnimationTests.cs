using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using NUnit.Framework;

public class SimpleAnimationTests
{
    static SimpleAnimation Instantiate()
    {
        var go = new GameObject();
        return go.AddComponent<SimpleAnimation>();
    }
    public class GetStates
    {
        [Test]
        public void GetStates_WithNoStates_IEnumerator_MoveNext_ReturnsFalse()
        {
            SimpleAnimation animation = Instantiate();
            IEnumerable<SimpleAnimation.State> states = animation.GetStates();
            var it = states.GetEnumerator();
            Assert.IsFalse(it.MoveNext());
        }

        [Test]
        public void GetStates_WithNoStates_IEnumerator_Current_Throws()
        {
            SimpleAnimation animation = Instantiate();
            IEnumerable<SimpleAnimation.State> states = animation.GetStates();
            var it = states.GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => { SimpleAnimation.State state = it.Current; });
        }

        [Test]
        public void GetStates_WithSingleState_IEnumerator_Returns_ValidState()
        {
            SimpleAnimation animation = Instantiate();
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);

            animation.AddClip(clipInstance, "SingleClip");
            IEnumerable<SimpleAnimation.State> states = animation.GetStates();
            var it = states.GetEnumerator();
            it.MoveNext();
            SimpleAnimation.State state = it.Current;
            Assert.AreEqual("SingleClip", state.name);
        }

        [Test]
        public void GetStates_ModifyStates_IEnumerator_MoveNext_Throws()
        {
            SimpleAnimation animation = Instantiate();
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);

            animation.AddClip(clipInstance, "SingleClip");
            IEnumerable<SimpleAnimation.State> states = animation.GetStates();
            var it = states.GetEnumerator();
            animation.RemoveState("SingleClip");
            Assert.Throws<InvalidOperationException>(() => { it.MoveNext(); });
        }
    }

    public class LegacyClips
    {
        [Test]
        public void SetClip_WithLegacyClip_Throws_ArgumentException()
        {
            SimpleAnimation animation = Instantiate();
            var clip = new AnimationClip();
            clip.legacy = true;
            Assert.Throws<ArgumentException>(() => { animation.clip = clip; });
        }

        [Test]
        public void AddClip_WithLegacyClip_Throws_ArgumentException()
        {
            SimpleAnimation animation = Instantiate();
            var clip = new AnimationClip();
            clip.legacy = true;
            Assert.Throws<ArgumentException>(() => { animation.AddClip(clip, "DefaultName");});
        }

        [Test]
        public void AddState_WithLegacyClip_Throws_ArgumentException()
        {
            SimpleAnimation animation = Instantiate();
            var clip = new AnimationClip();
            clip.legacy = true;
            Assert.Throws<ArgumentException>(() => { animation.AddState(clip, "DefaultName"); });
        }
    }


    //Event Receiver for FiresEvent AnimationClip
    public class ReceivesEvent : MonoBehaviour
    {
        public int eventCount;

        void Event()
        {
            eventCount++;
        }
    }
}
