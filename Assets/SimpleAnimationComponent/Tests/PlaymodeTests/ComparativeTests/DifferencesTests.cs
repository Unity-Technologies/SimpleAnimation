using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

public class DifferencesTests
{
    [Test]
    [Ignore("The Animation Component creates a new internal clip instance when a state has a different name than the name of the clip. This was deemed an undesirable behavior")]
    public void AddClip_WithNewName_CreatesNewClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "NewName");
        IAnimationState state = animation.GetState("NewName");
        Assert.AreNotEqual(clipInstance, state.clip, "AddClip should have created a new clip instance");
    }

    [Test]
    [Ignore("This is where the new component differs. Animation won't let you remove multiple states with the same clip, because it's not possible to have the same clip twice")]
    public void RemoveClip_AnimationClip_RemovesAllInstances([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "test");
        animation.AddClip(clipInstance, "test2");
        animation.RemoveClip(clipInstance);
        Assert.AreEqual(0, animation.GetClipCount(), "Component should have no clips after remove");
    }

    [UnityTest]
    [Ignore("Time does not advance on the frame on which Rewind is called on SimpleAnimation")]
    public IEnumerator Rewind_PlaysFrameZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clipX = Resources.Load<AnimationClip>("LinearX");
        var clipInstanceX = Object.Instantiate<AnimationClip>(clipX);
        clipInstanceX.legacy = animation.usesLegacy;
        animation.AddClip(clipInstanceX, "ValidName");
        animation.Play("ValidName");
        yield return new WaitForSeconds(0.5f);
        Assert.AreNotEqual(0f, animation.gameObject.transform.localPosition.x);
        animation.Rewind("ValidName");
        yield return null;
        Assert.AreEqual(0f, animation.gameObject.transform.localPosition.x);
    }

    [Test]
    [Ignore("Check were added to SimpleAnimation to prevent using invalid names")]
    public void Rewind_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        LogAssert.Expect(LogType.Error, new Regex(""));
        animation.Rewind("InvalidName");
    }

    [Test]
    [Ignore("Check were added to SimpleAnimation to prevent using invalid names")]
    public void Stop_WithInvalidName_FailsOnCall([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        LogAssert.Expect(LogType.Error, new Regex(""));
        animation.Stop("InvalidName");
    }

    [Test]
    [Ignore("The Animation Component accepts null as a valid name")]
    public void State_Name_NullString_Throws_ArgumentNullException([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "ValidName");
        IAnimationState state = animation.GetState("ValidName");
        Assert.Throws<System.ArgumentNullException>(() => { state.name = null; });
    }

    [UnityTest]
    [Ignore("Setting time guarantees you that the next automatic evaluation will use the time you supplied with Playables, whereas Animation Component will update the frame time")]
    public IEnumerator State_Time_SetTime_PreventsUpdatingTimeAutomatically([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "ValidName");
        IAnimationState state = animation.GetState("ValidName");
        animation.Play(state.name);
        float time = Time.time;
        state.time = 0.1f; //empty run for the Animation component, probably a side effect of reenabling the component
        yield return null;

        state.time = 0.1f;
        yield return null;

        Assert.AreEqual(0.1f, state.time);
    }

    [UnityTest]
    [Ignore("The Animation Component doesn't advance on the first frame")]
    public IEnumerator State_Time_IsSynchronizedWith_GameTime([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
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
        yield return null;
        yield return null;
        float elapsedTime = Time.time - previousTime;
        Assert.AreEqual(elapsedTime, state.time, 0.001f, "State time should be equal to elapsed time");
    }

    [UnityTest]
    [Ignore("In the Animation Component, RemoveClip doesn't remove queued instances of the removed clip, whereas Stop stops both the queued instances and the playing instances. This inconsistency was deemed undesirable")]
    public IEnumerator Queue_RemoveClip_StopsQueuedClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "PlayAndQueue");
        animation.Play("PlayAndQueue");
        animation.PlayQueued("PlayAndQueue", QueueMode.CompleteOthers);
        yield return null;
        animation.RemoveClip("PlayAndQueue");
        Assert.IsFalse(animation.isPlaying);
        yield return null;
        Assert.IsFalse(animation.isPlaying);
    }

    [UnityTest]
    [Ignore("States that play backwards should still be Queue-compatible, which is not the case in the Animation Component")]
    public IEnumerator NegativeSpeed_Does_Trigger_Crossfade([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "PlayBackwards");
        animation.AddClip(clipInstance, "Crossfade");
        IAnimationState state = animation.GetState("PlayBackwards");
        state.enabled = true;
        state.time = 0.5f;
        state.speed = -1f;
        animation.PlayQueued("Crossfade", QueueMode.CompleteOthers);
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(state.enabled);
        Assert.IsTrue(animation.IsPlaying("Crossfade"));
    }

    [UnityTest]
    [Ignore("The Animation Component doesn't apply velocities to rigidbodies with AnimatePhysics on")]
    public IEnumerator AnimatePhysics_True_AppliesVelocity([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.InstantiateCube(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        var rb = animation.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        animation.animatePhysics = true;
        animation.AddClip(clipInstance, "test");
        animation.Play("test");

        yield return null;
        yield return new WaitForSeconds(0.3f);
        Assert.AreNotEqual(rb.velocity, Vector3.zero);
    }
}
