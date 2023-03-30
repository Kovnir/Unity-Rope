using System.Reflection;
using Kovnir.Rope.Math;
using UnityEditor;
using UnityEngine;

namespace Kovnir.Rope.Editor
{
    [CustomPropertyDrawer(typeof(SecondOrderCalculator.Params))]
    public sealed class SecondOrderDynamicsParamsEditorDrawer : PropertyDrawer
    {
        static Material material;
        private MethodInfo manualUpdate;

        private void LazyInitMaterial()
        {
            if (material == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                material = new Material(shader);
            }

            if (manualUpdate == null)
            {
                manualUpdate =
                    typeof(SecondOrderCalculator).GetMethod("Update",
                        BindingFlags.NonPublic | BindingFlags.Static);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LazyInitMaterial();
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.indentLevel++;
            SecondOrderCalculator.Consts consts = DrawMainProperties(property);
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            Rect frameSize = GetCurrentRect();
            frameSize.yMin = 0;
            frameSize.yMax = 200;
            Rect clipRect = GUILayoutUtility.GetRect(frameSize.xMin, frameSize.xMax, frameSize.yMin, frameSize.yMax);
            if (Event.current.type == EventType.Repaint)
            {
                DrawGraph(consts, material, clipRect, frameSize);
            }


            EditorGUI.EndProperty();
        }

        private static SecondOrderCalculator.Consts DrawMainProperties(SerializedProperty property)
        {
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

            return SecondOrderCalculator.Consts.Create(
                new SecondOrderCalculator.Params(frequency.floatValue, damping.floatValue, response.floatValue));
        }

        private static Rect GetCurrentRect()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect frameSize = GUILayoutUtility.GetLastRect();
            return frameSize;
        }


        private void DrawGraph(SecondOrderCalculator.Consts consts, Material material, Rect clipRect, Rect frameSize)
        {
            const int BORDER = 3;

            GUI.BeginClip(clipRect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            GLDraw.Rect(0, 0, frameSize.width, frameSize.yMax, Color.black);
            GLDraw.EmptyRect(0, 0, frameSize.width, frameSize.yMax, BORDER, Color.gray);

            frameSize.width -= BORDER;
            frameSize.y += BORDER;
            frameSize.height -= BORDER * 2;
            frameSize.x += BORDER;

            GLDraw.Line(BORDER, 100, frameSize.width, 100, new Color(0.7f, 0.7f, 0.7f));

            const int THRESHOLD = 100;

            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150), new Vector2(THRESHOLD, 150),
                new Vector2(THRESHOLD, 50), new Vector2(frameSize.width, 50));
            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150 + 1), new Vector2(THRESHOLD + 1, 150 + 1),
                new Vector2(THRESHOLD + 1, 50 + 1), new Vector2(frameSize.width, 50 + 1));


            Vector3 targetPosition = new(0, 1.5f, 0);
            Vector3 prevTargetPosition = new(0, 1.5f, 0);
            Vector3 currPosition = new(0, 1.5f, 0);
            Vector3 currVelocity = new(0, 0, 0);

            Vector2[] points = new Vector2[(int)frameSize.width];
            for (int i = 0; i < (int)frameSize.width; i++)
            {
                if (i > THRESHOLD)
                {
                    targetPosition = new(0, 0.5f, 0);
                }

                (currPosition, currVelocity) = ((Vector3, Vector3))manualUpdate
                    .Invoke(null, new object[]{0.01f, targetPosition, prevTargetPosition,
                        currPosition, currVelocity, consts});
                prevTargetPosition = targetPosition;
                float currentPositionY = currPosition.y;
                points[i] = new Vector2(BORDER + i, currentPositionY * 100);
            }

            GLDraw.Lines(Color.green, frameSize, points);

            GL.PopMatrix();
            GUI.EndClip();
        }
    }
}