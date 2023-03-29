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

            var f = property.FindPropertyRelative("f");
            var z = property.FindPropertyRelative("z");
            var r = property.FindPropertyRelative("r");

            EditorGUILayout.PropertyField(f);
            EditorGUILayout.PropertyField(z);
            EditorGUILayout.PropertyField(r);
            
            if (f.floatValue <= 0.1f)
            {
                f.floatValue = 0.1f;
            }
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}