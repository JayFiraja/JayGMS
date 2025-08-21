using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Popup for displaying an Addressable Selector
    /// </summary>
    public class AddressableSelectorPopup : PopupWindowContent
    {
        private SerializedProperty property;
        private List<string> allAddressableKeys = new List<string>();
        private List<string> filteredKeys = new List<string>();
        private string searchQuery = "";
        private Vector2 scrollPosition;
        private Vector2 window_Size = new Vector2(400, 300);

        public AddressableSelectorPopup(SerializedProperty property)
        {
            this.property = property;
            LoadAddressableKeys();
            FilterKeys("");
        }

        /// <summary>
        /// Overwrite this if a child class wants a filtered version of the Addressables, like by asset type or group.
        /// </summary>
        protected virtual void LoadAddressableKeys()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                return;
            }

            allAddressableKeys.Clear();

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    // here we add all the found Addressables
                    allAddressableKeys.Add(entry.address);
                }
            }
        }

        private void FilterKeys(string query)
        {
            searchQuery = query.ToLower();
            filteredKeys = allAddressableKeys.FindAll(key => key.ToLower().Contains(searchQuery));
        }

        public override Vector2 GetWindowSize()
        {
            return window_Size;
        }

        public override void OnGUI(Rect rect)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Space(10);

                // Create a horizontal layout for the search icon and text field
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Draw the magnifying glass icon
                    GUIContent magnifyingGlassIcon = EditorGUIUtility.IconContent("Search Icon");
                    GUILayout.Label(magnifyingGlassIcon, GUILayout.Width(20), GUILayout.Height(20));

                    // Draw the search text field without a label
                    string newSearchQuery = EditorGUILayout.TextField(searchQuery, GUILayout.ExpandWidth(true));
                    if (newSearchQuery != searchQuery)
                    {
                        searchQuery = newSearchQuery;
                        FilterKeys(searchQuery);
                    }
                }

                GUILayout.Space(10);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                foreach (string key in filteredKeys)
                {
                    if (GUILayout.Button(key))
                    {
                        property.stringValue = key;
                        property.serializedObject.ApplyModifiedProperties();
                        editorWindow.Close();
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }
    }
}