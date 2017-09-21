using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

public class MiscTests
{
    [UnityTest]
    public IEnumerator StateSpeed_Affects_WhenCrossfadeHappens([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "PlaySlowly");
        animation.AddClip(clipInstance, "Queued");
        IAnimationState state = animation.GetState("PlaySlowly");
        state.enabled = true;
        state.speed = 0.1f;
        animation.PlayQueued("Queued", QueueMode.CompleteOthers);

        //Wait for the original length of PlaySlowly
        yield return new WaitForSeconds(1.1f);
        Assert.IsFalse(animation.IsPlaying("Queued"), "Clip 'Queued' should not be playing yet. Speed is probably applied wrong.");
        state.speed = 1000.0f;
        yield return null;
        yield return null;
        Assert.IsTrue(animation.IsPlaying("Queued"), "Clip 'PlaySlowly' should now be done, and clip 'Queued' should have started playing.");
    }

    [UnityTest]
    public IEnumerator PlayQueue_WithLoopedAnimation_Prevents_StateAccess_OfOriginalState_FromWorking_Correctly([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        var loopedClipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        loopedClipInstance.legacy = animation.usesLegacy;
        loopedClipInstance.wrapMode = WrapMode.Loop;

        animation.AddClip(clipInstance, "FirstClip");
        animation.AddClip(loopedClipInstance, "LoopedClip");
        animation.Play("FirstClip");
        animation.PlayQueued("LoopedClip", QueueMode.CompleteOthers);
        yield return new WaitForSeconds(1.1f);
        Assert.IsTrue(animation.IsPlaying("LoopedClip"), "Clip 'LoopedClip' should be playing");
        IAnimationState state = animation.GetState("LoopedClip");
        
        Assert.IsFalse(state.enabled, "We should be playing a copy of LoopedClip, not the LoopedClip State");
        yield return new WaitForSeconds(1.1f);
        state = animation.GetState("LoopedClip");
        Assert.IsFalse(state.enabled, "We should still be playing a copy of LoopedClip, not the LoopedClip State");
    }
}
