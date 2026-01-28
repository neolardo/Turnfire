
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangedStatInt))]
[CustomPropertyDrawer(typeof(RangedStatFloat))]
public class RangedStatDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Cache values
        var normalizedValueProp = property.FindPropertyRelative("_normalizedValue");
        var isRandomizedProp = property.FindPropertyRelative("_isRandomized");
        var randomnessProp = property.FindPropertyRelative("_randomness");
        var groupProp = property.FindPropertyRelative("_group");

        // Compute value
        string averageString = GetAverageValueString(property);

        // Append "(value)" to label
        var newLabel = new GUIContent($"{label.text} ({averageString})", label.tooltip);

        // Draw foldout
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            newLabel
        );

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        float y = position.y + EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            normalizedValueProp);
        y += EditorGUIUtility.singleLineHeight + 2;

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            isRandomizedProp);
        y += EditorGUIUtility.singleLineHeight + 2;

        if (isRandomizedProp.boolValue)
        {
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                randomnessProp);
            y += EditorGUIUtility.singleLineHeight + 2;
        }

        EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
            groupProp);

        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight; // foldout

        if (!property.isExpanded)
            return h;

        h += EditorGUIUtility.singleLineHeight + 2; // normalized
        h += EditorGUIUtility.singleLineHeight + 2; // isRandomized

        if (property.FindPropertyRelative("_isRandomized").boolValue)
            h += EditorGUIUtility.singleLineHeight + 2; // randomness

        h += EditorGUIUtility.singleLineHeight + 2; // group

        return h;
    }

    private string GetAverageValueString(SerializedProperty property)
    {
        object target = property.GetSerializedValue();
        if (target == null)
            return "?";

        if (target is RangedStat<int> intStat && intStat.Group != null)
        {
            int min = intStat.MinimumValue;
            int max = intStat.MaximumValue;
            return min == max ? min.ToString() : $"{min}-{max}";
        }

        if (target is RangedStat<float> floatStat && floatStat.Group != null)
        {
            float min = floatStat.MinimumValue;
            float max = floatStat.MaximumValue;
            return Mathf.Approximately(min, max) ? min.ToString() : $"{min}-{max}";
        }

        return "?";
    }
}
#endif