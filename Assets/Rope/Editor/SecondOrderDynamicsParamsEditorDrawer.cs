using System;
using System.Reflection;
using Kovnir.Rope.Math;
using UnityEditor;
using UnityEngine;

namespace Kovnir.Rope.Editor
{
    [CustomPropertyDrawer(typeof(SecondOrderCalculatorParams))]
    public sealed class SecondOrderDynamicsParamsEditorDrawer : PropertyDrawer
    {
        private const int OFFSET = 2;
        private const int GRAPH_SIZE = 200;

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
                    typeof(SecondOrderCalculator<Vector3>).GetMethod("Update",
                        BindingFlags.NonPublic | BindingFlags.Static);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + OFFSET) * 3
                   + EditorGUIUtility.singleLineHeight * 1.5f
                   + GRAPH_SIZE;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LazyInitMaterial();

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.indentLevel++;

            Vector3SecondOrderCalculator.Consts consts = DrawMainProperties(ref position, property);

            position.y += EditorGUIUtility.singleLineHeight * 1.5f;
            position.height = GRAPH_SIZE;

            if (Event.current.type == EventType.Repaint)
            {
                DrawGraph(consts, material, position);
            }

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        private static Vector3SecondOrderCalculator.Consts DrawMainProperties(ref Rect position,
            SerializedProperty property)
        {
            var frequency = property.FindPropertyRelative("Frequency");
            var damping = property.FindPropertyRelative("Damping");
            var response = property.FindPropertyRelative("Response");

            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.singleLineHeight + OFFSET;
            EditorGUI.PropertyField(position, frequency);
            position.y += EditorGUIUtility.singleLineHeight + OFFSET;
            EditorGUI.PropertyField(position, damping);
            position.y += EditorGUIUtility.singleLineHeight + OFFSET;
            EditorGUI.PropertyField(position, response);

            if (frequency.floatValue <= 0.1f)
            {
                frequency.floatValue = 0.1f;
            }

            if (damping.floatValue <= 0.1f)
            {
                damping.floatValue = 0.1f;
            }

            return Vector3SecondOrderCalculator.Consts.Create(
                new SecondOrderCalculatorParams(frequency.floatValue, damping.floatValue, response.floatValue));
        }

        private void DrawGraph(Vector3SecondOrderCalculator.Consts consts, Material material, Rect clipRect)
        {
            const int BORDER = 3;

            GUI.BeginClip(clipRect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            GLDraw.Rect(0, 0, clipRect.width, clipRect.height, Color.black);
            GLDraw.EmptyRect(0, 0, clipRect.width, clipRect.height, BORDER, Color.gray);

            clipRect.width -= BORDER;
            clipRect.y += BORDER;
            clipRect.height -= BORDER * 2;
            clipRect.x += BORDER;

            GLDraw.Line(BORDER, 100, clipRect.width, 100, new Color(0.7f, 0.7f, 0.7f));

            const int THRESHOLD = 100;

            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150), new Vector2(THRESHOLD, 150),
                new Vector2(THRESHOLD, 50), new Vector2(clipRect.width, 50));
            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150 + 1), new Vector2(THRESHOLD + 1, 150 + 1),
                new Vector2(THRESHOLD + 1, 50 + 1), new Vector2(clipRect.width, 50 + 1));


            Vector3SecondOrderCalculator.Vector3Data targetPosition = new(new(0, 1.5f, 0));
            Vector3SecondOrderCalculator.Vector3Data prevTargetPosition = new(new(0, 1.5f, 0));
            Vector3SecondOrderCalculator.Vector3Data currPosition = new(new(0, 1.5f, 0));
            Vector3SecondOrderCalculator.Vector3Data currVelocity = new(new(0, 0, 0));

            Vector2[] points = new Vector2[(int)clipRect.width];
            for (int i = 0; i < (int)clipRect.width; i++)
            {
                if (i > THRESHOLD)
                {
                    targetPosition = new(new(0, 0.5f, 0));
                }

                object callResult = manualUpdate
                    .Invoke(null, new object[]
                    {
                        0.01f, targetPosition, prevTargetPosition,
                        currPosition, currVelocity, consts
                    });

                (SecondOrderCalculator<Vector3>.IData<Vector3> position,
                    SecondOrderCalculator<Vector3>.IData<Vector3> velocity) resultTuple =
                        ((SecondOrderCalculator<Vector3>.IData<Vector3>,
                            SecondOrderCalculator<Vector3>.IData<Vector3>))
                        callResult;

                currPosition = (Vector3SecondOrderCalculator.Vector3Data)resultTuple.position;
                currVelocity = (Vector3SecondOrderCalculator.Vector3Data)resultTuple.velocity;

                prevTargetPosition = targetPosition;
                float currentPositionY = currPosition.GetData().y;
                points[i] = new Vector2(BORDER + i, currentPositionY * 100);
            }

            Rect rect = clipRect;
            rect.x = BORDER;
            rect.y = BORDER;
            rect.xMax = clipRect.width;
            rect.yMax = GRAPH_SIZE - BORDER;
            GLDraw.Lines(Color.green, rect, points);

            GL.PopMatrix();
            GUI.EndClip();
        }
    }
}