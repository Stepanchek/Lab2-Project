using System.Collections.Generic;
using UnityEngine;

namespace PartyQuiz.Utils.Inputs
{
    public sealed class InputDispatcher : MonoBehaviour
    {
        private static readonly List<IInputNode> _inputElements = new();
        private static readonly List<IInputNode> _unregisterQueue = new();

        private readonly List<ECommand> _commandsList = new(256);

        private InputPreset _currentPreset;

        internal static void Register(IInputNode inputReceiver)
        {
            _inputElements.Add(inputReceiver);
            _inputElements.Sort((a, b) => a.Order - b.Order);
        }

        internal static void Unregister(IInputNode inputReceiver)
        {
            _unregisterQueue.Add(inputReceiver);
        }

        private void Awake()
        {
            _currentPreset = new InputPreset(
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Escape, ECommand.Escape),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Return, ECommand.Enter),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.A, ECommand.Left),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.D, ECommand.Right),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.W, ECommand.Forward),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.S, ECommand.Back),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.E, ECommand.RotateCounterclockwise),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Q, ECommand.RotateClockwise),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Mouse0, ECommand.Mouse0),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Mouse1, ECommand.Mouse1),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Mouse2, ECommand.Mouse2),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.BackQuote, ECommand.ShowConsole),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha1, ECommand.Alpha1),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha2, ECommand.Alpha2),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha3, ECommand.Alpha3),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha4, ECommand.Alpha4),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha5, ECommand.Alpha5),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha6, ECommand.Alpha6),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha7, ECommand.Alpha7),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha8, ECommand.Alpha8),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha9, ECommand.Alpha9),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Alpha0, ECommand.Alpha0),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.F5, ECommand.QuickSave),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.F9, ECommand.QuickLoad),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.LeftControl, ECommand.LeftControl),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.Tab, ECommand.Tab),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.UpArrow, ECommand.UpArrow),
                new KeyValuePair<KeyCode, ECommand>(KeyCode.DownArrow, ECommand.DownArrow));
        }

        /// <summary>
        /// InputDispatcher script should be earlier than the others in Execution Order.
        /// </summary>
        private void Update()
        {
            _commandsList.Clear();

            foreach (var (keyCode, command) in _currentPreset.InputKeyCombinations)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    _commandsList.Add(command | ECommand.Press);
                    continue;
                }

                if (Input.GetKeyUp(keyCode))
                {
                    _commandsList.Add(command | ECommand.Release);
                    continue;
                }

                if (Input.GetKey(keyCode))
                    _commandsList.Add(command | ECommand.Hold);
            }

            foreach (var node in _unregisterQueue)
                _inputElements.Remove(node);

            _unregisterQueue.Clear();

            DispatchInput(_commandsList);
        }

        private void DispatchInput(List<ECommand> commandsList)
        {
            if (commandsList.Count == 0)
                return;

            foreach (var element in _inputElements)
                element.ReceiveInput(commandsList);
        }
    }
}