# Simple Animation Component
## Or, what am I supposed to do with Animation Playables?

### What is it?
The Simple Animation Component is an animation component implemented in C# using the Playables API. It emulates much of the interface and behaviour of the Animation Component, but lets you use Generic or Humanoid clips, which can also be reused with Timeline and the Animator Controller.

### Who is it for?
* Animation Component users who would like to easily reuse their clips in Timelines
* Animator Component users who would like a simpler, more straightforward way of playing non-Legacy clips.
* Users looking for an example how to use Animation Playables in a real-world use case.
* Users looking to make their own custom Animation component.

### How does it work?
The SimpleAnimation Component is made up of three parts
* The Component itself
* The PlayableGraph
* The SimpleAnimationPlayable

#### Simple Animation Component
The component is in charge of providing the interface for this system, both in terms of User Interface (UI) and Scripting Interface (Scripting API). 

In the Inspector, users can pre-define states, set a default clip and configure the behaviour of the component, in a way that’s analogous to the Animation Component.

The component then uses this information to create and manage a PlayableGraph, which is in charge of feeding the correct Animation data to the Animator Component. 

Note: The Animator Component is currently a required dependency for the SimpleAnimation Component, and Animation Playable Graphs in general. You can simply collapse it and ignore it; SimpleAnimation Component should offer the all necessary functionality.

On the scripting side, the interface of the Simple Animation Component is also very similar to the interface of the Animation Component. From the scripting interface, users can Add, Remove, Play, Stop or configure states, which will result in the necessary changes to the PlayableGraph.

#### PlayableGraph

Just like the Animator Component, the SimpleAnimation Component uses a PlayableGraph internally to manage AnimationClips and transitions between them. Where the Animator Component uses an AnimatorControllerPlayable as the main orchestrating Playable, the SimpleAnimation Component uses the SimpleAnimationPlayable to orchestrate its operations.

The SimpleAnimation Component generates a PlayableGraph that has the SimpleAnimationPlayable as its root playable, and connects the output of that graph to the required Animator Component for Animation playback.

Under the SimpleAnimationPlayable is an AnimationMixerPlayable, in charge of blending between the different states during transitions. Then, under the mixer will be as many AnimationClipPlayables as there currently are States in the SimpleAnimation Component.

#### SimpleAnimationPlayable
The SimpleAnimationPlayable is where almost all the processing happens. It is in charge of keeping track of currently playing States, queued States, and the different crossfades. It is designed to be used in conjunction with the SimpleAnimation Component, but since most of the logic of the Component is actually implemented by this Playable, it could be reused independently of the SimpleAnimation Component with very little effort.

Every frame, the SimpleAnimationPlayable does the following:
* Checks for states that have finished playing
* Updates the weights of the different AnimationClipPlayables according to current crossfading operations in progress
* Checks if there are new queued states to start
* Checks if all states are done, and notifies the Component if it’s the case, so the component can stop the graph if nothing needs to be done anymore.

### How compatible is it with the Animation Component?
While the interface of the components are very similar, there are still a few differences that might not make this a simple “rename and go” experience:

#### SimpleAnimation Component doesn’t support Layers
The goal in making this new component was to make a new animation component that was compatible with the Animator clips, and was optimal for users looking for a simpler way to animate objects. The Layer system in the Animation component adds a lot of complexity to a component that’s otherwise very simple. We believe that once you hit that level of complexity, you’ll probably be better served by using the AnimatorController. This is why all the layer parameters were removed from the SimpleAnimationComponent’s methods.

If you want to use the SimpleAnimationPlayable with Layers, you could either implement layers in the SimpleAnimationPlayable by adding an AnimationLayerMixerPlayable between the SimpleAnimationPlayable and the AnimationMixerPlayable(s), or you could have multiple SimpleAnimationPlayables connected as inputs to a single AnimationLayerMixerPlayable.

Both those options will require you to make changes to the way the component and the playable operates.


#### SimpleAnimation doesn’t support only writing the values of the current clip by default
When animating with the Animation Component, only the properties modified by the curves of the playing clips are modified, whereas when using the Animator, all the properties of all the clips currently connected are written, whether the clips are playing or not.
The Animation Component approach lets you do some more advanced things, because it doesn’t lock all the values, but the tradeoff is some values might be left in a non-deterministic state when animations get interrupted.
The Animator Component approach, on the other hand, is deterministic, so none of your properties will ever be left in an undefined state if you interrupt the animation, but the flipside is that all the values that it tracks are effectively locked by the animator, and can’t easily be modified by script.
Since the SimpleAnimation Component relies on the Animator Component for animation playback, it also shares this limitation. If you want to emulate the behaviour of the Animation Component, then all clips need to be disconnected from the graph when not playing. Setting the boolean property keepStoppedPlayablesConnected to false on the SimpleAnimationPlayable will do this for you, at a large cost of performance whenever new clips start and end. Use at your own risk.


#### SimpleAnimation Component is not compatible with Legacy clips
You will need to convert your Legacy clips to Generic. You can do this in the model importer inspector for imported clips, and setting AnimationClip.legacy to false using scripts, or via the debug inspector for non-imported clips.


#### SimpleAnimation Component is not compatible with PingPong WrapMode
PingPong was never implemented in the Animator, and therefore is not supported by SimpleAnimation.

For a more hands-on look at how the APIs map one to another, you can take a look at SimpleAnimationProxy.cs and AnimationProxy.cs, in Assets/SimpleAnimationComponent/Tests, which both wrap their respective components to implement the unified interface IAnimation.

### Is it fast?
SimpleAnimation is, in most cases faster than using the AnimatorController.
Otherwise, it is generally a bit slower than the Animation Component, except in cases where the AnimatorController would also be faster than the Animation Component (usually when using large clips).

The additional cost of using the Animator comes from the different additional features offered by the Animator: Root Motion, script callbacks, compatibility with Humanoid and Timeline.

One area where the SimpleAnimation Component is quite faster than the Animator using an AnimatorController is when animating large amounts of objects, but animating them rarely. 

Like the AnimationComponent, when all the clips are done, the component disables itself and stops evaluating and writing. The AnimatorController, on the other hand, being a state machine, is always evaluating and writing, which means objects that are not actively animated are still evaluated and updated.

To compare the performance of all three (Animator with AnimatorController, Animation Component, and SimpleAnimation Component), see the performance tests in Assets/SimpleAnimationComponent/Tests/PerformanceTests.

### Is it stable?
The project comes with over 100 comparative Playmode Tests that validate that the SimpleAnimation Component behaves the same way as the Animation Component. It also comes with additional tests for features that are found only on the SimpleAnimation Component.

There is also a category of tests called DifferenceTests, which contains tests that highlight cases where the SimpleAnimation Component behaves differently from the Animation Component.

### What’s next?
* Creating a SimpleAnimationPlayableAsset, so SimpleAnimation Component configurations can be saved outside of the component itself.
* Making SimpleAnimation Component compatible with PlayableAssets, or with Playables, so it can play Timelines or other Animation Playables.
* Support all the WrapModes (will need internal Unity support)
* Support not writing all values all the time with minimal cost (will need internal Unity support)
