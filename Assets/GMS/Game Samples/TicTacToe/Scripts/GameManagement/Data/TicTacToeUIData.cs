namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeUIData), dataDisplayName: "TicTacToe UIData", typeof(TicTacToeUIManager), displayName: "TicTacToeUIManager")]
    public struct TicTacToeUIData : ISubManagerData
    {
        [AddressableSelector]
        public string MainCanvasPrefabAddress;
    }
}
