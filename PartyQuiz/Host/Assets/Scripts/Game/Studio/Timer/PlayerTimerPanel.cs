using PartyQuiz.Gameplay.Players;
using PartyQuiz.Utils;
using UnityEngine;

namespace PartyQuiz.Gameplay.Studio
{
    internal sealed class PlayerTimerPanel : TimerPanel
    {
        internal void SetAnsweringPlayer(Player player)
        {
            var timerTransform = transform;
            
            timerTransform.SetParentAndReset(player.Avatar.transform);
            timerTransform.localPosition = new Vector3(0, 4, 0);
            timerTransform.localRotation = Quaternion.Euler(0, -180, 0);
        }
        
        protected override void NotifyTimerEnded()
        {
            TimeLeftLabel.text = "ANSWER!";
            
            base.NotifyTimerEnded();
        }
    }
}