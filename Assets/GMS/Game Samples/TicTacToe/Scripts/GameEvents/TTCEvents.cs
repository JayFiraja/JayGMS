using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
	public class TTCEvents
	{
		[GameEvent("GoToGameState", "Attempts to transition to a GameState")]
		public class GoToGameState : BaseGameEvent
		{
			public GameState NewGameState;
		}

		[GameEvent("StartGameLoop", "Communicates starting player and if the PlayerB is an AI")]
		public class StartGameLoop : BaseGameEvent
		{
			public PlayerTurn PlayerTurn;
			public bool IsSinglePlayer;
		}
		
		[GameEvent("GameStateChanged", "Communicates the from GameState and a new GameState")]
		public class GameStateChanged : BaseGameEvent
		{
			public GameState FromGameState;
			public GameState ToGameState;
		}
		
		[GameEvent("OnPlayerTurnChanged", "Informs that there is a new player turn change, bool is for isSinglePlayer")]
		public class PlayerTurnChanged : BaseGameEvent
		{
			public PlayerTurn PlayerTurn;
			public bool IsSinglePlayer;
		}
		
		[GameEvent("CellDataSelected", "Communicates which cell data has been selected, Used for decoupling the logic from the view.")]
		public class CellDataSelected : BaseGameEvent
		{
			public CellData SelectedCellData;
		}
		
		/// <summary>
		/// This then informs the logic to process winner checks and proceed with the other player's turn
		/// </summary>
		[GameEvent("MarkDrawCompletedOnCell", "Communicates the completion of the mark being drawn on the cell.")]
		public class MarkDrawCompletedOnCell : BaseGameEvent
		{
			public CellData CellData;
		}
		
		[GameEvent("PlayerWon", "Player won event, if bool is true, it was the AI as PlayerB only")]
		public class PlayerWon : BaseGameEvent
		{
			public PlayerTurn PlayerTurn;
			public bool IsSinglePlayer;
		}

		[GameEvent("GameEndedInDraw", "Both players ended up in a draw.")]
		public class GameEndedInDraw : BaseGameEvent
		{
			// no data needed
		}
		
		[GameEvent("PlayAICell", "Play a cell coords for the AI")]
		public class PlayAICell : BaseGameEvent
		{
			public Vector2 Coords;
		}
		
		[GameEvent("UsingAlternativeMarkToggled", "Communicates the mark toggle for using alternative meshes.")]
		public class UsingAlternativeMarkToggled : BaseGameEvent
		{
			public PlayerData PlayerData;
			public bool UsingAlternativeMark;
		}
		
		[GameEvent("WinningSequence", "Contains the matching sequence with the Coords of the winning cells.")]
		public class WinningSequence : BaseGameEvent
		{
			public List<Vector2> CoordsSequence;
		}
	}
}
