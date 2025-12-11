using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static object GetSerializedValue(this SerializedProperty property)
    {
        if (property == null) return null;

        object obj = property.serializedObject.targetObject;
        string[] path = property.propertyPath.Split('.');

        foreach (string p in path)
        {
            var type = obj.GetType();
            var field = type.GetField(p, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (field == null)
                return null;

            obj = field.GetValue(obj);
        }

        return obj;
    }
}