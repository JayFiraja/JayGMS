using UnityEngine;

namespace GMS.Samples
{
	/// <summary>
	/// Represents a minimal game structure used within the Game Management System (GMS) sample.
	/// This class contains events and functionality related to the game's basic operations.
	/// </summary>
	public class MinimalGameEvents
	{
		[GameEvent("OnPlayerInstanced", "On Player Controllable Character Instanced")]
		public class OnPlayerInstanced : BaseGameEvent
		{
			public GameObject PlayerGameObject;
		}

		[GameEvent("OnControllableCharacterMoved", "Fired when a Controllable Character Moved to a new position")]
		public class OnControllableCharacterMoved : BaseGameEvent
		{
			public Vector3 NewPosition;
		}

		[GameEvent("ItemPickedUp", "Fired when an item has been picked up")]
		public class ItemPickedUp : BaseGameEvent
		{
			public IPickUp PickupItemUp;
			public bool PickedByPlayableCharacter;
		}
	}
}
