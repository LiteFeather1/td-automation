#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectPool<>))]
public class WeightedObjectDrawer : PropertyDrawer
{
    private const float PAD = 32f;
    private const string TOOLTIP = "Second Parameter is the Initial pool size";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        using SerializedProperty p = property.FindPropertyRelative("_object");
        return EditorGUI.GetPropertyHeight(p, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using SerializedProperty objectP = property.FindPropertyRelative("_object");
        using SerializedProperty initialSizeP = property.FindPropertyRelative("_initialPoolSize");
        label.tooltip = TOOLTIP;

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            position.width -= PAD;
            EditorGUI.PropertyField(position, objectP, label, true);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position.x += PAD + position.width;
            position.width = PAD - 4;
            position.height = EditorGUI.GetPropertyHeight(initialSizeP);
            position.x -= position.width;

            EditorGUI.PropertyField(position, initialSizeP, GUIContent.none);

            EditorGUI.indentLevel = indent;
        }
    }
}
#endif
