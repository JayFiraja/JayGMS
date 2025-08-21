using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GMS.Samples
{
    /// <summary>
    /// Manages the Creation of the tic tac toe grid, in Data and View sides.
    /// Calls Spawn and Despawn interfaces based on GameState communicated by GlobalActions.
    /// 
    /// </summary>
    public class TicTacToeGridManager: ISubManager
    {
        private TicTacToeGridData _data;
        private GridData _gridData;

        private TicTacToeAudio _audio;
        private TicTacToeStateManager _stateManager;
        private TimeManager _timeManager;

        // View Elements
        public GameObject _gridParent;
        private const string GRID_PARENT_NAME = "Grid_Container";
        public Transform _cellsContainer;
        public Transform _marksContainer;
        public Transform _lightsContainer;
        private List<Light> _winningSequenceLights = new List<Light>();

        // Variables
        private List<Marker> _markersPool = new List<Marker>();
        private List<Cell_Interact> _cellInteracts = new List<Cell_Interact>();
        private bool _gridInitialized = false;
        private Queue<CellData> cellDataQueue = new Queue<CellData>();

        public TicTacToeGridManager(TicTacToeGridData data)
        {
            _data = data;
        }
        
        public bool Initialize(GameManager gameManager)
        {
            Addressables.LoadAssetAsync<GridData>(_data.GridDataAddress).Completed += OnGridDataLoaded;
            gameManager.TryGetSubManager(out _audio);
            gameManager.TryGetSubManager(out _timeManager);
            gameManager.TryGetSubManager(out _stateManager);

            GameEventService.RegisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            GameEventService.RegisterListener<TTCEvents.CellDataSelected>(CellDataSelected);
            GameEventService.RegisterListener<TTCEvents.WinningSequence>(PositionAndTurnOnLights);
            return true;
        }

        public void UnInitialize()
        {
            GameEventService.UnregisterListener<TTCEvents.GameStateChanged>(OnGameStateChanged);
            GameEventService.UnregisterListener<TTCEvents.CellDataSelected>(CellDataSelected);
            GameEventService.UnregisterListener<TTCEvents.WinningSequence>(PositionAndTurnOnLights);
            
            Addressables.Release(_gridData);
            GameObject.Destroy(_gridParent);
        }
        
        public void OnUpdate()
        {
            // Ticked by GameManager every frame. Implement logic that requires constant update here
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

        private void OnGameStateChanged(TTCEvents.GameStateChanged gameStateChanged)
        {
            GameState fromState = gameStateChanged.FromGameState;
            GameState toState = gameStateChanged.ToGameState;
            
            if (fromState == GameState.MainMenu && toState == GameState.GameLoop)
            {
                InitializeGrid();
                CallSpawnActions();
            }

            if (fromState == GameState.Results && toState == GameState.MainMenu)
            {
                CallDeSpawnActions();
            }
        }

        private void CellDataSelected(TTCEvents.CellDataSelected cellDataSelected)
        {
            PlayerData playerData = default;
            if (_stateManager != null && !_stateManager.TryGetCurrentPlayerTurnData(out playerData))
            {
                 return;
            }

            if (!TryGetNextAvailableMarker(out Marker availableMarker))
            {
                availableMarker = Marker.Instantiate(_gridData.MarkerPrefab, _marksContainer);
                _markersPool.Add(availableMarker);
            }

            availableMarker.Initialize(playerData, cellDataSelected.SelectedCellData);
        }

        private bool TryGetNextAvailableMarker(out Marker availableMarker)
        {
            availableMarker = null;

            foreach (Marker marker in _markersPool)
            {
                if (!marker.IsMarked())
                {
                    availableMarker = marker;
                    break;
                }
            }

            return availableMarker != null;
        }

        private void InitializeGrid()
        {
            if (_gridInitialized)
            {
                return;
            }

            GenerateGridData();
            SpawnGridPrefabs();
            _gridInitialized = true;
        }

        // View Side of Grid
        private void SpawnGridPrefabs()
        {
            while (cellDataQueue.Count > 0)
            {
                CellData nextCellData = cellDataQueue.Dequeue();

                // we instantiate one per frame, in order to avoid halts on slower machines, and for general smoother flow.
                GameObject instance = GameObject.Instantiate(_gridData.CellPrefab, nextCellData.LocalPosition, Quaternion.identity, _cellsContainer);
                instance.transform.localScale *= _gridData.Offset;

                if (!instance.TryGetComponent(out Cell_Interact cellInteract))
                {
                    cellInteract = instance.AddComponent<Cell_Interact>();
                }

                cellInteract.Initialize(nextCellData);
                _cellInteracts.Add(cellInteract);
            }
        } 

        private void CallSpawnActions()
        {
            foreach (Cell_Interact cell_interact in _cellInteracts)
            {
                cell_interact.SpawnActions();
            }
        }

        private void CallDeSpawnActions()
        {
            foreach (Cell_Interact cell_interact in _cellInteracts)
            {
                cell_interact.DeSpawnActions();
            }

            foreach (Marker marker in _markersPool)
            {
                marker.MakeDormant();
            }

            ToggleLights(false);
        }

        private void OnGridDataLoaded(AsyncOperationHandle<GridData> obj)
        {
            if (obj.Status != AsyncOperationStatus.Succeeded) 
            {
                Debug.LogError($"Failed to instantiate ScriptableObject with address{_data.GridDataAddress}.");
                return;
            }

            _gridData = obj.Result;
            _gridParent = new GameObject(GRID_PARENT_NAME);
            _gridParent.transform.position = _gridData.TransformCoordinates;
            _cellsContainer = new GameObject(_gridData.CellsTransformName).transform;
            _marksContainer = new GameObject(_gridData.MarksTransformName).transform;
            _lightsContainer = new GameObject(_gridData.LightsTransformName).transform;

            _cellsContainer.transform.SetParent( _gridParent.transform, false);
            _marksContainer.transform.SetParent(_gridParent.transform, false);
            _lightsContainer.transform.SetParent(_gridParent.transform, false);

            for (int i = 0; i < _gridData.LightsToSpawn; i++)
            {
                Light newLight = Light.Instantiate(_gridData.SpotLightPrefab, _lightsContainer);
                _winningSequenceLights.Add(newLight);
            }
            ToggleLights(value: false);
        }

        // data side of Grid
        private void GenerateGridData()
        {
             int gridDimension = GameLogic.GRID_DIMENSION;

             Vector3 centerOffset = GetCenterOffset();

             Vector3 boxColliderCenter = _gridData.BoxColliderCenter;
             Vector3 boxColliderSize = _gridData.BoxColliderSize;

             for (int i = 0; i < gridDimension; i++)
             {
                 for (int j = 0; j < gridDimension; j++)
                 {
                     Vector3 position = new Vector3(i * _gridData.Offset, 0, j * _gridData.Offset) - centerOffset;
    
                     int row = i;
                     int column = j;
    
                     CellData cellData = new CellData
                     (
                         localPosition: position,
                         coords: new Vector2(row, column),
                         isMarked: false,
                         boxColliderCenter,
                         boxColliderSize
                     );
    
                     cellDataQueue.Enqueue(cellData);
                 }
             }
        }

        private Vector3 GetCenterOffset()
        {
            Vector3 centerOffset = new Vector3((GameLogic.GRID_DIMENSION - 1) * _gridData.Offset / 2, 0, (GameLogic.GRID_DIMENSION - 1) * _gridData.Offset / 2);
            return centerOffset;
        }

        private void PositionAndTurnOnLights(TTCEvents.WinningSequence winningSequence)
        {
            int lightsCount = _winningSequenceLights.Count;
            if (winningSequence.CoordsSequence.Count != lightsCount)
            {
                Debug.LogError($"Expected same Count for  lights: {lightsCount} and Coords: {winningSequence.CoordsSequence.Count}");
            }

            Vector3 centerOffset = GetCenterOffset();

            for (int i = 0; i < lightsCount; i++)
            {
                int currentIndex = i;
                Transform lightTransform = _winningSequenceLights[i].transform;

                int x = (int)winningSequence.CoordsSequence[i].x;
                int y = (int)winningSequence.CoordsSequence[i].y;

                Vector3 position = new Vector3(x: x * _gridData.Offset, 
                                               y: 0, 
                                               z: y * _gridData.Offset) - centerOffset;
                // keep the light's Altitude
                position.y = lightTransform.localPosition.y;
                lightTransform.localPosition = position;

                _timeManager?.AddCooldown(_gridData.LightInterval * currentIndex, TurnOnLight, _winningSequenceLights[currentIndex]);
            }
        }

        private void TurnOnLight(Light light)
        {
            light.enabled = true;
            _audio?.PlayLightOn();
        }

        private void TurnOffLight(Light light)
        {
            light.enabled = false;
        }

        private void ToggleLights(bool value)
        {
            foreach (Light light in _winningSequenceLights)
            {
                TurnOffLight(light);
            }
        }
    }
}
