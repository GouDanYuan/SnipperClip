using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.UI;
using UnityEditor;

[CanEditMultipleObjects, CustomEditor(typeof(PressButton), true)]
public class NewBehaviourScript : ButtonEditor
{
    private SerializedProperty m_PressDelay;
    private SerializedProperty m_Interval;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_PressDelay = base.serializedObject.FindProperty("pressDelay");
        m_Interval = base.serializedObject.FindProperty("interval");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        base.serializedObject.Update();
        m_PressDelay.floatValue = EditorGUILayout.FloatField("PressDelay", m_PressDelay.floatValue);
        m_Interval.floatValue = EditorGUILayout.FloatField("Interval", m_Interval.floatValue);
        base.serializedObject.ApplyModifiedProperties();
    }
}
