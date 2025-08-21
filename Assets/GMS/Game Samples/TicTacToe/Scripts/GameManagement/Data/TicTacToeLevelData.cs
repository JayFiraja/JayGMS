namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeLevelData), dataDisplayName: "TicTacToe LevelData", typeof(TicTacToeLevelManager), displayName: "TicTacToeLevelManager")]
    public struct TicTacToeLevelData : ISubManagerData
    {
        [AddressableSelector]
        public string LevelRenderingPrefabAddress;
    }
}
