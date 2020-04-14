/* =========================================================
   ---------------------------------------------------
   Project   :    Easy UI Input
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2018 All rights reserved.
   ========================================================== */

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace EasyUIInput.Editor
{
    [CustomEditor(typeof(AxisHandler))]
    public class AxisHandlerEditor : UnityEditor.Editor
    {
        private SerializedProperty e_AxisName;
        private SerializedProperty e_Background;
        private SerializedProperty e_Stick;
        private SerializedProperty e_IsResetStick;
        private SerializedProperty e_SetStickPosOnPress;

        private GUIStyle titleStyle = new GUIStyle();

        public void OnEnable()
        {
            e_AxisName = serializedObject.FindProperty("axisName");
            e_Background = serializedObject.FindProperty("background");
            e_Stick = serializedObject.FindProperty("stick");
            e_IsResetStick = serializedObject.FindProperty("isResetStick");
            e_SetStickPosOnPress = serializedObject.FindProperty("setStickPosOnPress");
            InitGUIStyle();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Axis Handler", titleStyle);
            GUILayout.Space(5);
            e_AxisName.stringValue = EditorGUILayout.TextField("Axis Name", e_AxisName.stringValue);
            e_Background.objectReferenceValue = (Image) EditorGUILayout.ObjectField("Background", e_Background.objectReferenceValue, typeof(Image), true);
            e_Stick.objectReferenceValue = (Image) EditorGUILayout.ObjectField("Stick", e_Stick.objectReferenceValue, typeof(Image), true);
            e_IsResetStick.boolValue = EditorGUILayout.Toggle("Reset Stick", e_IsResetStick.boolValue);
            e_SetStickPosOnPress.boolValue = EditorGUILayout.Toggle("Set Stick Pos On Press", e_SetStickPosOnPress.boolValue);
            GUILayout.Space(3);
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void InitGUIStyle()
        {
            titleStyle.alignment = TextAnchor.UpperCenter;
            titleStyle.fontStyle = FontStyle.Bold;
        }
    }
}