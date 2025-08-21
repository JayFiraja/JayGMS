using System.Collections.Generic;
using UnityEngine;

namespace GMS.Samples
{
    public class TicTacToeStateManager : ISubManager
    {
        private TicTacToeStateData _data;

        /// <summary>
        /// Getter for current game state
        /// </summary>
        public GameState CurrentState { get; private set; }
        /// <summary>
        /// Getter for current Player Turn
        /// </summary>
        public PlayerTurn PlayerTurn { get; private set; }
        /// <summary>
        /// True if AI is playing as PlayerB (by definition)
        /// </summary>
        public bool IsAIPlaying { get; private set; }
        /// <summary>
        /// Control for disallowing to play marks until 1 has been fully registered
        /// </summary>
        public bool CanPlayCell { get; private set; }

        private int[,] _gameGrid;
        private bool _hasAIPlayedMarker;
        private float _timeSinceResultsScreen;

        private TicTacToeAudio _audioManager;
        private TimeManager _timeManager;

        public TicTacToeStateManager(TicTacToeStateData data)
        {
            _data = data;
            CurrentState = GameState.UnInitialized;
        }

        public bool Initialize(GameManager gameManager)
        {
            GoToGameState(GameState.Loading);

            GameEventService.RegisterListener<TTCEvents.GoToGameState>(OnGoToGameState);
            GameEventService.RegisterListener<TTCEvents.CellDataSelected>(CellDataSelected);
            GameEventService.RegisterListener<TTCEvents.MarkDrawCompletedOnCell>(OnMarkDrawCompletedOnCell);
            GameEventService.RegisterListener<TTCEvents.StartGameLoop>(StartGameLoop);
            GameEventService.RegisterListener<TTCEvents.UsingAlternativeMarkToggled>(OnPlayerUsingAlternateMark);

            gameManager.TryGetSubManager(out _timeManager);
            gameManager.TryGetSubManager(out _audioManager);

            _timeManager?.AddCooldown(_data.TimeToStartMainMenu, GoToGameState, GameState.MainMenu);
            return true;
        }

        public void UnInitialize()
        {
            GameEventService.UnregisterListener<TTCEvents.GoToGameState>(OnGoToGameState);
            GameEventService.UnregisterListener<TTCEvents.CellDataSelected>(CellDataSelected);
            GameEventService.UnregisterListener<TTCEvents.MarkDrawCompletedOnCell>(OnMarkDrawCompletedOnCell);
            GameEventService.UnregisterListener<TTCEvents.StartGameLoop>(StartGameLoop);
            GameEventService.UnregisterListener<TTCEvents.UsingAlternativeMarkToggled>(OnPlayerUsingAlternateMark);
        }

        public void OnUpdate()
        {
            if (CurrentState == GameState.Results)
            {
                bool transitionToMainMenu = Time.timeSinceLevelLoad > (_timeSinceResultsScreen + _data.TimeToMoveFromResultsToMainMenu);
                if (transitionToMainMenu)
                {
                    GoToGameState(GameState.MainMenu);
                }
            }

            if (CurrentState == GameState.GameLoop)
            {
                if (IsAIPlaying && PlayerTurn == PlayerTurn.PlayerB)
                {
                    if (!_hasAIPlayedMarker && GameLogic.TryGetRandomEmptyCell(_gameGrid, out Vector2 emptyCell))
                    {
                        TTCEvents.PlayAICell playAICellEvent = GameEventService.GetEvent<TTCEvents.PlayAICell>();
                        playAICellEvent.Coords = emptyCell;
                        GameEventService.TriggerEvent(playAICellEvent);
                        _hasAIPlayedMarker = true;
                    }
                }
            }
        }

        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        private void CellDataSelected(TTCEvents.CellDataSelected cellDataSelected)
        {
            CanPlayCell = false;
        }

        /// <summary>
        /// Handles the event triggered to transition to a new game state.
        /// </summary>
        private void OnGoToGameState(TTCEvents.GoToGameState eventData)
        {
            GoToGameState(eventData.NewGameState);
        }
        
