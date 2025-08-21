using System;

namespace GMS
{
    /// <summary>
    /// This class allows us to link a Logic class to a data struct
    /// The use case is to initialize a Logic class and pass in the data in it's constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class LinkDataLogicAttribute : Attribute
    {
        /// <summary>
        /// The type of our data class.
        /// </summary>
        public Type DataClassType { get; }
        /// <summary>
        /// Display  Name for the Data class
        /// </summary>
        public string DataDisplayName { get; }
        /// <summary>
        /// The type of our logic class.
        /// </summary>
        public Type LogicClassType { get; }
        /// <summary>
        /// Display name for the logic class
        /// </summary>
        public string DisplayName { get; }
    
        public LinkDataLogicAttribute(Type dataClassType,string dataDisplayName,  Type logicClassType, string displayName)
        {
            DataClassType = dataClassType;
            DataDisplayName = dataDisplayName;
            LogicClassType = logicClassType;
            DisplayName = string.IsNullOrEmpty(displayName) ? logicClassType.Name : displayName;
        }
    }
}