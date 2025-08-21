using UnityEditor;
using System;
using System.Reflection;
using GMS;

[CustomEditor(typeof(GameEventCaller))]
public class GameEventSelectorEditor : Editor
{
    private string[] eventNames;
    private int selectedIndex;
    private const string NO_EVENT_SELECTED_NAME = "Select a Game Event";

    // The serialized property representing the string field
    private SerializedProperty selectedEventNameProp;

    private void OnEnable()
    {
        // Get the SerializedProperty for the selectedEventName field
        selectedEventNameProp = serializedObject.FindProperty("selectedEventName");

        // Retrieve the event names at the time the editor is enabled
        GetEventNames();

        // Set the index based on the currently selected event name
        if (string.IsNullOrEmpty(selectedEventNameProp.stringValue))
        {
            selectedIndex = 0; // Default to "None"
        }
        else
        {
            selectedIndex = Array.IndexOf(eventNames, selectedEventNameProp.stringValue);
        }

        if (selectedIndex == -1)
        {
            selectedIndex = 0; // Fallback to "None" if the name doesn't exist
        }
    }

    private void GetEventNames()
    {
        // Get all assemblies in the current AppDomain
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var selectionList = new System.Collections.Generic.List<string>();

        // Add a default option at the beginning
        selectionList.Add(NO_EVENT_SELECTED_NAME);

        // Loop through each assembly to find events
        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes();

            // Loop through each type in the assembly
            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.IsSubclassOf(typeof(BaseGameEvent)))
                {
                    // Create an instance of the event class and add its name
                    BaseGameEvent eventInstance = (BaseGameEvent)Activator.CreateInstance(type);
                    selectionList.Add(eventInstance.EventName);
                }
            }
        }

        // Convert list to array for the dropdown
        eventNames = selectionList.ToArray();
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Dropdown for selecting event
        selectedIndex = EditorGUILayout.Popup("Select Game Event", selectedIndex, eventNames);

        // Update the selectedEventName property based on the dropdown selection
        if (selectedIndex == 0)
        {
            // If "None" is selected, clear the selectedEventName
            selectedEventNameProp.stringValue = string.Empty;
        }
        else if (selectedIndex > 0 && selectedIndex < eventNames.Length)
        {
            selectedEventNameProp.stringValue = eventNames[selectedIndex];
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