        /// <summary>
        /// Transitions the game to a new state and triggers any necessary state change events.
        /// </summary>
        /// <param name="newGameState">The new game state to transition to.</param>
        private void GoToGameState(GameState newGameState)
        {
            if (CurrentState == newGameState)
            {
                return;
            }

            GameState lastState = CurrentState;
            CurrentState = newGameState;

            TTCEvents.GameStateChanged gameStateChanged = GameEventService.GetEvent<TTCEvents.GameStateChanged>();
            gameStateChanged.FromGameState = lastState;
            gameStateChanged.ToGameState = newGameState;
            GameEventService.TriggerEvent(gameStateChanged);
        }

        /// <summary>
        /// Single point where the game loop can start from.
        /// </summary>
        private void StartGameLoop(TTCEvents.StartGameLoop startGameLoop)
        {
            if (CurrentState == GameState.GameLoop)
            {
                return;
            }

            GoToGameState(GameState.GameLoop);

            // update our local variables.
            _gameGrid = GameLogic.CreateNewGrid();
            PlayerTurn = startGameLoop.PlayerTurn;
            IsAIPlaying = startGameLoop.IsSinglePlayer;
            CanPlayCell = true;
        }

        private void OnMarkDrawCompletedOnCell(TTCEvents.MarkDrawCompletedOnCell markDrawCompletedOnCell)
        {
            _timeManager?.AddCooldown(cooldown: _data.TimeToCallNextPlayerTurn, cooldownFinished: CheckForWinner, parameter: markDrawCompletedOnCell.CellData);
        }

        private void CheckForWinner(CellData cellData)
        {
            if (TryRegisterNewMarker(cellData, out int markerRegistered))
            {
                EvaluateWinner(markerRegistered);
            }
        }

        /// <summary>
        /// Tries to register a new marker on the given cellData,
        /// updating the unique grid <see cref="_gameGrid"/>
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="markerRegistered"> mark is registered depending on current player turn</param>
        /// <returns>True if operation is successful, false if a marker was already registered</returns>
        private bool TryRegisterNewMarker(CellData cellData, out int markerRegistered)
        {
            markerRegistered = 0;
            // register new Move
            int x = (int)cellData.Coords.x;
            int y = (int)cellData.Coords.y;

            if (_gameGrid[x, y] != 0)
            {
                // Already registered
                return false;
            }

            switch (PlayerTurn)
            {
                case PlayerTurn.PlayerA:
                    markerRegistered = 1;
                    break;
                case PlayerTurn.PlayerB:
                    markerRegistered = 2;
                    break;
            }

            _gameGrid[x, y] = markerRegistered;
            return true;
        }

