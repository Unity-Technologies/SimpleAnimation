
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleAnimation))]
public class SimpleAnimationEditor : Editor
{
    static class Styles
    {
        public static GUIContent animation = new GUIContent("Animation", "The clip that will be played if Play() is called, or if \"Play Automatically\" is enabled");
        public static GUIContent animations = new GUIContent("Animations", "These clips will define the States the component will start with");
        public static GUIContent playAutomatically = new GUIContent("Play Automatically", "If checked, the default clip will automatically be played");
        public static GUIContent animatePhysics = new GUIContent("Animate Physics", "If checked, animations will be updated at the same frequency as Fixed Update");

        public static GUIContent cullingMode = new GUIContent("Culling Mode", "Controls what is updated when the object has been culled");
    }

    SerializedProperty clip;
    SerializedProperty states;
    SerializedProperty playAutomatically;
    SerializedProperty animatePhysics;
    SerializedProperty cullingMode;

    bool m_AnimationsExpanded = true;
    void OnEnable()
    {
        clip = serializedObject.FindProperty("m_Clip");
        states = serializedObject.FindProperty("m_States");
        playAutomatically = serializedObject.FindProperty("m_PlayAutomatically");
        animatePhysics = serializedObject.FindProperty("m_AnimatePhysics");
        cullingMode = serializedObject.FindProperty("m_CullingMode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(clip, Styles.animation);
        EditorGUILayout.PropertyField(states, Styles.animations, true);
        EditorGUILayout.PropertyField(playAutomatically, Styles.playAutomatically);
        EditorGUILayout.PropertyField(animatePhysics, Styles.animatePhysics);
        EditorGUILayout.PropertyField(cullingMode, Styles.cullingMode);


        serializedObject.ApplyModifiedProperties();
    }
}