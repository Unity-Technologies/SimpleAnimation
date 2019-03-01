using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class BaseConditionsTests
{
	[UnityTest]
	public IEnumerator AnimatePhysics_False_DoesntApplyVelocity([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.InstantiateCube(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        var rb = animation.gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        animation.gameObject.GetComponent<BoxCollider>().enabled = false;
        animation.animatePhysics = false;
        animation.AddClip(clipInstance, "test");
        animation.Play("test");

        yield return null;
        yield return null;
        Assert.AreEqual(0f, rb.velocity.x);
        Assert.AreEqual(0f, rb.velocity.z);

	}

   

    [UnityTest]
    public IEnumerator CullingMode_AlwaysAnimate_Animates_InvisibleObject([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.InstantiateCube(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        
        animation.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        animation.AddClip(clipInstance, "test");
        animation.gameObject.GetComponent<MeshRenderer>().enabled = false;
        animation.Play("test");
        
        yield return null;
        yield return null;
        Assert.AreNotEqual(Vector3.zero, animation.gameObject.transform.localPosition);
    }

    [UnityTest]
    public IEnumerator CullingMode_CullCompletely_Doesnt_Animate_InvisibleObject([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.InstantiateCube(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.cullingMode = AnimatorCullingMode.CullCompletely;
        animation.AddClip(clipInstance, "test");
        animation.gameObject.GetComponent<MeshRenderer>().enabled = false;
        animation.Play("test");

        yield return null;
        yield return null;
        Assert.AreEqual(Vector3.zero, animation.gameObject.transform.localPosition);
    }

    [Test]
    public void IsPlaying_BeforePlay_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "test");
        Assert.AreEqual(false, animation.isPlaying);
    }

    [Test]
    public void IsPlaying_AfterPlay_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "test");
        animation.Play("test");
        Assert.AreEqual(true, animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator IsPlaying_AfterStop_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;

        animation.AddClip(clipInstance, "test");
        animation.Play("test");
        yield return null;
        animation.Stop();
        Assert.AreEqual(false, animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator IsPlaying_AfterClipDone_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Once;

        animation.AddClip(clipInstance, "test");
        animation.Play("test");
        yield return new WaitForSeconds(2f);
        Assert.AreEqual(false, animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator IsPlaying_AfterCrossfadeDone_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Once;

        animation.AddClip(clipInstance, "test");
        animation.AddClip(clipInstance, "test2");
        animation.Play("test");
        yield return new WaitForSeconds(0.2f);
        animation.CrossFade("test2", 1.5f);
        yield return new WaitForSeconds(2.0f);
        Assert.AreEqual(false, animation.isPlaying);
    }

    [Test]
    public void DefaultClip_DoesntPlay_When_IsPlayingAutomatically_IsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        animation.clip = clipInstance;
        animation.playAutomatically = false;

        Assert.AreEqual(false, animation.isPlaying);
        Assert.AreEqual(false, animation.IsPlaying(animation.clip.name));
    }

    [Test]
    public void DefaultClip_Plays_When_IsPlayingAutomatically_IsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        animation.clip = clipInstance;
        animation.playAutomatically = true;

        var newGO = Object.Instantiate<GameObject>(animation.gameObject);
        animation = newGO.GetComponent<IAnimation>();

        Assert.AreEqual(true, animation.isPlaying);
        var defaultName = animation.usesLegacy ? animation.clip.name : "Default";
        Assert.AreEqual(true, animation.IsPlaying(defaultName));
    }

    [Test]
    public void PlayAutomatically_HasNoEffect_WhenThereIsNo_DefaultClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        animation.playAutomatically = true;

        var newGO = Object.Instantiate<GameObject>(animation.gameObject);
        animation = newGO.GetComponent<IAnimation>();

        Assert.AreEqual(false, animation.isPlaying);
    }

    [Test]
    public void PlayAutomatically_WithNo_DefaultClip_HasNoEffect_OnOtherClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        animation.playAutomatically = true;
        animation.AddClip(clipInstance, clipInstance.name);

        var newGO = Object.Instantiate<GameObject>(animation.gameObject);
        animation = newGO.GetComponent<IAnimation>();

        Assert.AreEqual(false, animation.isPlaying);
    }

    [Test]
    public void PlayAutomatically_With_DefaultClip_HasNoEffect_OnOtherClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        animation.clip = clipInstance;
        animation.playAutomatically = true;
        animation.AddClip(clipInstance, "OtherClip");

        Assert.AreEqual(false, animation.IsPlaying("OtherClip"));
    }

    [Test]
    public void PlayAutomatically_BeforeSet_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        Assert.AreEqual(true, animation.playAutomatically);
    }

    [Test]
    public void PlayAutomatically_AfterSet_True_ReturnsTrue([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        animation.playAutomatically = true;
        Assert.AreEqual(true, animation.playAutomatically);
    }

    [Test]
    public void PlayAutomatically_AfterSet_False_ReturnsFalse([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        animation.playAutomatically = false;
        Assert.AreEqual(false, animation.playAutomatically);
    }

    [UnityTest]
    public IEnumerator WrapMode_Default_UsesClipSetting([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Loop;

        animation.wrapMode = WrapMode.Default;
        animation.AddClip(clipInstance, "test");
        animation.Play("test");
        
        yield return new WaitForSeconds(1.2f);
        Assert.AreEqual(true, animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator WrapMode_OverridesClipsWithDefaultSetting([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Default;

        animation.wrapMode = WrapMode.Loop;
        animation.AddClip(clipInstance, "test");
        animation.Play("test");

        yield return new WaitForSeconds(1.2f);
        Assert.AreEqual(true, animation.isPlaying);
    }

    [UnityTest]
    public IEnumerator WrapMode_DoesntOverrideClipsSetting([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
    {
        IAnimation animation = ComparativeTestFixture.Instantiate(type);
        var clip = Resources.Load<AnimationClip>("LinearX");
        var clipInstance = Object.Instantiate<AnimationClip>(clip);
        clipInstance.legacy = animation.usesLegacy;
        clipInstance.wrapMode = WrapMode.Once;

        animation.wrapMode = WrapMode.Loop;
        animation.AddClip(clipInstance, "test");
        animation.Play("test");

        yield return new WaitForSeconds(1.2f);
        Assert.AreEqual(false, animation.isPlaying);
    }
}
