using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ComparativeTestFixture
{
    private static System.Type[] Sources()
    {
        return new Type [] { typeof(AnimationProxy), typeof(SimpleAnimationProxy) };
    }

    public static IAnimation Instantiate(System.Type type)
    {
        var go = new GameObject();
        var component = go.AddComponent(type);
        return component as IAnimation;
    }

    public static IAnimation InstantiateCube(System.Type type)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var component = go.AddComponent(type);
        return component as IAnimation;
    }
}
