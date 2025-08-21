namespace GMS.Samples
{
    [System.Serializable]
    [LinkDataLogic(typeof(TicTacToeCameraData), dataDisplayName: "TicTacToe CameraData", typeof(TicTacToeCameraManager), displayName: "TicTacToeCameraManager")]
    public struct TicTacToeCameraData : ISubManagerData
    {
        [AddressableSelector]
        public string CameraContainerPrefabAddress;
    }
}
