namespace GMS
{
    [System.Serializable]
    [LinkDataLogic(typeof(VFXData), dataDisplayName: "VFXData", typeof(VFXManager), displayName: "VFXManager")]
    public struct VFXData : ISubManagerData
    {
        public string ParticlesPoolGameObjectName;
    }
}
