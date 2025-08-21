using UnityEngine;

namespace GMS.Samples
{
    [RequireComponent(typeof(Renderer))]
    public class PlayerTurnReaderMaterialColorSet : MonoBehaviour
    {
        [SerializeField]
        private string MaterialColorName = "_Color";

        private Renderer _renderer;
        private TicTacToeStateData _gameManager;
        private MaterialPropertyBlock _propertyBlock;

        private void Start()
        {
            TryGetComponent(out _renderer);
            _propertyBlock = new MaterialPropertyBlock();
            GameEventService.RegisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);
        }

        private void OnDestroy()
        {
            GameEventService.UnregisterListener<TTCEvents.PlayerTurnChanged>(OnPlayerTurnChanged);
        }

        private void OnPlayerTurnChanged(TTCEvents.PlayerTurnChanged playerTurnChanged)
        {
            UpdateMaterialColor();
        }

        private void UpdateMaterialColor()
        {
            // Use material property block to change this material instance color from _renderer
            _renderer.GetPropertyBlock(_propertyBlock);
            // _propertyBlock.SetColor(MaterialColorName, newColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
