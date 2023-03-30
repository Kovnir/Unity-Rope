using System.Reflection;
using UnityEditor;

namespace Kovnir.Rope.Editor
{
    [CustomEditor(typeof(Rope))]
    public sealed class RopeEditorDrawer : UnityEditor.Editor
    {
        SerializedProperty ropeStart;
        SerializedProperty ropeEnd;
        SerializedProperty length;
        SerializedProperty segmentsCount;
        SerializedProperty dynamicsParams;

        private MethodInfo initRopeDynamics;

        private void OnEnable()
        {
            ropeStart = serializedObject.FindProperty("ropeStart");
            ropeEnd = serializedObject.FindProperty("ropeEnd");
            length = serializedObject.FindProperty("length");
            segmentsCount = serializedObject.FindProperty("segmentsCount");
            dynamicsParams = serializedObject.FindProperty("dynamicsParams");
            
            Rope rope = (Rope)target;
            initRopeDynamics = rope.GetType().GetMethod("InitDynamics", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawMainFields();
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(dynamicsParams);
            if (EditorGUI.EndChangeCheck())
            {
                dynamicsParams.serializedObject.ApplyModifiedProperties();
                initRopeDynamics
                    .Invoke(target, null);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMainFields()
        {
            EditorGUILayout.PropertyField(ropeStart);
            EditorGUILayout.PropertyField(ropeEnd);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(length);
            if (length.floatValue < 0)
            {
                length.floatValue = 0;
            }

            EditorGUILayout.IntSlider(segmentsCount, 3, 50);
        }
    }
}