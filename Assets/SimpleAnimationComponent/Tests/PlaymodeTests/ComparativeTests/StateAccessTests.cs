using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class StateAccessTests
{
    public class BaseTests
    {
        [Test]
        public void GetState_WithNoState_ReturnsNull([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);

            IAnimationState state = animation.GetState("InvalidName");
            Assert.AreEqual(null, state);
        }

        [Test]
        public void GetState_WithState_ReturnsState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreNotEqual(null, state);
            Assert.AreEqual("ValidName", state.name);
        }
    }

    public class TimeTests
    {
        [Test]
        public void State_Time_Equals_Zero_BeforePlay([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreEqual(0f, state.time);
        }

        [Test]
        public void State_Time_Equals_Zero_AfterPlay([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            Assert.AreEqual(0f, state.time);
        }

        [Test]
        public void State_Time_Affects_ClipPlayback([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            state.weight = 1f;
            state.time = 0.5f;
            animation.Sample();
            Assert.AreEqual(animation.gameObject.transform.localPosition.x, state.time, "Sampling should have updated the position of the object at time 0.5");
        }

        [UnityTest]
        public IEnumerator State_Time_ChangesWith_GameTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            float previousTime = Time.time;
            yield return null;
            yield return null; //Since the Animation Component doesn't update the time on the first frame, we must wait two frames to see an effect
            float elapsedTime = Time.time - previousTime;
            Assert.AreNotEqual(elapsedTime, state.time, "State time should have changed");
        }

        [UnityTest]
        public IEnumerator State_Time_SetPast_ClipEnd_StopsState_AfterOneFrame([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.time = 2.0f;
            yield return null;
            Assert.IsFalse(state.enabled, "State should be disabled");
            Assert.IsFalse(animation.IsPlaying(state.name), "State should be disabled");
            Assert.IsFalse(animation.isPlaying, "State should be disabled");
        }

        [UnityTest]
        public IEnumerator State_Time_SetPast_ClipEnd_ThenBack_DoesntStopState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.time = 2.0f;
            state.time = 0.2f;
            yield return null;
            Assert.IsTrue(state.enabled, "State should be enabled");
            Assert.IsTrue(animation.IsPlaying(state.name), "State should be playing");
            Assert.IsTrue(animation.isPlaying, "Component should be playing");
        }

        [Test]
        public void State_Time_SetPast_ClipEnd_Doesnt_Immediately_StopState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.time = 2.0f;
            Assert.IsTrue(state.enabled, "State should be enabled");
            Assert.IsTrue(animation.IsPlaying(state.name), "State should be playing");
            Assert.IsTrue(animation.isPlaying, "Component should be playing");
        }

        [UnityTest]
        public IEnumerator State_Time_SetTime_DoesntStartState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.time = 2.0f;
            state.time = 0.2f;
            yield return null;
            Assert.IsTrue(state.enabled, "State should be disabled");
            Assert.IsTrue(animation.IsPlaying(state.name), "State should be disabled");
            Assert.IsTrue(animation.isPlaying, "State should be disabled");
        }

        [UnityTest]
        public IEnumerator State_Time_SetTime_DoesntPlayEvents([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("FiresEvent");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            var eventReceiver = animation.gameObject.AddComponent<SimpleAnimationTests.ReceivesEvent>();
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.time = 0.1f;
            yield return null;
            Assert.AreEqual(0, eventReceiver.eventCount, "Event should not have been received");
            state.time = 0.6f;
            yield return null;
            Assert.AreEqual(0, eventReceiver.eventCount, "Event should have been received after setting the time on the state");
        }
    }

    public class NormalizedTime
    {
        [Test]
        public void State_NormalizedTime_AffectsTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;
            //Create event to extend clip
            var evt = new AnimationEvent();
            evt.time = 2.0f;
            clipInstance.AddEvent(evt);

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.normalizedTime = 0.5f;
            Assert.AreEqual(state.normalizedTime*state.length, state.time);
        }

        [Test]
        public void State_Time_AffectsNormalizedTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            //Create event to extend clip
            var evt = new AnimationEvent();
            evt.time = 2.0f;
            clipInstance.AddEvent(evt);

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.time = 0.5f;
            Assert.AreEqual(state.time / state.length, state.normalizedTime);
        }
    }

    public class Enabled
    {
        [Test]
        public void State_Enabled_InitialValue_False([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.IsFalse(state.enabled);
        }

        [Test]
        public void State_Enabled_SetTrue_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            Assert.IsTrue(state.enabled);
        }

        [Test]
        public void State_Enabled_AfterPlay_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            Assert.IsTrue(state.enabled);
        }

        [Test]
        public void State_Enabled_AfterStop_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            animation.Stop(state.name);
            Assert.IsFalse(state.enabled);
        }

        [UnityTest]
        public IEnumerator State_Enabled_AfterStateEnd_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            yield return new WaitForSeconds(1.1f);

            Assert.IsFalse(state.enabled);
        }

        [UnityTest]
        public IEnumerator State_Enabled_AfterSetTime_ToStateEnd_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            state.time = 2f;
            yield return null;
            Assert.IsFalse(state.enabled);
        }

        [Test]
        public void State_Enabled_False_IsPlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            animation.Play(state.name);
            state.enabled = false;
            Assert.IsFalse(animation.isPlaying);
            Assert.IsFalse(animation.IsPlaying(state.name));
        }
    }

    public class Speed
    {
        [UnityTest]
        public IEnumerator State_Speed_SetTo_Zero_Time_DoesntAdvance([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.speed = 0f;
            state.enabled = true;
            yield return null;
            yield return null; //Second frame to allow Animation Component to advance time
            Assert.AreEqual(0f, state.time);

        }

        [UnityTest]
        public IEnumerator State_Speed_Negative_StopsState_WhenTime_Reaches_Negative([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.speed = -1f;
            state.enabled = true;
            yield return null;
            yield return null; //Second frame to allow Animation Component to advance time
            Assert.IsFalse(state.enabled);
        }

        [UnityTest]
        public IEnumerator State_Speed_Negative_DoesntStopsState_WhenTime_Reaches_Negative_OnLoopedClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;
            clipInstance.wrapMode = WrapMode.Loop;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.speed = -1f;
            state.enabled = true;
            yield return null;
            yield return null; //Second frame to allow Animation Component to advance time
            Assert.IsTrue(state.enabled);

        }

        [Test]
        public void State_Speed_DoesntAffect_NormalizedTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;
            clipInstance.wrapMode = WrapMode.Loop;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.time = 0.5f;
            float normalizedTime = state.normalizedTime;
            state.speed = 10.0f;
            Assert.AreEqual(normalizedTime, state.normalizedTime);
        }
    }

    public class Name
    {
        [Test]
        public void State_Name_Equals_AddClip_Name([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreEqual("ValidName", state.name);
        }

        [Test]
        public void State_Name_ChangingName_Name_Equals_NewName([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.name = "NewName";
            Assert.AreEqual("NewName", state.name);
        }

        [Test]
        public void State_Name_ChangingName_DoesntInvalidate_State([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.name = "NewName";
            Assert.IsTrue(state.isValid);
        }

        [Test]
        public void State_Name_RenamedState_CantBeFound_ByOldName([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.name = "NewName";
            state = animation.GetState("ValidName");
            Assert.IsNull(state);
        }

        [Test]
        public void State_Name_RenamedState_CanBeFound_ByNewName([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.name = "NewName";
            state = animation.GetState("NewName");
            Assert.IsNotNull(state);
        }

        [Test]
        public void State_Name_EmptyString_CanBeFound([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.name = "";
            state = animation.GetState("");
            Assert.IsNotNull(state);
        }
    }

    public class Weight
    {
        [UnityTest]
        public IEnumerator State_Weight_EqualZero_DoesntWrite([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            state.weight = 0f;
            state.time = 0.5f; //Seek the clip so that values should be written;
            yield return null;
            Assert.AreEqual(0f, animation.gameObject.transform.localPosition.x);
        }

        [UnityTest]
        public IEnumerator State_Weight_FlipFlopTest([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            state.weight = 0f;
            state.time = 0.5f; //Seek the clip so that values should be written;
            yield return null;
            Assert.AreEqual(0f, animation.gameObject.transform.localPosition.x);
            state.weight = 1f;
            yield return null;
            Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.x);
        }

        [UnityTest]
        public IEnumerator State_Weight_Normalizes([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clip2 = Resources.Load<AnimationClip>("LinearY");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            var clipInstance2 = Object.Instantiate<AnimationClip>(clip2);
            clipInstance.legacy = animation.usesLegacy;
            clipInstance2.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "State1");
            animation.AddClip(clipInstance2, "State2");
            IAnimationState state1 = animation.GetState("State1");
            IAnimationState state2 = animation.GetState("State2");
            state1.enabled = true;
            state2.enabled = true;
            state1.weight = 1f;
            state2.weight = 1f;
            state1.time = 0.5f; //Seek the clip so that values should be written;
            state2.time = 0.5f; //Seek the clip so that values should be written;
            yield return null;
            Assert.AreEqual(0.25f, animation.gameObject.transform.localPosition.x);
            Assert.AreEqual(0.25f, animation.gameObject.transform.localPosition.y);
        }

        [UnityTest]
        public IEnumerator State_Weight_NonZero_Writes([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            state.weight = 1f;
            state.time = 0.5f; //Seek the clip so that values should be written;
            yield return null;
            Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.x);
        }

        [Test]
        public void State_Weight_SetWeight_Equals_GetWeight([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.weight = 1f;
            Assert.AreEqual(1f, state.weight);
        }

        [Test]
        public void State_Weight_InitialValue_IsZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreEqual(0f, state.weight);
        }

        [Test]
        public void State_Weight_SetEnable_DoesntChange_Weight([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.enabled = true;
            Assert.AreEqual(0f, state.weight);
            state.weight = 1f;
            state.enabled = false;
            Assert.AreEqual(1f, state.weight);
        }

        [Test]
        public void State_Weight_SetWeight_DoesntChange_Enable([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.weight = 1f;
            Assert.IsFalse(state.enabled);
            state.enabled = true;
            state.weight = 0f;
            Assert.IsTrue(state.enabled);
        }
    }

    public class Length
    {
        [Test]
        public void State_Length_Equals_ClipLength([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreEqual(clipInstance.length, state.length);
        }
    }

    public class Clip
    {
        [Test]
        public void State_Clip_Equals_SetClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, clipInstance.name);
            IAnimationState state = animation.GetState(clipInstance.name);
            Assert.AreEqual(clipInstance, state.clip);
        }
    }

    public class WrapModeTests
    {
        [Test]
        public void WrapMode_Equals_Clip_WrapMode([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, clipInstance.name);
            IAnimationState state = animation.GetState(clipInstance.name);
            Assert.AreEqual(clipInstance.wrapMode, state.wrapMode);
        }
    }
}
