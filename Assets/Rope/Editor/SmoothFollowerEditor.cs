using System.Reflection;
using Rope;
using UnityEditor;
using UnityEngine.Device;

namespace Kovnir.Rope.Editor
{
    [CustomEditor(typeof(SmoothFollower))]
    public sealed class SmoothFollowerEditor : UnityEditor.Editor
    {
        SerializedProperty targetProperty;
        SerializedProperty dynamicsParams;
        
        private MethodInfo initDynamics;

        private void OnEnable()
        {
            targetProperty = serializedObject.FindProperty("target");
            dynamicsParams = serializedObject.FindProperty("dynamicsParams");
            
            SmoothFollower smoothFollower = (SmoothFollower)target;
            initDynamics = smoothFollower.GetType().GetMethod("InitDynamics", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            if (targetProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("You need to setup Target transform.", MessageType.Error);
            }
            
            EditorGUILayout.PropertyField(targetProperty);
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(dynamicsParams);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying)
            {
                dynamicsParams.serializedObject.ApplyModifiedProperties();
                initDynamics
                    .Invoke(target, null);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}