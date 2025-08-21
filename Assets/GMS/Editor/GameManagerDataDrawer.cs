using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace GMS
{
    [CustomPropertyDrawer(typeof(SubManagerData))]
    public class GameManagerDataDrawer : PropertyDrawer
    {
        private List<Type> dataTypes;
        private string[] displayNames;
        private string[] dataTypeNames;

        private Color _Green = new Color(0.1f, 0.5f, 0.1f);
        private Color _DarkGray = new Color(0.15f, 0.15f, 0.15f);

        public GameManagerDataDrawer()
        {
            dataTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(LinkDataLogicAttribute), false).Length > 0)
                .ToList();

            displayNames = dataTypes.Select(type =>
            {
                var attr = type.GetCustomAttribute<LinkDataLogicAttribute>();
                return attr.DisplayName;
            }).ToArray();

            dataTypeNames = dataTypes.Select(type => type.AssemblyQualifiedName).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            Color originalColor = GUI.color;

            // Calculate rects for each field
            float labelWidth = 50f;
            float boolWidth = 20f;
            float padding = 5f;
            float typeWidth = position.width - labelWidth * 2 - boolWidth * 2 - padding * 4;

            Rect typeRect = new Rect(
                position.x, 
                position.y, 
                typeWidth, 
                position.height);
            Rect loadLabelRect = new Rect(
                position.x + typeWidth + padding,
                position.y,
                labelWidth,
                position.height);
            Rect loadRect = new Rect(
                position.x + typeWidth + labelWidth + padding * 2, 
                position.y, 
                boolWidth,
                position.height);
            Rect baseLabelRect = new Rect(
                position.x + typeWidth + labelWidth + boolWidth + padding * 3, 
                position.y,
                labelWidth,
                position.height);
            Rect baseRect = new Rect(
                position.x + typeWidth + labelWidth * 2 + boolWidth + padding * 4, 
                position.y, 
                boolWidth, 
                position.height);

            // Draw the SubManagerType dropdown
            SerializedProperty dataTypeNameProperty = property.FindPropertyRelative("dataTypeName");
            if (dataTypeNameProperty != null)
            {
                int index = Mathf.Max(0, Array.IndexOf(dataTypeNames, dataTypeNameProperty.stringValue));
                index = EditorGUI.Popup(typeRect, index, displayNames);
                if (index >= 0)
                {
                    dataTypeNameProperty.stringValue = dataTypeNames[index];
                }
            }
            else
            {
                Debug.LogError($"dataTypeName not found in {property.propertyPath}");
                EditorGUI.LabelField(typeRect, "dataTypeName not found");
            }

            // Draw the Loads boolean
            SerializedProperty loadsProperty = property.FindPropertyRelative("Loads");
            if (loadsProperty != null)
            {
                GUIContent loadsContent = new GUIContent("", "If true, this will be loaded by the game manager.");
                GUI.tooltip = loadsContent.tooltip;

                EditorGUI.LabelField(loadLabelRect, "Load");
                GUI.color = loadsProperty.boolValue ? _Green : _DarkGray;
                loadsProperty.boolValue = EditorGUI.Toggle(loadRect, loadsContent, loadsProperty.boolValue);
                GUI.color = originalColor; // Reset color
            }
            else
            {
                EditorGUI.LabelField(loadLabelRect, "Load");
                EditorGUI.LabelField(loadRect, "Loads not found");
            }

            // Draw the IsBase boolean
            SerializedProperty isBaseProperty = property.FindPropertyRelative("IsBase");
            if (isBaseProperty != null)
            {
                GUIContent isBaseContent = new GUIContent("", "If true, this will be loaded as a base SubManager.");
                GUI.tooltip = isBaseContent.tooltip;

                EditorGUI.LabelField(baseLabelRect, "Is Base");
                GUI.color = isBaseProperty.boolValue ? _Green : _DarkGray;
                isBaseProperty.boolValue = EditorGUI.Toggle(baseRect, isBaseContent, isBaseProperty.boolValue);
                GUI.color = originalColor; // Reset color
            }
            else
            {
                EditorGUI.LabelField(baseLabelRect, "Is Base");
                EditorGUI.LabelField(baseRect, "IsBase not found");
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
