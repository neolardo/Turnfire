#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RangedStatFloat))]
public class RangedStatFloatDrawer : PropertyDrawer
{
    private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
    private const float Spacing = 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout header and nested properties as usual
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, LineHeight),
            property.isExpanded,
            $"{label.text}"
        );

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            float y = position.y + LineHeight + Spacing;
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();

            // Iterate over all nested fields inside RangedStat
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                float height = EditorGUI.GetPropertyHeight(iterator, true);
                Rect rect = new Rect(position.x, y, position.width, height);

                // Decorate `_normalizedValue` field
                if (iterator.name == "_normalizedValue" || iterator.name == "normalizedValue")
                {
                    SerializedProperty valueProp = property.FindPropertyRelative("_normalizedValue");
                    SerializedProperty groupProp = property.FindPropertyRelative("_group");
                    var group = groupProp.objectReferenceValue as RangedStatFloatGroupDefinition;


                    if (valueProp == null || groupProp == null)
                    {
                        EditorGUI.LabelField(position, label.text, $"Missing fields in {nameof(RangedStat)} class");
                        EditorGUI.EndProperty();
                        return;
                    }
                    float actualValue = Mathf.Lerp(group.Minimum, group.Maximum, valueProp.floatValue);


                    GUIContent decoratedLabel = new GUIContent($"Value (actual = {actualValue:F2})");
                    EditorGUI.PropertyField(rect, iterator, decoratedLabel);
                }
                else if (iterator.name == "_randomness") //only draw if randomization is enabled
                {
                    SerializedProperty randomizedProp = property.FindPropertyRelative("_isRandomized");
                    if (randomizedProp.boolValue)
                    {
                        EditorGUI.PropertyField(rect, iterator, true);
                    }
                    else
                    {
                        height = 0;
                    }
                }
                else
                {
                    EditorGUI.PropertyField(rect, iterator, true);
                }

                y += height + Spacing;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return LineHeight;

        float total = LineHeight + Spacing;
        SerializedProperty iterator = property.Copy();
        SerializedProperty endProperty = iterator.GetEndProperty();

        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            enterChildren = false;
            total += EditorGUI.GetPropertyHeight(iterator, true) + Spacing;
        }

        return total;
    }
}
#endif