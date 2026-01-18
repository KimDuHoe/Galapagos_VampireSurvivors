using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector, which shows the public fields of Spawner
        DrawDefaultInspector();

        // Get a reference to the Spawner component being inspected
        Spawner spawner = (Spawner)target;

        // Don't proceed if the spawn data array is not set up
        if (spawner.spawnData == null || spawner.spawnData.Length == 0)
        {
            return;
        }

        // --- Calculate Total Weight ---
        float totalWeight = 0;
        foreach (var data in spawner.spawnData)
        {
            // Ensure weight is not negative
            if (data.weight > 0)
            {
                totalWeight += data.weight;
            }
        }

        // Add a visual separator
        EditorGUILayout.Space(10);

        // --- Display Percentages ---
        EditorGUILayout.LabelField("Spawn Probabilities", EditorStyles.boldLabel);

        if (totalWeight > 0)
        {
            // Begin a vertical group for a nice box layout
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var data in spawner.spawnData)
            {
                float percentage = 0;
                if (data.weight > 0)
                {
                    percentage = (data.weight / totalWeight) * 100;
                }
                // Display the pool index and its calculated percentage
                EditorGUILayout.LabelField($"  Pool Index {data.poolIndex}", $"{percentage:F2}%");
            }

            EditorGUILayout.EndVertical();
        }
        else
        {
            // Show a message if all weights are zero
            EditorGUILayout.HelpBox("All weights are zero. No percentages to calculate.", MessageType.Warning);
        }
        
        // Add an informational note at the end
        EditorGUILayout.HelpBox("Percentages are calculated in real-time based on the weights above.", MessageType.Info);
        
        // Apply any changes made in the Inspector
        serializedObject.ApplyModifiedProperties();
    }
}
