using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Players
{
    public sealed class AvatarFactory : MonoBehaviour
    {
        [SerializeField] private Avatar[] _allAvatars;

        private readonly List<Avatar> _availableAvatars = new();

        private void Awake()
        {
            foreach (var avatar in _allAvatars)
                _availableAvatars.Add(avatar);
        }

        [CanBeNull]
        internal Avatar Request(Player player, string avatar)
        {
            var avatarId = Path.GetFileNameWithoutExtension(avatar);
            var template = _availableAvatars.FirstOrDefault(x => x.name == avatarId);

            if (template == null)
            {
                Debug.LogError($"Cannot find avatar with id {avatar}");
                return null;
            }

            var result = Instantiate(template);
            result.name = $"{player.Id} {player.Name}";

            return result;
        }

#if DEBUG
        public Avatar RequestRandom(Player player)
        {
            var template = _allAvatars.PickRandom();
            
            var result = Instantiate(template);
            result.name = $"{player.Id} {player.Name}";

            return result;
        }
#endif
    }
}