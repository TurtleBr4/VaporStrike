
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveDefinition))]
public class MoveDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Move
        EditorGUILayout.LabelField("Move", EditorStyles.boldLabel);
        Draw("moveName");

        EditorGUILayout.Space();

        // Input
        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        Draw("button");
        Draw("direction");

        EditorGUILayout.Space();

        // Input Forgiveness
        EditorGUILayout.LabelField("Input Forgiveness", EditorStyles.boldLabel);
        Draw("directionWindow");
        Draw("buttonWindow");
        Draw("bufferDuration");

        EditorGUILayout.Space();

        // Animation
        EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
        Draw("animationID");

        EditorGUILayout.Space();

        // Data
        EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
        Draw("isAirMove");
        Draw("frameTime");
        Draw("hitStun");
        Draw("cancelTime");

        EditorGUILayout.Space();

        // Displacement
        EditorGUILayout.LabelField("Displacement", EditorStyles.boldLabel);

        Draw("hasDisplacement");

        if (Find("hasDisplacement").boolValue)
        {
            EditorGUI.indentLevel++;

            Draw("displacementDirection");
            Draw("displacementMagnitude");

            EditorGUILayout.Space(3);

            EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);

            Draw("displacementStartFrame");
            Draw("displacementEndFrame");

            EditorGUI.indentLevel--;
        }

        // Hitboxes
        EditorGUILayout.LabelField("Hitboxes", EditorStyles.boldLabel);
        Draw("hitboxIDs");
        Draw("hitboxStartFrames");
        Draw("hitboxEndFrames");

        serializedObject.ApplyModifiedProperties();
    }

    private void Draw(string propertyName)
    {
        SerializedProperty property = Find(propertyName);

        if (property != null)
        {
            EditorGUILayout.PropertyField(property, true);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Missing property: " + propertyName,
                MessageType.Warning
            );
        }
    }

    private SerializedProperty Find(string propertyName)
    {
        return serializedObject.FindProperty(propertyName);
    }
}
