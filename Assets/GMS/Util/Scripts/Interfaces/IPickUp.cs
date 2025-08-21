using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public interface IPickUp
    {
        bool HasBeenPickedUp { get; }
        void PickUp();
    }
}
