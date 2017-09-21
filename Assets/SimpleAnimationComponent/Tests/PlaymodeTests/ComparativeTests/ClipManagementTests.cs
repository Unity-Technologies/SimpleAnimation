using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;

public class ClipManagementTests
{
    public class GetClipCount
    {
        [Test]
        public void GetClipCount_BeforeAdd_ReturnsZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            Assert.AreEqual(0, animation.GetClipCount(), "Component should have no clips connected at this point");
        }

        [Test]
        public void GetClipCount_AfterAddOne_ReturnsOne([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);

            clipInstance.legacy = animation.usesLegacy;
            animation.AddClip(clipInstance, "test");

            Assert.AreEqual(1, animation.GetClipCount(), "Component should have 1 clip connected after add");
        }

        [Test]
        public void GetClipCount_AfterRemoveSingleClip_ReturnsZero([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);

            clipInstance.legacy = animation.usesLegacy;
            animation.AddClip(clipInstance, "test");
            animation.RemoveClip("test");

            Assert.AreEqual(0, animation.GetClipCount(), "Component should have no clips after remove");
        }
    }

    public class AddClip
    {
        [Test]
        public void AddClip_WithNullClip_Throws_NullReferenceException([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            Assert.Throws<System.NullReferenceException> (() => { animation.AddClip(null, "test"); });
        }

        [Test]
        public void AddClip_TwiceWithSameName_GetClipCount_ReturnsOne([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "test");
            LogAssert.ignoreFailingMessages = true; //The error message here is irrelevant
            animation.AddClip(clipInstance, "test");
            LogAssert.ignoreFailingMessages = false;
            
            Assert.AreEqual(1, animation.GetClipCount(), "Component should have no clips after remove");
        }

        [Test]
        public void AddClip_TwiceDifferentName_GetClipCount_ReturnsTwo([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, "test");
            animation.AddClip(clipInstance, "test2");
            Assert.AreEqual(2, animation.GetClipCount(), "Component should have no clips after remove");
        }

        [Test]
        public void AddClip_WithSameName_AsClip_DoenstCreateNewClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, clipInstance.name);
            IAnimationState state = animation.GetState(clipInstance.name);
            Assert.AreEqual(clipInstance, state.clip, "Component should have no clips after remove");
        }
    }

    public class RemoveClip_ByAnimationClip
    {
        [Test]
        public void RemoveClip_AnimationClip_WithNullClip_Throws_NullReferenceException([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			AnimationClip clip = null;
			Assert.Throws<System.NullReferenceException> (() => { animation.RemoveClip(clip); });
        }

        [Test]
        [Description("AddClip always duplicates clips in the Animation Component, making it very hard to remove clips")]
        public void RemoveClip_AnimationClip_RemovesClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, clipInstance.name);
			animation.RemoveClip(clipInstance);
			Assert.AreEqual(0, animation.GetClipCount(), "Component should have no clips after remove");
		}

        [Test]
        public void RemoveClip_AnimationClip_DoesntRemoveUnrelatedClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
            var clip2 = Resources.Load<AnimationClip>("LinearY");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;
			var clipInstance2 = Object.Instantiate<AnimationClip>(clip2);
            clipInstance2.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, clipInstance.name);
			animation.AddClip(clipInstance2, clipInstance2.name);
			animation.RemoveClip(clipInstance);
			Assert.AreEqual(1, animation.GetClipCount(), "Component should still have 1 connected clip after remove");
            Assert.NotNull(animation.GetState(clipInstance2.name));
		}
    }

    public class RemoveClip_ByName
    {
        [Test]
        public void RemoveClip_ByName_WithEmptyName_Works([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "");
			animation.RemoveClip("");
			Assert.AreEqual(0, animation.GetClipCount(), "Component should still have 1 connected clip after remove");
		}
        
        [Test]
        public void RemoveClip_ByName_DoesntRemoveOtherClips([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;
			var legacyClip2 = Object.Instantiate<AnimationClip>(clipInstance);

			animation.AddClip(clipInstance, "test");
			animation.AddClip(legacyClip2, "test2");
			animation.RemoveClip("test");
			Assert.AreEqual(1, animation.GetClipCount(), "Component should still have 1 connected clip after remove");
        }

        [Test]
        public void RemoveClip_ByName_RemovesClip([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
			var clip = Resources.Load<AnimationClip>("LinearX");
			var clipInstance = Object.Instantiate<AnimationClip>(clip);
			clipInstance.legacy = animation.usesLegacy;

			animation.AddClip(clipInstance, "test");
			animation.RemoveClip("test");
			Assert.AreEqual(0, animation.GetClipCount(), "Component should still have 1 connected clip after remove");
		}
    }

    public class RemoveClip
    {
        [Test]
        public void RemoveClip_Invalidates_ExistingState([ValueSource(typeof(ComparativeTestFixture), "Sources")]System.Type type)
        {
            IAnimation animation = ComparativeTestFixture.Instantiate(type);
            var clip = Resources.Load<AnimationClip>("LinearX");
            var clipInstance = Object.Instantiate<AnimationClip>(clip);
            clipInstance.legacy = animation.usesLegacy;

            animation.AddClip(clipInstance, clipInstance.name);
            IAnimationState state = animation.GetState(clipInstance.name);
            animation.RemoveClip(clipInstance);
            Assert.IsFalse(state.isValid);
        }
    }

}
