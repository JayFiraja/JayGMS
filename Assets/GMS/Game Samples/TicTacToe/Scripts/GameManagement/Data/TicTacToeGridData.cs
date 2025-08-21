using GMS;
namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeGridData), dataDisplayName: "TicTacToe GridData", typeof(TicTacToeGridManager), displayName: "TicTacToeGridManager")]
    public struct TicTacToeGridData : ISubManagerData
    {
        [AddressableSelector]
        public string GridDataAddress;
    }
}
