using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GMS.Editor
{
    /// <summary>
    /// Editor drawer class that will take care of the usability of the <see cref="GameManagerData"/> scriptable objects.
    /// Edit at your own risk, as this tool needs several testing in order to ensure it's stability.
    /// </summary>
    [CustomEditor(typeof(GameManagerData))]
    public class GameManagerDataEditor : UnityEditor.Editor
    {
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private int subManagerDataListHash;

        private Color _headerBoxColour = new Color(0.302f, 0.529f, 0.51f, 0.69f);

        // Button variables for opening the Create new Sub Manager window tool
        private Color _normalColour = new Color(0.953f, 0.482f, 0.435f);
        private Color _hoverColour = new Color(0.988f, 0.627f, 0.596f);
        private Color _activeColour = new Color(0.678f, 0.278f, 0.239f);
        private float _windowButtonWidth = 400;
        private GUIContent buttonContent;
        private const string BUTTON_TEXT = "Open Create Sub Manager Window";
        private const string BUTTON_TOOLTIP = "Click to open the Create Sub Manager Window";

        // button variables for the collapse data fields
        private Color _normalColourCollapseFields = new Color(0.302f, 0.529f, 0.51f, 0.69f);
        private Color _hoverColourCollapseFields = new Color(0.402f, 0.629f, 0.61f, 0.8f);
        private Color _activeColourCollapseFields = new Color(0.202f, 0.429f, 0.51f, 0.59f);
        private float _windowButtonWidthCollapseFields = 400;
        private GUIContent buttonContentCollapseFields;
        private const string BUTTON_TEXT_COLLAPSE_FIELDS = "Collapse All Data Fields";
        private const string BUTTON_TOOLTIP_COLLAPSE_FIELDS = "Click to collapse all the data entries";

        private Texture2D _gmsIcon;
        private Color _originalColor;
        private Vector2 scrollPosition;
        private GUIStyle leftAlignedButtonStyle;
        private GUIStyle middleAlignedButtonStyle;
        private GUIStyle middleAlignedCollapseButtonStyle;
        private GUIStyle subManagerContainerStyle;
        private GUIStyle subManagerFieldsStyle;

        private const string HEADER_DATA_STRUCTS = "Data structs for selected SubManagers";

        private void OnEnable()
        {
            _originalColor = GUI.color;
            _gmsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GMS/Editor/Sprites/gmsIcon.png");
        }

        public override void OnInspectorGUI()
        {
            InitializeGUIStyles();

            serializedObject.Update();
            GameManagerData gameManagerData = (GameManagerData)target;

            // Begin Scroll View
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUIDrawButtonCreateSubManagerWindow();

            GUIDrawSubManagerList(gameManagerData);

            GUIDrawPanelHeader();

            GUILayout.Space(5);

            GUIDrawCollapseDataFieldsButton();

            DrawSubManagerDatas(gameManagerData);

            // End Scroll View
            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();

            // Force the inspector to repaint if there are changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
        private void InitializeGUIStyles()
        {
            // Initialize the left-aligned button style
            if (EditorStyles.helpBox != null)
            {
                leftAlignedButtonStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 14,
                    fixedHeight = 16,
                };
            }

            // Initialize the button content with a tooltip
            buttonContent = new GUIContent(BUTTON_TEXT, BUTTON_TOOLTIP);
            Texture2D normalTexture = DrawerEditor.MakeTexture(width: 1, height: 1, _normalColour);
            Texture2D hoverTexture = DrawerEditor.MakeTexture(1, 1, _hoverColour);
            Texture2D activeTexture = DrawerEditor.MakeTexture(1, 1, _activeColour);
            // Initialize the middle-aligned button style - used as a utility button for opening the create submanager window
            middleAlignedButtonStyle = new GUIStyle(EditorStyles.toolbarPopup)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fixedHeight = 30,
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(10, 10, 5, 5),
                margin = new RectOffset(5, 5, 5, 5)
            };
            middleAlignedButtonStyle.normal.background = normalTexture;
            middleAlignedButtonStyle.normal.textColor = Color.white;
            middleAlignedButtonStyle.hover.background = hoverTexture;
            middleAlignedButtonStyle.hover.textColor = Color.white;
            middleAlignedButtonStyle.active.background = activeTexture;
            middleAlignedButtonStyle.active.textColor = Color.black;


            buttonContentCollapseFields = new GUIContent(BUTTON_TEXT_COLLAPSE_FIELDS, BUTTON_TOOLTIP_COLLAPSE_FIELDS);
            Texture2D normalTextureCollapseFields = DrawerEditor.MakeTexture(width: 1, height: 1, _normalColourCollapseFields);
            Texture2D hoverTextureCollapseFields = DrawerEditor.MakeTexture(1, 1, _hoverColourCollapseFields);
            Texture2D activeTextureCollapseFields = DrawerEditor.MakeTexture(1, 1, _activeColourCollapseFields);

            // Initialize middle Aligned Collapse Button Style - for the collapse data fields button.
            middleAlignedCollapseButtonStyle = new GUIStyle(EditorStyles.toolbarPopup)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fixedHeight = 20,
                border = new RectOffset(4, 4, 4, 4),
                padding = new RectOffset(10, 10, 1, 1),
                margin = new RectOffset(5, 5, 5, 5)
            };
            middleAlignedCollapseButtonStyle.normal.background = normalTextureCollapseFields;
            middleAlignedCollapseButtonStyle.normal.textColor = Color.white;
            middleAlignedCollapseButtonStyle.hover.background = hoverTextureCollapseFields;
            middleAlignedCollapseButtonStyle.hover.textColor = Color.white;
            middleAlignedCollapseButtonStyle.active.background = activeTextureCollapseFields;
            middleAlignedCollapseButtonStyle.active.textColor = Color.black;
            

            // Initialize the sub-manager container style
            subManagerContainerStyle = new GUIStyle(EditorStyles.toolbarPopup)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            subManagerFieldsStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };
        }


        private void GUIDrawSubManagerList(GameManagerData gameManagerData)
        {
            // Draw default subManagerDataList
            EditorGUILayout.PropertyField(serializedObject.FindProperty("subManagerDataList"), includeChildren: true);

            // Check for changes in the subManagerDataList
            int newSubManagerDataListHash = GetListHash(gameManagerData.subManagerDataList);
            if (subManagerDataListHash != newSubManagerDataListHash)
            {
               // UpdateSubManagersList(gameManagerData);
                subManagerDataListHash = newSubManagerDataListHash;
                gameManagerData.CleanupSubManagers();

                EditorApplication.delayCall += () => UpdateSubManagersList(gameManagerData);
            }
        }

        private void DrawSubManagerDatas(GameManagerData gameManagerData)
        {
            SerializedProperty subManagersProperty = serializedObject.FindProperty("subManagers");
            for (int i = 0; i < subManagersProperty.arraySize; i++)
            {
                SerializedProperty subManagerProperty = subManagersProperty.GetArrayElementAtIndex(i);
                if (subManagerProperty == null)
                {
                    continue;
                }

                ISubManagerData subManager = gameManagerData.subManagers[i];
                if (subManager == null)
                {
                    continue;
                }

                // Check if the logic class is found in the current gameManagerData.subManagerDataList
                if (!IsDataStructFound(gameManagerData, subManager.GetType()))
                {
                    continue;
                }

                string displayName = GetSubManagerDisplayName(subManager);
                string key = $"{subManager.GetType().FullName}_{i}";
                if (!foldoutStates.ContainsKey(key))
                {
                    foldoutStates[key] = true;
                }

                if (GUILayout.Button(displayName, subManagerContainerStyle))
                {
                    foldoutStates[key] = !foldoutStates[key];
                }

                if (foldoutStates[key])
                {
                    EditorGUI.indentLevel++;
                    DrawSubManagerFields(subManagerProperty, subManager, gameManagerData, key);
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void GUIDrawButtonCreateSubManagerWindow()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Add flexible space to the left

            if (GUILayout.Button(buttonContent, middleAlignedButtonStyle, GUILayout.Width(_windowButtonWidth)))
            {
                GameManagementWindow.ShowWindow();
            }

            GUILayout.FlexibleSpace(); // Add flexible space to the right
            GUILayout.EndHorizontal();
        }

        private void GUIDrawPanelHeader()
        {
            // Draw the header with the background color
            DrawerEditor.GetGUIBoxStyle(_headerBoxColour, out GUIStyle colouredBox);
            GUILayout.BeginVertical(colouredBox);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Add flexible space to the left

            // Load icon
            GUIContent iconContent = EditorGUIUtility.IconContent("console.infoicon.sml@2x");
            GUIStyle infoLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white },
                fontStyle = FontStyle.Bold
            };
            GUILayout.Label(iconContent, GUILayout.Width(20), GUILayout.Height(20));
            GUILayout.Label(HEADER_DATA_STRUCTS, infoLabelStyle, GUILayout.Height(20));

            GUILayout.FlexibleSpace(); // Add flexible space to the right
            GUILayout.EndHorizontal();

            GUILayout.EndVertical(); // end coloured box
        }

        private void UpdateSubManagersList(GameManagerData gameManagerData)
        {
            Dictionary<string, ISubManagerData> existingSubManagers = new Dictionary<string, ISubManagerData>();

            for (int i = gameManagerData.subManagers.Count - 1; i >= 0; i--)
            {
                if (gameManagerData.subManagers[i] == null)
                {
                    gameManagerData.subManagers.RemoveAt(i);
                }
            }

            // Collect existing sub-managers in a dictionary by their type names
            foreach (ISubManagerData subManager in gameManagerData.subManagers)
            {
                string typeName = subManager.GetType().FullName;
                if (!existingSubManagers.ContainsKey(typeName))
                {
                    existingSubManagers.Add(typeName, subManager);
                }
            }

            // Add new subManagers based on the subManagerDataList
            foreach (SubManagerData subManagerData in gameManagerData.subManagerDataList)
            {
                Type subManagerType = Type.GetType(subManagerData.dataTypeName);
                if (subManagerType != null && typeof(ISubManagerData).IsAssignableFrom(subManagerType))
                {
                    if (!existingSubManagers.TryGetValue(subManagerType.FullName, out ISubManagerData subManagerInstance))
                    {
                        subManagerInstance = (ISubManagerData)Activator.CreateInstance(subManagerType);
                    }
                    if (!gameManagerData.subManagers.Contains(subManagerInstance))
                    {
                        gameManagerData.subManagers.Add(subManagerInstance);
                    }
                }
                else
                {
                    Debug.LogWarning($"Could not find type {subManagerData.dataTypeName}");
                }
            }

            CleanupUnlinkedSubManagers(gameManagerData);
        }

        private void DrawSubManagerFields(SerializedProperty subManagerProperty, ISubManagerData subManager, GameManagerData gameManagerData, string key)
        {
            if (subManager == null)
                return;

            EditorGUILayout.BeginVertical(subManagerFieldsStyle);

            Type subManagerType = subManager.GetType();
            FieldInfo[] fields = subManagerType.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (fields.Length == 0)
            {
                EditorGUILayout.LabelField("No Fields");
            }
            else
            {
                foreach (FieldInfo field in fields)
                {
                    if (field.GetCustomAttribute<HideInInspector>() != null)
                    {
                        continue;
                    }
                    SerializedProperty property = subManagerProperty.FindPropertyRelative(field.Name);
                    if (property != null)
                    {
                        EditorGUILayout.PropertyField(property, true);
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"Field {field.Name} not found");
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        private bool IsDataStructFound(GameManagerData gameManagerData, Type dataType)
        {
            if (!TryGetLinkDataLogicAttribute(dataType, out LinkDataLogicAttribute linkDataLogicAttribute))
            {
                return false;
            }

            foreach (SubManagerData subManagerData in gameManagerData.subManagerDataList)
            {
                if (subManagerData.dataTypeName == linkDataLogicAttribute.DataClassType.AssemblyQualifiedName)
                {
                    return true;
                }
            }
            return false;
        }

        private bool TryGetLinkDataLogicAttribute(Type dataType, out LinkDataLogicAttribute linkDataLogicAttribute)
        {
            linkDataLogicAttribute = dataType.GetCustomAttribute<LinkDataLogicAttribute>();
            return linkDataLogicAttribute != null;
        }

        private string GetSubManagerDisplayName(ISubManagerData subManager)
        {
            if (subManager == null)
                return "SubManager";

            Type type = subManager.GetType();
            LinkDataLogicAttribute attribute = type.GetCustomAttribute<LinkDataLogicAttribute>();
            return attribute != null ? attribute.DataDisplayName : ObjectNames.NicifyVariableName(type.Name);
        }

        private int GetListHash(List<SubManagerData> list)
        {
            int hash = 17;
            foreach (SubManagerData item in list)
            {
                hash = hash * 31 + (item.dataTypeName != null ? item.dataTypeName.GetHashCode() : 0);
            }
            return hash;
        }

        private void GUIDrawCollapseDataFieldsButton()
        {
            if (foldoutStates.Count == 0)
            {
                return;
            }
            List<string> keys = new List<string>(foldoutStates.Keys);

            bool atLeastOneIsExapended = false;
            // check if at least one is expanded
            foreach (string key in keys)
            {
                atLeastOneIsExapended |= foldoutStates[key];
            }
            if (!atLeastOneIsExapended)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Add flexible space to the left

            if (GUILayout.Button(BUTTON_TEXT_COLLAPSE_FIELDS, middleAlignedCollapseButtonStyle, GUILayout.Width(_windowButtonWidthCollapseFields)))
            {
                CollapseAllFields();
            }

            GUILayout.FlexibleSpace(); // Add flexible space to the right
            GUILayout.EndHorizontal();
        }

        private void CollapseAllFields()
        {
            // Create a list of keys to iterate over
            List<string> keys = new List<string>(foldoutStates.Keys);

            // Iterate over the list of keys and set each foldout state to false
            foreach (string key in keys)
            {
                foldoutStates[key] = false;
            }
        }

        private void CleanupUnlinkedSubManagers(GameManagerData gameManagerData)
        {
            // Get the serialized property for the subManagers list
            SerializedProperty subManagersProperty = serializedObject.FindProperty("subManagers");

            // Create a HashSet to track existing sub-manager data types
            HashSet<Type> existingSubManagerTypes = new HashSet<Type>();

            // Build a set of existing sub-manager types
            foreach (var subManager in gameManagerData.subManagers)
            {
                existingSubManagerTypes.Add(subManager.GetType());
            }

            // Iterate over the subManagerDataList and synchronize the subManagers list
            foreach (SubManagerData subManagerData in gameManagerData.subManagerDataList)
            {
                Type dataType = Type.GetType(subManagerData.dataTypeName);

                if (dataType == null)
                {
                    Debug.LogWarning($"SubManagerData has an invalid type: {subManagerData.dataTypeName}");
                    continue;
                }

                LinkDataLogicAttribute attribute = dataType.GetCustomAttribute<LinkDataLogicAttribute>();
                if (attribute == null)
                {
                    Debug.LogWarning($"SubManagerData is not associated with any logic class: {subManagerData.dataTypeName}");
                    continue;
                }

                // Check if the sub-manager data type is already in the list
                if (!existingSubManagerTypes.Contains(dataType))
                {
                    // Create a new instance of the data class
                    ISubManagerData newSubManager = (ISubManagerData)Activator.CreateInstance(dataType);

                    if (newSubManager == null)
                    {
                        Debug.LogWarning($"Failed to create subManager for {dataType.FullName}");
                        continue;
                    }

                    gameManagerData.subManagers.Add(newSubManager);

                    // Add the new sub-manager's type to the set
                    existingSubManagerTypes.Add(dataType);

                    // Add a new serialized property entry
                    subManagersProperty.InsertArrayElementAtIndex(gameManagerData.subManagers.Count - 1);
                    SerializedProperty newElement = subManagersProperty.GetArrayElementAtIndex(gameManagerData.subManagers.Count - 1);
                    newElement.managedReferenceValue = newSubManager;
                }
            }

            // Clean up any sub-managers that no longer have a valid data link
            for (int i = gameManagerData.subManagers.Count - 1; i >= 0; i--)
            {
                ISubManagerData subManager = gameManagerData.subManagers[i];
                if (!IsSubManagerLinked(gameManagerData, subManager))
                {
                    Debug.LogWarning($"Removing unlinked subManager: {subManager.GetType().Name}");
                    gameManagerData.subManagers.RemoveAt(i);

                    // Remove the corresponding serialized property
                    subManagersProperty.DeleteArrayElementAtIndex(i);
                }
            }

            // Apply the changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }

        private bool IsSubManagerLinked(GameManagerData gameManagerData, ISubManagerData subManager)
        {
            Type subManagerDataType = subManager.GetType();

            foreach (SubManagerData subManagerData in gameManagerData.subManagerDataList)
            {
                Type dataType = Type.GetType(subManagerData.dataTypeName);

                if (dataType != null)
                {
                    LinkDataLogicAttribute attribute = dataType.GetCustomAttribute<LinkDataLogicAttribute>();

                    if (attribute != null && attribute.DataClassType == subManagerDataType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}