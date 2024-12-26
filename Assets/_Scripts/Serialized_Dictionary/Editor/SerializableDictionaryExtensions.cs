#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace LTF.SerializedDictionary.Editor
{
    public static class SerializableDictionaryExtensions
    {
        public const BindingFlags PUBLIC_OR_NON_INSTANCE= BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static object GetValue(this object source, string name)
        {
            if (source == null)
                return null;

            Type type = source.GetType();
            while (type != null)
            {
                FieldInfo fieldInfo = type.GetField(name, PUBLIC_OR_NON_INSTANCE);
                if (fieldInfo != null)
                    return fieldInfo.GetValue(source);

                PropertyInfo prop = type.GetProperty(name, PUBLIC_OR_NON_INSTANCE | BindingFlags.IgnoreCase);
                if (prop != null)
                    return prop.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        public static object GetValue(this object source, string name, int index)
        {
            if (GetValue(source, name) is not IEnumerable enumerable)
                return null;

            IEnumerator iEnumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
                if (!iEnumerator.MoveNext())
                    return null;

            return iEnumerator.Current;
        }

        public static object GetTargetObject(this SerializedProperty prop)
        {
            object targetObject = prop.serializedObject.targetObject;
            string[] elements = prop.propertyPath.Replace(".Array.data[", "[").Split('.');
            int length = elements.Length;
            for (int i = 0; i < length; i++)
            {
                if (elements[i].Contains("["))
                {
                    string elementName = elements[i][..elements[i].IndexOf("[")];
                    int index = Convert.ToInt32(elements[i][elements[i].IndexOf("[")..].Replace("[", "").Replace("]", ""));
                    targetObject = GetValue(targetObject, elementName, index);
                }
                else
                    targetObject = GetValue(targetObject, elements[i]);
            }

            return targetObject;
        }

        public static object GetValue(this SerializedProperty prop) => prop.propertyType switch
        {
            SerializedPropertyType.Integer => prop.intValue,
            SerializedPropertyType.Boolean => prop.boolValue,
            SerializedPropertyType.Float => prop.floatValue,
            SerializedPropertyType.String => prop.stringValue,
            SerializedPropertyType.Color => prop.colorValue,
            SerializedPropertyType.ObjectReference => prop.objectReferenceValue,
            SerializedPropertyType.Enum => prop.enumValueIndex,
            SerializedPropertyType.Vector2 => prop.vector2Value,
            SerializedPropertyType.Vector3 => prop.vector3Value,
            SerializedPropertyType.Vector4 => prop.vector4Value,
            SerializedPropertyType.Rect => prop.rectIntValue,
            SerializedPropertyType.ArraySize => prop.arraySize,
            SerializedPropertyType.AnimationCurve => prop.animationCurveValue,
            SerializedPropertyType.Bounds => prop.boundsValue,
            SerializedPropertyType.Quaternion => prop.quaternionValue,
            SerializedPropertyType.ExposedReference => prop.exposedReferenceValue,
            SerializedPropertyType.FixedBufferSize => prop.fixedBufferSize,
            SerializedPropertyType.Vector2Int => prop.vector2IntValue,
            SerializedPropertyType.Vector3Int => prop.vector3IntValue,
            SerializedPropertyType.RectInt => prop.rectIntValue,
            SerializedPropertyType.BoundsInt => prop.boundsIntValue,
            // Missing cases but relative easy to add
            _ => prop.type switch
            {
                "double" => prop.doubleValue,
                "long" => prop.longValue,
                _ => prop.GetTargetObject(),
            },
        };

        public static bool HasAnyElementSameValue(this SerializedProperty array, SerializedProperty key1, int skipIndex)
        {
            int length = array.arraySize;
            for (int i = 0; i < length; i++)
            {
                if (i == skipIndex)
                    continue;

                SerializedProperty key2 = array.GetArrayElementAtIndex(i);
                object key1Value = key1?.GetValue();
                object key2Value = key2?.GetValue();
                if (key1Value == null ? key2Value == null : key1Value.Equals(key2Value))
                    return true;
            }

            return false;
        }
    }
}
#endif
