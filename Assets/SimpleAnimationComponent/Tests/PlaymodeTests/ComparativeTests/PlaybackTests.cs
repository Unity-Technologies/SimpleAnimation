using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

public class PlaybackTests
{
    public class Play
    {
        [Test]
        public void Play_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			LogAssert.Expect(LogType.Error, new Regex(""));
			animation.Play("WrongState");
        }

        [Test]
        public void Play_ValidName_IsPlaying_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "RightName");
			animation.Play("RightName");
			Assert.IsTrue(animation.isPlaying, "The clip RightName should be playing");
		}

        [UnityTest]
        public IEnumerator Play_ValidName_MovesObject([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ValidName");
			animation.Play("ValidName");

			//TODO: replace by Seek(time)
			yield return null;
			yield return null;
			yield return null;

			Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.x, "LocalPosition.x should have been moved by the clip");
		}

        [UnityTest]
        public IEnumerator Play_OtherClip_IsPlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;
			var clipInstance2 = Object.Instantiate<AnimationClip>(clipInstance);
            clipInstance2.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ClipToPlayOver");
			animation.AddClip(clipInstance2, "ClipThatPlaysOver");
			animation.Play("ClipToPlayOver");
			yield return null;
			animation.Play("ClipThatPlaysOver");

			Assert.IsFalse(animation.IsPlaying("ClipToPlayOver"), "ClipToPlayOver should be stopped by playing ClipThatPlaysOver");
			Assert.IsTrue(animation.IsPlaying("ClipThatPlaysOver"), "ClipThatPlaysOver should now be the only playing clip");

			yield return null;
        }

		[UnityTest]
        public IEnumerator Play_SameClip_StateTime_DoesntChange([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "test");
			animation.Play("test");

            yield return null;

            float time = animation.GetState("test").time;
            animation.Play("test");
            Assert.AreEqual(time, animation.GetState("test").time);
		}

		[UnityTest]
        public IEnumerator Play_SameClip_Done_StateTime_Resets([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "test");
            animation.Play("test");

            yield return new WaitForSeconds(1.1f);

            float time = animation.GetState("test").time;
            animation.Play("test");
            Assert.AreEqual(0f, animation.GetState("test").time);
        }
    }

    public class PlayQueue_CompleteOthers
    {
        [Test]
        public void PlayQueue_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			LogAssert.Expect(LogType.Error, new Regex(""));
			animation.PlayQueued("InvalidName", QueueMode.CompleteOthers);
        }

        [UnityTest]
        public IEnumerator PlayQueue_WithNoClipPlaying_IsStatePlaying_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToPlayQueue");
			animation.PlayQueued("ToPlayQueue", QueueMode.CompleteOthers);

			yield return null;
			Assert.IsTrue(animation.IsPlaying("ToPlayQueue"));
        }

        [UnityTest]
        public IEnumerator PlayQueue_WithClipPlaying_IsStatePlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToPlay");
			animation.AddClip(clipInstance, "ToPlayQueue");
			animation.Play("ToPlay");
			animation.PlayQueued("ToPlayQueue", QueueMode.CompleteOthers);

			Assert.IsFalse(animation.IsPlaying("ToPlayQueue"));

			yield return null;
        }

        [UnityTest]
        public IEnumerator PlayQueue_WithClipPlaying_IsStatePlaying_AfterOriginalClipDone_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToPlay");
			animation.AddClip(clipInstance, "ToPlayQueue");
			animation.Play("ToPlay");
			animation.PlayQueued("ToPlayQueue", QueueMode.CompleteOthers);
			yield return new WaitForSeconds(1.1f);

			Assert.IsTrue(animation.IsPlaying("ToPlayQueue"));

			yield return null;
        }

        [UnityTest]
        public IEnumerator PlayQueue_Queueing_SameClip_AfterFirstClipIsDone_IsPlaying_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlayAndPlayQueue");
            animation.Play("ToPlayAndPlayQueue");
            animation.PlayQueued("ToPlayAndPlayQueue", QueueMode.CompleteOthers);
            yield return new WaitForSeconds(1.1f);

            Assert.IsTrue(animation.IsPlaying("ToPlayAndPlayQueue"));

            yield return null;
        }
    }

    public class PlayQueue_PlayNow
    {
 
    }

    public class Crossfade
    {

		[Test]
        public void Crossfade_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			LogAssert.Expect(LogType.Error, new Regex(""));
			animation.CrossFade("InvalidName", 0);

		}

        [Test]
        public void Crossfade_WithValidName_IsPlaying_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToCrossfade");
			animation.CrossFade("ToCrossfade", 0f);

			Assert.IsTrue(animation.IsPlaying("ToCrossfade"));
		}

        [UnityTest]
        public IEnumerator Crossfade_WithValidName_MovesObject([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToCrossfade");
			animation.CrossFade("ToCrossfade", 0f);

			yield return new WaitForSeconds(0.2f);
			Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.x);
        }

		[Test]
		public void Crossfade_LengthZero_StopsOtherClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
		{
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToPlay");
			animation.AddClip(clipInstance, "ToCrossfade");
			animation.CrossFade("ToPlay", 0f);
			animation.CrossFade("ToCrossfade", 0.0f);

			Assert.IsFalse(animation.IsPlaying("ToPlay"));
		}
		[Test]
        public void Crossfade_DoesntStopOtherClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "ToPlay");
			animation.AddClip(clipInstance, "ToCrossfade");
			animation.CrossFade("ToPlay", 0f);
			animation.CrossFade("ToCrossfade", 0.2f);

			Assert.IsTrue(animation.IsPlaying("ToPlay"));
		}

        [UnityTest]
        public IEnumerator CrossfadedOut_Clips_AreStopped([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlay");
            animation.AddClip(clipInstance, "ToCrossfade");
            animation.Play("ToPlay");
            animation.CrossFade("ToCrossfade", 0.1f);

            yield return new WaitForSeconds(0.2f);

            Assert.IsFalse(animation.IsPlaying("ToPlay"));
        }

        [UnityTest]
        public IEnumerator CrossfadedOut_Clips_AreTimeReset([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlay");
            animation.AddClip(clipInstance, "ToCrossfade");
            animation.Play("ToPlay");
            animation.CrossFade("ToCrossfade", 0.1f);

            yield return new WaitForSeconds(0.2f);

            Assert.AreEqual(0.0f, animation.GetState("ToPlay").normalizedTime);
        }

        [UnityTest]
        public IEnumerator Crossfade_MultipleTimes_DoesntReset_Crossfade_Duration([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clip2 = Resources.Load<AnimationClip>("LinearY");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            var clipInstance2 = Object.Instantiate<AnimationClip>(clip2);
            clipInstance.legacy = animation.usesLegacy;
            clipInstance2.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlay");
            animation.AddClip(clipInstance2, "ToCrossfade");
            animation.Play("ToPlay");
            animation.CrossFade("ToCrossfade", 0.2f);
            yield return new WaitForSeconds(0.1f);
            animation.CrossFade("ToCrossfade", 0.2f);
            yield return new WaitForSeconds(0.11f);
            Assert.AreEqual(0.0f, animation.GetState("ToPlay").weight);
            Assert.AreEqual(1.0f, animation.GetState("ToCrossfade").weight);
        }

        [UnityTest]
        [Ignore("Wrong Assumption; clips don't get kept alive by crossfade")]
        public IEnumerator Crossfade_FromFinishingClip_KeepsClipAlive_UntilCrossfadeDone([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clipX = Resources.Load<AnimationClip>("LinearX");
			var clipY = Resources.Load<AnimationClip>("LinearY");
			var xClip = Object.Instantiate<AnimationClip>(clipX);
			var yClip = Object.Instantiate<AnimationClip>(clipY);
			xClip.legacy = animation.usesLegacy;
			yClip.legacy = animation.usesLegacy;

			animation.AddClip(xClip, "ToPlay");
			animation.AddClip(yClip, "ToCrossfade");
			animation.Play("ToPlay");

			yield return new WaitForSeconds(0.9f);

			Assert.IsTrue(animation.IsPlaying("ToPlay"));
			Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.y);
			animation.CrossFade("ToCrossfade", 0.5f);
			yield return new WaitForSeconds(0.3f);

			Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.y);
			Assert.IsTrue(animation.IsPlaying("ToCrossfade"));

			yield return null;
        }

        [UnityTest]
        [Ignore("Wrong Assumption; clips don't get kept alive by crossfade")]
        public IEnumerator Crossfade_LongerThanClip_KeepsClipAlive_UntilCrossfadeDone([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipY = Resources.Load<AnimationClip>("LinearY");
            var xClip = Object.Instantiate<AnimationClip>(clipX);
            var yClip = Object.Instantiate<AnimationClip>(clipY);
            xClip.legacy = animation.usesLegacy;
            yClip.legacy = animation.usesLegacy;

            animation.AddClip(yClip, "ToPlay");
            animation.AddClip(xClip, "ToCrossfade");
            animation.Play("ToPlay");

            yield return new WaitForSeconds(0.9f);

            Assert.IsTrue(animation.IsPlaying("ToPlay"));
            Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.y);
            animation.CrossFade("ToCrossfade", 1.2f);
            yield return new WaitForSeconds(1.5f);

            Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.y);
            Assert.IsTrue(animation.IsPlaying("ToCrossfade"));
        }

    }

    public class CrossfadeQueue
    {
        [Test]
        public void CrossfadeQueue_WithNoClipAttached_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            LogAssert.Expect(LogType.Error, new Regex(""));
            animation.CrossFadeQueued("UnknownState", 0f, QueueMode.CompleteOthers); 
        }

        [UnityTest]
        public IEnumerator CrossfadeQueue_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlay");
            LogAssert.Expect(LogType.Error,  new Regex(""));
            animation.CrossFadeQueued("invalidName", 0f, QueueMode.CompleteOthers);

            yield return null;
        }

        [UnityTest]
        public IEnumerator CrossfadeQueue_WithNoClipPlaying_IsStatePlaying_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToPlay");
            animation.CrossFadeQueued("ToPlay", 0f, QueueMode.CompleteOthers);

            yield return null;
            Assert.IsTrue(animation.IsPlaying("ToPlay"));
        }

        [UnityTest]
        public IEnumerator CrossfadeQueue_WithClipPlaying_IsStatePlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipY = Resources.Load<AnimationClip>("LinearY");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            var clipInstanceY = Object.Instantiate<AnimationClip>(clipY);
            clipInstanceX.legacy = animation.usesLegacy;
            clipInstanceY.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceY, "ToPlay");
            animation.AddClip(clipInstanceX, "ToCrossfade");
            animation.Play("ToPlay");
            animation.CrossFadeQueued("ToCrossfade", 0.5f, QueueMode.CompleteOthers);
            yield return null;

            Assert.IsFalse(animation.IsPlaying("ToCrossfade"));
        }

        [UnityTest]
        public IEnumerator CrossfadeQueue_WithClipPlaying_IsStatePlaying_AfterOriginalClipDone_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipY = Resources.Load<AnimationClip>("LinearY");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            var clipInstanceY = Object.Instantiate<AnimationClip>(clipY);
            clipInstanceX.legacy = animation.usesLegacy;
            clipInstanceY.legacy = animation.usesLegacy;
        
            animation.AddClip(clipInstanceY, "ToPlay");
            animation.AddClip(clipInstanceX, "ToCrossfade");
            animation.Play("ToPlay");
            animation.CrossFadeQueued("ToCrossfade", 0.0f, QueueMode.CompleteOthers);
            yield return new WaitForSeconds(1.1f);

            Assert.IsTrue(animation.IsPlaying("ToCrossfade"));
        }
    }

    public class Stop
    {

        [UnityTest]
        public IEnumerator Stop_ValidName_NotPlaying_IsPlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);

            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            animation.AddClip(clipInstanceX, "ValidName");

            yield return null;

            animation.Stop("ValidName");
            Assert.IsFalse(animation.IsPlaying("ValidName"));
        }

        [UnityTest]
        public IEnumerator Stop_ValidName_Playing_IsPlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            animation.AddClip(clipInstanceX, "ValidName");
            animation.Play("ValidName");

            yield return null;

            animation.Stop("ValidName");
            Assert.IsFalse(animation.IsPlaying("ValidName"));
        }
        [UnityTest]
        public IEnumerator Stop_ValidName_DoesntMoveObject([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            animation.AddClip(clipInstanceX, "ValidName");
            animation.Play("ValidName");

            yield return null;

            Vector3 localPos = animation.gameObject.transform.localPosition;
            animation.Stop("ValidName");
            Assert.AreEqual(localPos, animation.gameObject.transform.localPosition);
        }

        [UnityTest]
        public IEnumerator Stop_WithName_OtherClip_IsPlaying_DoesntChange([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);

            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipY = Resources.Load<AnimationClip>("LinearY");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            var clipInstanceY = Object.Instantiate<AnimationClip>(clipY);
            clipInstanceX.legacy = animation.usesLegacy;
            clipInstanceY.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceY, "ClipToStop");
            animation.AddClip(clipInstanceX, "OtherClip");
            var stateToStop = animation.GetState("ClipToStop");
            var otherState = animation.GetState("OtherClip");

            stateToStop.weight = 1f;
            stateToStop.enabled = true;
            otherState.weight = 1f;
            otherState.enabled = true;

            yield return null;

            animation.Stop("ClipToStop");
            Assert.IsFalse(animation.IsPlaying("ClipToStop"));
            Assert.IsTrue(animation.IsPlaying("OtherClip"));
        }

        [UnityTest]
        public IEnumerator Stop_WithoutName_IsPlaying_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);

            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipY = Resources.Load<AnimationClip>("LinearY");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            var clipInstanceY = Object.Instantiate<AnimationClip>(clipY);
            clipInstanceX.legacy = animation.usesLegacy;
            clipInstanceY.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceY, "clip1");
            animation.AddClip(clipInstanceX, "clip2");

            animation.GetState("clip1").enabled = true;
            animation.GetState("clip1").weight = 0.5f;

            animation.GetState("clip2").enabled = true;
            animation.GetState("clip2").weight = 0.5f;

            Assert.IsTrue(animation.IsPlaying("clip1"));
            Assert.IsTrue(animation.IsPlaying("clip2"));
            yield return null;

            animation.Stop();
            Assert.IsFalse(animation.IsPlaying("clip1"));
            Assert.IsFalse(animation.IsPlaying("clip2"));
        }

        [UnityTest]
        public IEnumerator Stop_Playing_StoppedState_DoesntFire_AnyEvents([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var eventReceiver = animation.gameObject.AddComponent<SimpleAnimationTests.ReceivesEvent>();
            var clip = Resources.Load<AnimationClip>("FiresEvent");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "FiresEvent");
            animation.Play("FiresEvent");
            yield return new WaitForSeconds(0.6f);

            Assert.AreEqual(1, eventReceiver.eventCount, "Event at 0.5 should have fired");
            animation.Stop("FiresEvent");
            animation.Play("FiresEvent");
            Assert.AreEqual(0.0f, animation.GetState("FiresEvent").time, "Time should have reset to 0"); 
            Assert.AreEqual(1, eventReceiver.eventCount, "No new event should have fired");
            yield return null;
            Assert.AreEqual(1, eventReceiver.eventCount, "No new event should have fired after update");

        }
    }

    public class Rewind
    {
        [Test]
        public void Rewind_ValidName_NotPlaying_StateTime_IsZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            animation.AddClip(clipInstanceX, "ValidName");
            IAnimationState state = animation.GetState("ValidName");
            state.time = 0.5f;
            animation.Rewind("ValidName");
            Assert.AreEqual(0f, state.time);
        }

        [UnityTest]
        public IEnumerator Rewind_ValidName_Playing_StateTime_IsZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            animation.AddClip(clipInstanceX, "ValidName");
            animation.Play("ValidName");
            yield return new WaitForSeconds(0.5f);

            animation.Rewind("ValidName");
            IAnimationState state = animation.GetState("ValidName");
            Assert.AreEqual(0f, state.time);
        }
       

        [Test]
        public void Rewind_WithName_OtherClip_Time_DoesntChange([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToRewind");
            animation.AddClip(clipInstance, "ToLeaveAlone");
            IAnimationState toRewind = animation.GetState("ToRewind");
            toRewind.time = 0.5f;
            IAnimationState toLeaveAlone = animation.GetState("ToLeaveAlone");
            toLeaveAlone.time = 0.5f;

            animation.Rewind("ToRewind");
            Assert.AreNotEqual(0f, toLeaveAlone.time);
        }

        [Test]
        public void Rewind_WithoutName_AllStateTimes_AreZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "ToRewind");
            animation.AddClip(clipInstance, "ToRewindToo");
            IAnimationState toRewind = animation.GetState("ToRewind");
            toRewind.time = 0.5f;
            IAnimationState toRewindToo = animation.GetState("ToRewindToo");
            toRewindToo.time = 0.5f;

            animation.Rewind();
            Assert.AreEqual(0f, toRewind.time);
            Assert.AreEqual(0f, toRewindToo.time);
        }
    }

    public class Sample
    {
        [Test]
        public void Sample_DoesntChange_StateTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceX, "clip1");

            animation.GetState("clip1").enabled = true;
            animation.GetState("clip1").weight = 1f;
            animation.GetState("clip1").time = 0.5f;
            animation.Sample();
            
            Assert.AreEqual(0.5f, animation.gameObject.transform.localPosition.x);
            Assert.IsTrue(Mathf.Approximately(0.5f, animation.GetState("clip1").time));
        }

        [Test]
        public void Sample_EvaluatesAt_StateTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceX, "ToSample");
            IAnimationState state = animation.GetState("ToSample");

            state.enabled = true;
            state.weight = 1f;
            state.time = 0.5f;
            animation.Sample();

            Assert.AreEqual(0.5f, animation.gameObject.transform.localPosition.x);
        }
    }

    public class Blend
    {
        [Test]
        public void Blend_AfterBlend_Instant_State_IsPlaying([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceX, "ToBlend");
            animation.Blend("ToBlend", 1, 0);

            Assert.IsTrue(animation.IsPlaying("ToBlend"));
        }

        [Test]
        public void Blend_AfterBlend_NonInstant_State_IsPlaying([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceX, "ToBlend");
            animation.Blend("ToBlend", 1, 0.5f);

            Assert.IsTrue(animation.IsPlaying("ToBlend"));
        }

        [Test]
        public void Blend_DoesntChange_OtherState_Weight([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstance1 = Object.Instantiate<AnimationClip>(clipX);
            var clipInstance2 = Object.Instantiate<AnimationClip>(clipX);
            clipInstance1.legacy = animation.usesLegacy;
            clipInstance2.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance1, "ToBlend");
            animation.AddClip(clipInstance2, "ToLeaveAlone");
            animation.Play("ToLeaveAlone");
            animation.Blend("ToBlend", 1f, 0f);

            Assert.AreEqual(1f, animation.GetState("ToLeaveAlone").weight);
        }

        [UnityTest]
        public IEnumerator Blend_Instant_WithWeightZero_State_DoesntStop_State([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;

            animation.AddClip(clipInstanceX, "ToBlend");
            animation.Blend("ToBlend", 0, 0);
            yield return null;
            Assert.IsTrue(animation.IsPlaying("ToBlend"));
        }

    }

    public class PlayQueued
    {
        [UnityTest]
        public IEnumerator PlayQueued_Only_Plays_QueuedAnimations_Once([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clipX = Resources.Load<AnimationClip>("LinearX");
            var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceX.legacy = animation.usesLegacy;
            var clipInstanceXOnce = Object.Instantiate<AnimationClip>(clipX);
            clipInstanceXOnce.legacy = animation.usesLegacy;
            clipInstanceXOnce.wrapMode = WrapMode.Once;

            animation.AddClip(clipInstanceX, "A");
            animation.AddClip(clipInstanceX, "B");
            animation.AddClip(clipInstanceX, "C");

            animation.Play("A");
            animation.PlayQueued("B", QueueMode.CompleteOthers);
            animation.PlayQueued("C", QueueMode.CompleteOthers);
            yield return new WaitForSeconds(2.8f);            
            Assert.IsTrue(animation.IsPlaying("C"));
            yield return new WaitForSeconds(0.3f);
            Assert.IsFalse(animation.isPlaying);
        }
        //TODO: Crossfade and Play clears queued animations
        //TODO: Stop clears queued animations
    }

}
