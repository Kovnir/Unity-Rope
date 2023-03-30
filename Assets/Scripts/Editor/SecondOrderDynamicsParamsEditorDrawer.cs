using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(SecondOrderDynamics.Params))]
    public sealed class SecondOrderDynamicsParamsEditorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 1;

            var frequency = property.FindPropertyRelative("Frequency");
            var damping = property.FindPropertyRelative("Damping");
            var response = property.FindPropertyRelative("Response");

            EditorGUILayout.PropertyField(frequency);
            EditorGUILayout.PropertyField(damping);
            EditorGUILayout.PropertyField(response);
            
            if (frequency.floatValue <= 0.1f)
            {
                frequency.floatValue = 0.1f;
            }
            
            if (damping.floatValue <= 0.1f)
            {
                damping.floatValue = 0.1f;
            }
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}