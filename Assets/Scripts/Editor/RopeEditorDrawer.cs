using System.Reflection;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace GLHelper
{
    [CustomEditor(typeof(Rope))]
    public sealed class RopeEditorDrawer : UnityEditor.Editor
    {
        SerializedProperty ropeStart;
        SerializedProperty ropeEnd;
        SerializedProperty length;
        SerializedProperty segmentsCount;
        SerializedProperty dynamicsParams;


        Material material;

        private void OnEnable()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            material = new Material(shader);

            ropeStart = serializedObject.FindProperty("ropeStart");
            ropeEnd = serializedObject.FindProperty("ropeEnd");
            length = serializedObject.FindProperty("length");
            segmentsCount = serializedObject.FindProperty("segmentsCount");
            dynamicsParams = serializedObject.FindProperty("dynamicsParams");
        }

        private void OnDisable()
        {
            DestroyImmediate(material);
        }

        public override void OnInspectorGUI()
        {
            DrawMainFields();

            EditorGUILayout.Space();
            Rect frameSize = GetCurrentRect();
            frameSize.yMin = 0;
            frameSize.yMax = 200;

            Rect clipRect = GUILayoutUtility.GetRect(frameSize.xMin, frameSize.xMax, frameSize.yMin, frameSize.yMax);
            if (Event.current.type == EventType.Repaint)
            {
                Rope rope = target as Rope;
                rope.DrawInspector(material, clipRect, frameSize);
            }
        }

        private static Rect GetCurrentRect()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect frameSize = GUILayoutUtility.GetLastRect();
            return frameSize;
        }

        private void DrawMainFields()
        {
            serializedObject.Update();

            //custom editor for main fields of Rope
            EditorGUILayout.PropertyField(ropeStart);
            EditorGUILayout.PropertyField(ropeEnd);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(length);
            if (length.floatValue < 0)
            {
                length.floatValue = 0;
            }

            EditorGUILayout.IntSlider(segmentsCount, 3, 50);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(dynamicsParams);
            if (EditorGUI.EndChangeCheck())
            {
                dynamicsParams.serializedObject.ApplyModifiedProperties();
                Rope rope = (Rope)target;
                rope.GetType().GetMethod("InitDynamics", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(rope, null);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}