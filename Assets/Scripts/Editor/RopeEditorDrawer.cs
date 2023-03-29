using DefaultNamespace;
using UnityEditor;
using UnityEngine;

namespace GLHelper
{
    [CustomEditor(typeof(Rope))]
    public sealed class RopeEditorDrawer : UnityEditor.Editor
    {

        Material material;

        private void OnEnable()
        {
            var shader = Shader.Find("Hidden/Internal-Colored");
            material = new Material(shader);
        }

        private void OnDisable()
        {
            DestroyImmediate(material);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect frameSize = GUILayoutUtility.GetLastRect();
            frameSize.yMin = 0;
            frameSize.yMax = 200;

            Rect clipRect = GUILayoutUtility.GetRect(frameSize.xMin, frameSize.xMax, frameSize.yMin, frameSize.yMax);
            if (Event.current.type == EventType.Repaint)
            {
                Rope rope = target as Rope;
                rope.DrawInspector(material, clipRect, frameSize);
            }
        }
    }
}