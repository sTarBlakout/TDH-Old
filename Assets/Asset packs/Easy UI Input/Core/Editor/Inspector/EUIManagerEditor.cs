/* =========================================================
   ---------------------------------------------------
   Project   :    Next-Gen FPS
   Publisher :    Infinite Dawn
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright © Infinite Dawn 2017 - 2018 All rights reserved.
   ========================================================== */

using UnityEditor;
using UnityEngine;

namespace EasyUIInput.Editor
{
    [CustomEditor(typeof(EUIManager))]
    public class EUIManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty e_Axis;
        private SerializedProperty e_Buttons;

        private void OnEnable()
        {
            e_Axis = serializedObject.FindProperty("axis");
            e_Buttons = serializedObject.FindProperty("buttons");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("EUI Manager", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(e_Axis, true);
            EditorGUILayout.PropertyField(e_Buttons, true);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Register New Inputs", EditorStyles.miniButtonLeft, GUILayout.Width(150), GUILayout.Height(17)))
            {
                AddInputs();
            }
            if(GUILayout.Button("Re-Register All Input", EditorStyles.miniButtonMid, GUILayout.Width(150), GUILayout.Height(17)))
            {
                EUIManager.Instance.GetAxis().Clear();
                EUIManager.Instance.GetButtons().Clear();
                AddInputs();
            }
            if(GUILayout.Button("Clear All Inputs", EditorStyles.miniButtonRight, GUILayout.Width(150), GUILayout.Height(17)))
            {
                EUIManager.Instance.GetAxis().Clear();
                EUIManager.Instance.GetButtons().Clear();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        public void AddInputs()
        {
            AxisHandler[] axis = FindObjectsOfType<AxisHandler>();
                for (int i = 0; i < axis.Length; i++)
                {
                    if (!EUIManager.Instance.GetAxis().Contains(axis[i]))
                    {
                        EUIManager.Instance.GetAxis().Add(axis[i]);
                    }
                }

                ButtonHandler[] buttons = FindObjectsOfType<ButtonHandler>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    if(!EUIManager.Instance.GetButtons().Contains(buttons[i]))
                    {
                        EUIManager.Instance.GetButtons().Add(buttons[i]);
                    }
                }
        }
    }
}