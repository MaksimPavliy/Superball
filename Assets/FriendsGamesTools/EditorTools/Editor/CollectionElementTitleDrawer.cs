using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    [CustomPropertyDrawer(typeof(CollectionElementTitleAttribute))]
    public class CollectionElementTitleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
        string fieldName => ((CollectionElementTitleAttribute)attribute).elementFieldName;
        public override void OnGUI(Rect position,
                                  SerializedProperty property,
                                  GUIContent label)
        {
            string shownText;
            if (!fieldName.Contains("{"))
                shownText = GetPropertyValueString(property, fieldName);
            else
            {
                shownText = fieldName;
                while (shownText.Contains("{"))
                {
                    var startInd = shownText.IndexOf('{');
                    var endInd = shownText.IndexOf('}');
                    var field = shownText.Substring(startInd + 1, endInd - startInd - 1);
                    var fieldValue = GetPropertyValueString(property, field);
                    shownText = shownText.Replace($"{{{field}}}", fieldValue);
                }
            }
            if (string.IsNullOrEmpty(shownText))
                shownText = label.text;
            EditorGUI.PropertyField(position, property, new GUIContent(shownText, label.tooltip), true);
        }
        private string GetPropertyValueString(SerializedProperty property, string fieldName)
        {
            var fullPathName = property.propertyPath + "." + fieldName;
            var itemNameProperty = property.serializedObject.FindProperty(fullPathName);
            return GetPropertyValueString(itemNameProperty);
        }
        private string GetPropertyValueString(SerializedProperty itemNameProperty)
        {
            switch (itemNameProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return itemNameProperty.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return itemNameProperty.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return itemNameProperty.floatValue.ToString();
                case SerializedPropertyType.String:
                    return itemNameProperty.stringValue;
                case SerializedPropertyType.Color:
                    return itemNameProperty.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    return itemNameProperty.objectReferenceValue.ToString();
                case SerializedPropertyType.Enum:
                    return itemNameProperty.enumNames[itemNameProperty.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return itemNameProperty.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return itemNameProperty.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return itemNameProperty.vector4Value.ToString();
                case SerializedPropertyType.Character:
                    return ((char)itemNameProperty.intValue).ToString();
                default:
                    return $"{itemNameProperty.propertyType} showing not implemented, cant show field {fieldName}";
            }
        }
    }
}