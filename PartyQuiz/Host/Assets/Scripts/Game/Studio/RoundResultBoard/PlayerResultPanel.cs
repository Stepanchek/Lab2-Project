using Cysharp.Threading.Tasks;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using Redcode.Awaiting;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class PlayerResultPanel : ObjectWithDisposableContainer
    {
        [SerializeField] private BoardTextCell _nameCell;
        [SerializeField] private BoardTextCell _scoreCell;

        internal Player Player { get; private set; }

        private void Awake()
        {
            _nameCell.Show(string.Empty);
            _scoreCell.Show(string.Empty);
        }
        
        internal async UniTask Roll(bool value, float time)
        {
            _nameCell.Roll(value, time);
            _scoreCell.Roll(value, time);

            await new WaitForSeconds(time);
        }
        
        internal void Show(Player player)
        {
            DC.Dispose();
            
            Player = player;
            
            OnNameChangedHandler(string.Empty, Player.Name);
            UpdateScore(Player.Score);

            Player.OnNameChanged += OnNameChangedHandler;

            ShowGameObject();
        }

        private void OnNameChangedHandler(string oldName, string newName)
        {
            _nameCell.Show(newName);
        }

        internal void UpdateScore(int value)
        {
            _scoreCell.Show(value.ToString());
        }

        public override void Dispose()
        {
            if (Player == null)
                return;
            
            _nameCell.Show(string.Empty);
            _scoreCell.Show(string.Empty);

            Player.OnNameChanged -= OnNameChangedHandler;
            Player = null;
            
            base.Dispose();
        }
    }
}