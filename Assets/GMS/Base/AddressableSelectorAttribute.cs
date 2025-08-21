using System;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Attribute for strings, works with <see cref="AddressableSelectorDrawer"/>
    /// in order to draw a select button and popup for easily selecting an existing Addressable key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AddressableSelectorAttribute : PropertyAttribute
    {

    }
}
