#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Portal.Stage.EnemySpawn))]
public class EnemySpawnDrawer : PropertyDrawer
{
    private const float TIME_PERCENT = .1f;
    private const string TOOLTIP = "Second Parameter is the spawn time";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        using SerializedProperty p = property.FindPropertyRelative("_spawnTime");
        return EditorGUI.GetPropertyHeight(p, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using SerializedProperty enemyPoolP = property.FindPropertyRelative("_enemyPool");
        using SerializedProperty spawnTimeP = property.FindPropertyRelative("_spawnTime");
        label.tooltip = TOOLTIP;

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            float pad = position.width * TIME_PERCENT;
            position.width -= pad;
            EditorGUI.PropertyField(position, enemyPoolP, label, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position.x += pad + position.width;
            position.width = pad;
            position.height = EditorGUI.GetPropertyHeight(enemyPoolP);
            position.x -= position.width;

            EditorGUI.PropertyField(position, spawnTimeP, GUIContent.none);

            EditorGUI.indentLevel = indent;
        }
    }
}
#endif