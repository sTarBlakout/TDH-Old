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

namespace EasyUIInput.Editor
{
    [CustomEditor(typeof(ButtonHandler))]
    public class ButtonHandlerEditor : UnityEditor.Editor
    {
        private SerializedProperty e_ButtonName;

        private GUIStyle titleStyle = new GUIStyle();

        private void OnEnable()
        {
            e_ButtonName = serializedObject.FindProperty("buttonName");
            InitGUIStyle();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Button Handler", titleStyle);
            GUILayout.Space(5);
            e_ButtonName.stringValue = EditorGUILayout.TextField("Name", e_ButtonName.stringValue);
            GUILayout.Space(5);
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