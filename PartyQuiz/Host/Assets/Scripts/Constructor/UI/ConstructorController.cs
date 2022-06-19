using PartyQuiz.Structure.Runtime;
using UnityEngine;
using Zenject;

namespace PartyQuiz.Constructor
{
    internal sealed class ConstructorController : MonoBehaviour, IInitializable
    {
        [SerializeField] private PackListScreen _packListScreen;
        [SerializeField] private PackEditorScreen _packEditorScreen;

        public void Initialize()
        {
            _packListScreen.Show(OnNewPackCreatedHandler);
            _packEditorScreen.HideGameObject();
        }

        internal void Open(Pack pack)
        {
            _packListScreen.HideGameObject();
            _packEditorScreen.Show(pack);
        }

        private void OnNewPackCreatedHandler(Pack pack)
        {
            Open(pack);
        }
    }
}