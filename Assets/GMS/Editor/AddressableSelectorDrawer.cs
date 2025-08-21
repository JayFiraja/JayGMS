using UnityEngine;
using UnityEditor;

namespace GMS
{
    /// <summary>
    /// Property Attribute for being able to Draw a nice selector instead of just a plain string field for accurately select Addressables and serialize their keys
    /// </summary>
    [CustomPropertyDrawer(typeof(AddressableSelectorAttribute))]
    public class AddressableSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect = new Rect(position.x, position.y, position.width - 60, position.height);
            Rect buttonRect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
            property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);

            if (GUI.Button(buttonRect, "Select"))
            {
                ShowAddressableSelectorPopup(buttonRect, property);
            }

            EditorGUI.EndProperty();
        }

        private void ShowAddressableSelectorPopup(Rect buttonRect, SerializedProperty property)
        {
            PopupWindow.Show(buttonRect, new AddressableSelectorPopup(property));
        }
    }
}