        private void EvaluateWinner(int marker)
        {
            // Evaluate winner
            if (GameLogic.CheckWinner(_gameGrid, target: marker, out List<Vector2> matchingCells))
            {
                // Inform the winning sequence
                TTCEvents.WinningSequence winingSequenceEvent = GameEventService.GetEvent<TTCEvents.WinningSequence>();
                winingSequenceEvent.CoordsSequence = matchingCells;
                GameEventService.TriggerEvent(winingSequenceEvent);

                // Save which player won, this may unlock an alternative mesh for that player
                // using player prefs to save for simplicity reasons in this prototype
                PlayerPrefs.SetInt(PlayerTurn.ToString(), 1);

                // Fire Game won by current player
                TTCEvents.PlayerWon playerWonEvent = GameEventService.GetEvent<TTCEvents.PlayerWon>();
                playerWonEvent.PlayerTurn = PlayerTurn;
                playerWonEvent.IsSinglePlayer = IsAIPlaying;
                GameEventService.TriggerEvent(playerWonEvent);
                
                _timeSinceResultsScreen = Time.timeSinceLevelLoad;
                // Go to results state
                GoToGameState(GameState.Results);

                _audioManager?.PlayWinVoice();

                // little demo of storing stats...
                if (!IsAIPlaying || PlayerTurn == PlayerTurn.PlayerA)
                {
                    int totalPlayerWinCounts = PlayerPrefs.GetInt(TicTacToeStateData.TOTAL_WIN_PLAYER_COUNT);
                    totalPlayerWinCounts++;
                    PlayerPrefs.SetInt(TicTacToeStateData.TOTAL_WIN_PLAYER_COUNT, totalPlayerWinCounts);
                }
                else if (IsAIPlaying)
                {
                    int totalAIWinCounts = PlayerPrefs.GetInt(TicTacToeStateData.TOTAL_WINS_AI_COUNT);
                    totalAIWinCounts++;
                    PlayerPrefs.SetInt(TicTacToeStateData.TOTAL_WINS_AI_COUNT, totalAIWinCounts);
                }
            }
            else
            {
                // check if nobody won and there are no free cells
                if (!GameLogic.TryGetRandomEmptyCell(_gameGrid, out Vector2 emptyCell))
                {
                    CanPlayCell = false;
                    _audioManager?.PlayDraw();
                    _timeSinceResultsScreen = Cooldown.Cool();
                    TTCEvents.GameEndedInDraw gameEndedEvent = GameEventService.GetEvent<TTCEvents.GameEndedInDraw>();
                    GameEventService.TriggerEvent(gameEndedEvent);
                    GoToGameState(GameState.Results);
                    return;
                }

                // Call next player turn
                PlayerTurn nextTurn = PlayerTurn == PlayerTurn.PlayerA ? PlayerTurn.PlayerB : PlayerTurn.PlayerA;
                PlayerTurn = nextTurn;
                if (IsAIPlaying && PlayerTurn == PlayerTurn.PlayerB)
                {
                    // only reset this flag here
                    _hasAIPlayedMarker = false;
                }

                CanPlayCell = true;
                
                TTCEvents.PlayerTurnChanged playerTurnChangedEvent = GameEventService.GetEvent<TTCEvents.PlayerTurnChanged>();
                playerTurnChangedEvent.PlayerTurn = PlayerTurn;
                playerTurnChangedEvent.IsSinglePlayer = IsAIPlaying;
                GameEventService.TriggerEvent(playerTurnChangedEvent);

                // little demo of storing stats...
                int totalTurnCounts = PlayerPrefs.GetInt(TicTacToeStateData.TOTAL_TURN_COUNT);
                totalTurnCounts++;
                PlayerPrefs.SetInt(TicTacToeStateData.TOTAL_TURN_COUNT, totalTurnCounts);
            }
        }

        public bool TryGetCurrentPlayerTurnData(out PlayerData foundPlayerData)
        {
            foundPlayerData = new PlayerData();
            foundPlayerData.PlayerTurn = PlayerTurn.None;

            PlayerTurn playerTurn = PlayerTurn;
            return TryGetPlayerTurnData(playerTurn, out foundPlayerData);
        }

        public bool TryGetPlayerTurnData(PlayerTurn playerTurn, out PlayerData foundPlayerData)
        {
            foundPlayerData = new PlayerData();
            foundPlayerData.PlayerTurn = PlayerTurn.None;

            foreach (PlayerData playerData in _data.PlayerDatas)
            {
                if (playerData.PlayerTurn == playerTurn)
                {
                    foundPlayerData = playerData;
                    break;
                }
            }

            return foundPlayerData.PlayerTurn != PlayerTurn.None;
        }

        private void OnPlayerUsingAlternateMark(TTCEvents.UsingAlternativeMarkToggled usingAlternativeMarkToggled)
        {
            PlayerTurn = usingAlternativeMarkToggled.PlayerData.PlayerTurn;
            bool usingAlternativeMark = usingAlternativeMarkToggled.UsingAlternativeMark;
            
            for (int i = 0; i < _data.PlayerDatas.Length; i++)
            {
                if (_data.PlayerDatas[i].PlayerTurn == PlayerTurn)
                {
                    _data.PlayerDatas[i].usingAlternativeMark = usingAlternativeMark;
                    break;
                }
            }
        }
    }
}
