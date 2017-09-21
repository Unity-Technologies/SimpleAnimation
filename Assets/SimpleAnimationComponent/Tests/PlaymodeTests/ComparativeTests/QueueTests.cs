using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

public class QueueTests
{
    [UnityTest]
    public IEnumerator Queue_Stop_Clip_StopsQueuedClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "PlayAndQueue");
        animation.Play("PlayAndQueue");
        animation.PlayQueued("PlayAndQueue", QueueMode.CompleteOthers);
        yield return null;
        animation.Stop("PlayAndQueue");
        Assert.IsFalse(animation.isPlaying);
        yield return null;
        //Queued animation would have started if it was going to.
        Assert.IsFalse(animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator State_Enabled_DoesntCover_QueuedState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "PlayAndQueue");
        animation.Play("PlayAndQueue");
        animation.PlayQueued("PlayAndQueue", QueueMode.CompleteOthers);
        IAnimationState state = animation.GetState("PlayAndQueue");
        Assert.IsTrue(state.enabled);
        yield return new WaitForSeconds(1.1f);
        Assert.IsFalse(state.enabled);
    }

    [UnityTest]
    public IEnumerator Queue_Looped_Clips_Block_QueuedAnimations([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Loop;

        animation.AddClip(clipInstance, "Play");
        animation.AddClip(clipInstance, "Queued");
        animation.Play("Play");
        animation.PlayQueued("Queued", QueueMode.CompleteOthers);
        yield return new WaitForSeconds(1.1f);
        Assert.IsFalse(animation.IsPlaying("Queued"));
    }
}
