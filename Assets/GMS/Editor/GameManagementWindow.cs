using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GMS.Editor
{
    public class GameManagementWindow : EditorWindow
    {
        private const string DEFAULT_NAMESPACE = "GMS";
        private const string GAMEMANAGEMENT_ROOT = "Assets/GMS/";
        private const string LOGIC_CLASSES_PATH_KEY = "LogicClassesPath";
        private const string DATA_STRUCTS_PATH_KEY = "DataClassesPath";
        private const string LOGIC_CLASS_FOLDER_NAME = "SubManagerLogic";
        private const string DATA_STRUCTS_FOLDER_NAME = "SubManagerDatas";

        private const string CREATE_SUBMANAGER_LABEL = "Type your file names";
        private const string SAVE_FILE_LABEL = "Files Save Locations";
        private const string LOGIC_CLASSES = "Logic Classes Save Location";
        private const string DATA_STRUCTS = "Data Structs Save Location";
        private const string TIP_EMPTY_STRINGS = "Class name and Data Struct name cannot be empty.";
        private const string TIP_FILE_ALREADY_EXISTS = "File with name already exists";
        private const string EXECUTE_FILE_CREATION = "Create Now!";
        private const string EXISTING_SUBMANAGERS_LABEL = "Existing SubManagers";

        private string _customNamespace = "YourNamespace";
        private bool _useCustomNamespace = false;
        private string _logicClassName = "NewLogicClass";
        private string _dataStructName = "NewDataStruct";

        private string _existingSubManagerButtonsTip = "Select the buttons to navigate to the file location.";

        private Vector2 _scrollPosition;
        private Vector2 _classListScrollPosition;
        private string[] _existingClasses;
        private List<SubManagerGroup> _subManagerGroups = new List<SubManagerGroup>();
        private string _defaultLogicClassesPath;
        private string _defaultDataClassesPath;
        private string _logicClassesPath;
        private string _dataStructsPath;

        private Color _HeaderBoxColour = new Color(0.78f, 0.4f, 0.33f);
        private Color _darkBlueBackground = new Color(0.12f, 0.12f, 0.18f);
        private Color _darkBlue = new Color(0.2f, 0.2f, 0.25f);
        private Color _darkGreen = new Color(0.12f, 0.18f, 0.12f, 1f);
        private Color _normalTrashColour = new Color(1f, 0.1f, 0.1f);
        private Color _hoverTrashColour = new Color(1f, 0.5f, 0.5f);
        private Color _activeTrashColour = new Color(0.8f, 0f, 0f);
        private Color _textColour = Color.white;
        private Color _originalGUIBackgroundColour;
        private Color _originalGUIContentColour;
        private GUIStyle _toggleStyle;
        private static Texture2D _gmsIcon;
        private static Color _originalColor;

        private class SubManagerGroup
        {
            public string LogicClass;
            public string DataStruct;
            public bool IsSelected;
            public string LogicClassRelativePath;
            public string DataStructRelativePath;
        }

        [MenuItem("Tools/GMS/Create SubManager")]
        public static void ShowWindow()
        {
            GameManagementWindow window = GetWindow<GameManagementWindow>("GMS Create SubManager");
            window.minSize = new Vector2(1050, 350);
            _originalColor = GUI.color;
        }

        private void OnEnable()
        {
            _gmsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GMS/Editor/Sprites/gmsIcon.png");
            _defaultLogicClassesPath = Application.dataPath + $"/GMS/{LOGIC_CLASS_FOLDER_NAME}";
            _defaultDataClassesPath = Application.dataPath + $"/GMS/{DATA_STRUCTS_FOLDER_NAME}";
            _logicClassesPath = EditorPrefs.GetString(LOGIC_CLASSES_PATH_KEY, _defaultLogicClassesPath);
            _dataStructsPath = EditorPrefs.GetString(DATA_STRUCTS_PATH_KEY, _defaultDataClassesPath);
            _originalGUIBackgroundColour = GUI.backgroundColor;
            RefreshExistingClasses();
        }

        private void DelayedInitialize()
        {
            // Remove this method from the update loop
            EditorApplication.update -= DelayedInitialize;

            // Re-fetch data or reset necessary variables here
            RefreshExistingClasses();
        }
        private void CacheToggleStyle()
        {
            _toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { textColor = Color.white },
                onNormal = { textColor = Color.white },
                hover = { textColor = Color.white },
                onHover = { textColor = Color.white },
                active = { textColor = Color.white },
                onActive = { textColor = Color.white },
                focused = { textColor = Color.white },
                onFocused = { textColor = Color.white }
            };
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            CacheToggleStyle();
            _originalGUIContentColour = GUI.contentColor;
            _originalGUIBackgroundColour = GUI.backgroundColor;


            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    GUIBeginColouredBox(_darkBlueBackground);
                    GUICreateParametersSection();
                    GUICreateExecuteButton();
                    GUIDrawFolderSelection();
                    GUILayout.EndVertical();
                }

                EditorGUILayout.Space(5);

                using (new EditorGUILayout.VerticalScope())
                {
                    GUIDrawExistingSubManagers();
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        #region GUI Methods

        private void GUICreateParametersSection()
        {
            GUIWriteTipIconWithLabel(CREATE_SUBMANAGER_LABEL);

            GUI.backgroundColor = _darkBlue;

            EditorGUI.BeginChangeCheck();
            _logicClassName = EditorGUILayout.TextField("Logic Class Name", _logicClassName);
            if (EditorGUI.EndChangeCheck())
            {
                CheckFileExistence(_logicClassName, "Logic");
            }

            EditorGUI.BeginChangeCheck();
            _dataStructName = EditorGUILayout.TextField("Data Struct Name", _dataStructName);
            if (EditorGUI.EndChangeCheck())
            {
                CheckFileExistence(_dataStructName, "Data");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"Use Custom namespace (default: {DEFAULT_NAMESPACE})", GUILayout.Width(330));

                GUI.backgroundColor = _originalGUIBackgroundColour;
                _useCustomNamespace = EditorGUILayout.Toggle(_useCustomNamespace, _toggleStyle, GUILayout.Width(20));
                GUI.backgroundColor = _originalGUIBackgroundColour;
                GUI.contentColor = _originalGUIContentColour;
            }

            if (_useCustomNamespace)
            {
                _customNamespace = EditorGUILayout.TextField("Custom namespace", _customNamespace);
            }
        }

        private void GUIDrawExistingSubManagers()
        {
            GUIBeginColouredBox(_darkGreen);

            EditorGUILayout.BeginHorizontal();
            GUIWriteTipIconWithLabel(EXISTING_SUBMANAGERS_LABEL);
            GUILayout.FlexibleSpace();

            GUIStyle redButtonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { background = MakeTexture(1, 1, _normalTrashColour), textColor = _textColour },
                hover = { background = MakeTexture(1, 1, _hoverTrashColour) },
                active = { background = MakeTexture(1, 1, _activeTrashColour) }
            };

            Texture2D trashBinIcon = EditorGUIUtility.IconContent("TreeEditor.Trash").image as Texture2D;

            bool atLeastOneSelected = _subManagerGroups.Any(group => group.IsSelected);
            if (atLeastOneSelected && GUILayout.Button(new GUIContent(string.Empty, trashBinIcon), redButtonStyle, GUILayout.Width(40)))
            {
                List<SubManagerGroup> selectedGroups = _subManagerGroups.Where(group => group.IsSelected).ToList();
                if (selectedGroups.Any())
                {
                    string message = "Are you sure you want to delete the following SubManager Logic Classes and Data Structs?\n\n" +
                                     string.Join("\n", selectedGroups.Select(group => $"{group.LogicClass} and {group.DataStruct}"));

                    bool confirmDelete = EditorUtility.DisplayDialog(
                        "Confirm Delete",
                        message,
                        "Delete",
                        "Cancel"
                    );

                    if (confirmDelete)
                    {
                        foreach (var group in selectedGroups)
                        {
                            TryDeleteFile(group.LogicClass, isLogicClass: true);
                            TryDeleteFile(group.DataStruct, isLogicClass: false);
                        }
                        RefreshExistingClasses();
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            _classListScrollPosition = EditorGUILayout.BeginScrollView(_classListScrollPosition, GUILayout.Height(260));

            EditorGUILayout.LabelField(_existingSubManagerButtonsTip);
            float buttonWidth = 300f;
            for (int i = 0; i < _subManagerGroups.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SubManagerGroup subManagerGroup = _subManagerGroups[i];

                GUI.backgroundColor = _originalGUIBackgroundColour;
                subManagerGroup.IsSelected = EditorGUILayout.Toggle(subManagerGroup.IsSelected, _toggleStyle, GUILayout.Width(20));
                GUI.backgroundColor = _originalGUIBackgroundColour;
                GUI.contentColor = _originalGUIContentColour;

                if (GUILayout.Button(subManagerGroup.LogicClass ?? "N/A", GUILayout.Width(buttonWidth)))
                {
                    UnityEngine.Object logicClassAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(subManagerGroup.LogicClassRelativePath);
                    if (logicClassAsset != null)
                    {
                        Selection.activeObject = logicClassAsset;
                        EditorGUIUtility.PingObject(logicClassAsset);
                    }
                }

                if (GUILayout.Button(subManagerGroup.DataStruct ?? "N/A", GUILayout.Width(buttonWidth)))
                {
                    UnityEngine.Object dataStructAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(subManagerGroup.DataStructRelativePath);
                    if (dataStructAsset != null)
                    {
                        Selection.activeObject = dataStructAsset;
                        EditorGUIUtility.PingObject(dataStructAsset);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void GUIDrawFolderSelection()
        {
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = _darkBlue;
            GUILayout.BeginVertical(GUI.skin.box);

            GUIWriteTipIconWithLabel(SAVE_FILE_LABEL);

            EditorGUILayout.Space(5);

            GUIDrawFolderIconWithPath(ref _logicClassesPath, LOGIC_CLASSES, isFolder: true, defaultPathKey: LOGIC_CLASSES_PATH_KEY, prefsKey: LOGIC_CLASSES_PATH_KEY);
            GUIDrawFolderIconWithPath(ref _dataStructsPath, DATA_STRUCTS, isFolder: true, defaultPathKey: DATA_STRUCTS_PATH_KEY, prefsKey: DATA_STRUCTS_PATH_KEY);

            GUI.backgroundColor = originalColor;
            EditorGUILayout.EndVertical();
        }

        private void GUICreateExecuteButton()
        {
            bool logicClassExists = File.Exists(GetRelativeLogicClassPath());
            bool dataStructExists = File.Exists(GetRelativeDataStructPath());

            if (string.IsNullOrEmpty(_logicClassName) || string.IsNullOrEmpty(_dataStructName))
            {
                EditorGUILayout.HelpBox(TIP_EMPTY_STRINGS, MessageType.Warning);
            }
            else if (logicClassExists || dataStructExists)
            {
                EditorGUILayout.HelpBox(TIP_FILE_ALREADY_EXISTS, MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button(EXECUTE_FILE_CREATION, GUILayout.MinHeight(40)))
                {
                    CreateSubManager(GetRelativeLogicClassPath(), GetRelativeDataStructPath());
                }
            }
        }

        private void GUIDrawFolderIconWithPath(ref string path, string fileName, bool isFolder = false, string defaultPathKey = "", string prefsKey = "")
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                float fieldHeight = 50f;

                if (GUILayout.Button(EditorGUIUtility.IconContent("Folder Icon"), GUILayout.Width(50), GUILayout.Height(fieldHeight)))
                {
                    string newPath;
                    if (isFolder)
                    {
                        newPath = EditorUtility.OpenFolderPanel($"Select {fileName}", "Assets", "");
                    }
                    else
                    {
                        newPath = EditorUtility.OpenFilePanel($"Select {fileName}", "Assets", "cs");
                    }

                    if (!string.IsNullOrEmpty(newPath) && newPath != path)
                    {
                        path = newPath;
                        EditorPrefs.SetString(prefsKey, path);
                        RefreshExistingClasses();
                    }
                }

                if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                {
                    string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                    Rect labelRect = GUILayoutUtility.GetRect(new GUIContent($"{fileName} File: " + relativePath), GUI.skin.label, GUILayout.Height(fieldHeight));

                    float labelY = labelRect.y + (labelRect.height - EditorGUIUtility.singleLineHeight) / 2;

                    EditorGUI.LabelField(new Rect(labelRect.x, labelY, labelRect.width, EditorGUIUtility.singleLineHeight),
                        new GUIContent($"{fileName} File: " + relativePath, $"Path to the {fileName}.cs file."));
                }

                if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(fieldHeight)))
                {
                    path = defaultPathKey == LOGIC_CLASSES_PATH_KEY ? _defaultLogicClassesPath : _defaultDataClassesPath;
                    EditorPrefs.SetString(prefsKey, path);
                    RefreshExistingClasses();
                }
            }
        }

        #endregion GUI Methods

        private void CreateSubManager(string relativeLogicClassPath, string relativeDataStructPath)
        {
            SubManagerWriter.CreateLogicClass(relativeLogicClassPath, _useCustomNamespace, DEFAULT_NAMESPACE,
                _customNamespace, _dataStructName, _logicClassName);

            SubManagerWriter.CreateDataStruct(relativeDataStructPath, _useCustomNamespace, DEFAULT_NAMESPACE,
                _customNamespace, _dataStructName, _logicClassName);

            AssetDatabase.Refresh();
        }

        private string GetRelativeLogicClassPath()
        {
            return Path.Combine(_logicClassesPath, $"{_logicClassName}.cs");
        }

        private string GetRelativeDataStructPath()
        {
            return Path.Combine(_dataStructsPath, $"{_dataStructName}.cs");
        }

        private void RefreshExistingClasses()
        {
            string[] logicFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
            string[] dataFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

            List<string> logicClassNames = new List<string>();
            List<string> dataStructNames = new List<string>();

            foreach (string file in logicFiles)
            {
                string className = Path.GetFileNameWithoutExtension(file);
                logicClassNames.Add(className);
            }

            foreach (string file in dataFiles)
            {
                string structName = Path.GetFileNameWithoutExtension(file);
                dataStructNames.Add(structName);
            }

            List<Type> logicTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => logicClassNames.Contains(type.Name) && typeof(ISubManager).IsAssignableFrom(type))
                .ToList();

            List<Type> dataTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => dataStructNames.Contains(type.Name) && type.GetCustomAttribute<LinkDataLogicAttribute>() != null)
                .ToList();

            if (_subManagerGroups != null)
            {
                _subManagerGroups.Clear();
            }
            else
            {
                _subManagerGroups = new List<SubManagerGroup>();
            }

            foreach (Type dataType in dataTypes)
            {
                var attribute = dataType.GetCustomAttribute<LinkDataLogicAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                string logicClassName = attribute.LogicClassType.Name;
                string dataStructName = dataType.Name;

                string logicClassRelativePath = GetRelativePathForFile(logicFiles, logicClassName);
                string dataStructRelativePath = GetRelativePathForFile(dataFiles, dataStructName);

                _subManagerGroups.Add(new SubManagerGroup
                {
                    LogicClass = logicClassName,
                    DataStruct = dataStructName,
                    IsSelected = false,
                    LogicClassRelativePath = logicClassRelativePath,
                    DataStructRelativePath = dataStructRelativePath
                });
            }
        }

        private void CheckFileExistence(string className, string classType)
        {
            string path = classType == "Logic" ? _logicClassesPath : _dataStructsPath;
            string filePath = Path.Combine(path, $"{className}.cs");
            if (File.Exists(filePath))
            {
                EditorGUILayout.HelpBox($"Warning: A {classType} class with the name '{className}' already exists.", MessageType.Warning);
            }
        }

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = color;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void GUIBeginColouredBox(Color color)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 10, 10),
                normal = { background = MakeTexture(1, 1, color) }
            };
            EditorGUILayout.BeginVertical(boxStyle);
        }

        private bool TryDeleteFile(string fileName, bool isLogicClass)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            string path = isLogicClass ? _logicClassesPath : _dataStructsPath;
            string filePath = Path.Combine(path, $"{fileName}.cs");

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    AssetDatabase.Refresh();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete file {fileName}: {e.Message}");
                return false;
            }

            return true;
        }

        private void GUIWriteTipIconWithLabel(string labelText)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle infoLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 15,
                    alignment = TextAnchor.LowerLeft,
                    normal = { textColor = Color.white },
                    fontStyle = FontStyle.Bold
                };

                GUIContent iconContent = EditorGUIUtility.IconContent("console.infoicon.sml@2x");
                GUIContent labelContent = new GUIContent(labelText);

                EditorGUILayout.LabelField(iconContent, GUILayout.Width(20), GUILayout.Height(20));
                EditorGUILayout.LabelField(labelContent, infoLabelStyle, GUILayout.Height(20));
            }
        }

        private string GetRelativePathForFile(string[] files, string className)
        {
            foreach (var file in files)
            {
                if (Path.GetFileNameWithoutExtension(file) == className)
                {
                    return "Assets" + file.Substring(Application.dataPath.Length);
                }
            }
            return string.Empty;
        }
    }
}
