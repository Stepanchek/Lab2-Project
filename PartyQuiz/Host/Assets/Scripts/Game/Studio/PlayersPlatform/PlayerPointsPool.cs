using System;
using System.Collections.Generic;
using System.Linq;
using PartyQuiz.Gameplay.Players;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class PlayerPointsPool : MonoBehaviour
    {
        [SerializeField] private Transform[] _hostPoints;
        [SerializeField] private Transform[] _playerPoints;

        [SerializeField] private Transform _defaultPoint;

        private readonly List<Transform> _availableHostPoints = new();
        private readonly List<Transform> _availablePlayerPoints = new();

        internal Transform DefaultPoint => _defaultPoint;

        private void Awake()
        {
            foreach (var point in _playerPoints)
                _availablePlayerPoints.Add(point);

            foreach (var point in _hostPoints)
                _availableHostPoints.Add(point);
        }
        
        internal Transform Request(Player player)
        {
            Transform result = player.Role switch
            {
                ERole.Player => _availablePlayerPoints.FirstOrDefault(),
                ERole.Host => _availableHostPoints.FirstOrDefault(),
                ERole.NotSet => null,
                _ => throw new ArgumentOutOfRangeException()
            };

            _availablePlayerPoints.Remove(result);
            
            return result == null ? DefaultPoint : result;
        }

        internal void SetToDefault(Stand stand)
        {
            stand.SetPoint(DefaultPoint, false);
        }

        internal void ReturnToAvailable(Transform point, ERole role)
        {
            switch (role)
            {
                case ERole.Player:
                {
                    _availablePlayerPoints.Add(point);
                    break;
                }
                case ERole.Host:
                {
                    _availableHostPoints.Add(point);
                    break;
                }
                case ERole.NotSet:
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }
    }
}