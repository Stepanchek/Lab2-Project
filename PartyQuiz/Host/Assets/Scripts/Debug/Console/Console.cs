using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using PartyQuiz.Utils;
using PartyQuiz.Gameplay;
using PartyQuiz.Gameplay.Players;
using PartyQuiz.Gameplay.Sequences;
using PartyQuiz.Utils.Inputs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;

namespace PartyQuiz.Debug
{
    internal sealed partial class Console : UIInputNode
    {
        private const int VISIBLE_LINES = 60;
        private const int MAX_UNITY_LOG_COUNT = 300;
        private const int LINES_SKIP_COUNT = 2;

        [SerializeField] private GameObject _informationHandler;

        [SerializeField] private Slider _consoleSlider;
        [SerializeField] private ConsoleLogColorsDictionary _colors;

        [SerializeField] private TMP_InputField _enterCommandField;
        [SerializeField] private TMPInputFieldNoScroll _logsPanel;

        private readonly List<ConsoleCommand> _commands = new();
        private readonly LinkedList<string> _logList = new();

        private int _linesSkipped;
        private bool _isConsoleActive;

        private readonly List<string> _inputs = new();

        private bool _initialized;
        private int _lastInputIndex;

        private int LinesSkipped
        {
            get => _linesSkipped;
            set
            {
                _linesSkipped = value;
                _consoleSlider.value = _linesSkipped;
            }
        }

        [Inject]
        private void Construct(GameController gameController, AvatarFactory avatarFactory,
            SequenceController sequenceController)
        {
            _gameController = gameController;
            _avatarFactory = avatarFactory;
            _sequenceController = sequenceController;

            InitConsole();
        }

        private void InitConsole()
        {
            ShowGameObject();
            SetVisible(false);

            if (_initialized)
                return;

            _initialized = true;

            Application.logMessageReceived += OnUnityLog;

            _enterCommandField.onEndEdit.AddListener(TryCommand);

            _consoleSlider.onValueChanged.AddListener(arg =>
            {
                LinesSkipped = (int)arg;
                RefreshParams();
            });

            AddCommands();
        }

        private void OnEnable()
        {
            LinesSkipped = 0;
            RefreshParams();
        }

        private void SetVisible(bool isActive)
        {
            _isConsoleActive = isActive;
            _informationHandler.SetActive(_isConsoleActive);
        }

        private void Show()
        {
            if (this == null || gameObject == null)
                return;

            if (_isConsoleActive)
                return;

            ShowGameObject();
            SetVisible(true);

            _logsPanel.Show(OnScroll);

            if (_enterCommandField.isFocused)
                return;

            _enterCommandField.ActivateInputField();
            _enterCommandField.Select();

            RefreshParams();
            OnEnable();
        }

        private void OnUnityLog(string condition, string stacktrace, LogType type)
        {
            if (condition.Length > MAX_UNITY_LOG_COUNT)
                condition = condition.Substring(0, MAX_UNITY_LOG_COUNT) + "...";

            var newString = $"[{type}] {condition}";

            foreach (var item in newString.Split('\n'))
            {
                switch (type)
                {
                    case LogType.Log:
                    {
                        AddLog(item, EConsoleLogType.Regular);
                        break;
                    }
                    case LogType.Warning:
                    {
                        AddLog(item, EConsoleLogType.Warning);
                        break;
                    }
                    case LogType.Error:
                    case LogType.Exception:
                    case LogType.Assert:
                    {
                        AddLog(item, EConsoleLogType.Error);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        private string TimeStamp()
        {
            return $"{DateTime.Now:HH:mm:ss}";
        }

        private void AddLog(string log, EConsoleLogType logType)
        {
            if (this == null || gameObject == null)
                return;

            var time = TimeStamp();
            var color = ColorUtility.ToHtmlStringRGBA(_colors[logType]);

            _logList.AddFirst($"<color=#{color}>{time} {log}</color>");

            _lastInputIndex = 0;

            if (LinesSkipped > 0)
                LinesSkipped += log.Split('\n').Length;

            if (isActiveAndEnabled)
                RefreshParams();
        }

        private void RefreshParams()
        {
            _linesSkipped = Mathf.Clamp(LinesSkipped, 0, Mathf.Max(0, _logList.Count));

            _logsPanel.text = string.Join("\n",
                _logList.Skip(LinesSkipped).Take(VISIBLE_LINES).ToArray());

            _consoleSlider.maxValue = _logList.Count;
        }

        private void TryCommand(string input)
        {
            input = input.Trim();

            if (string.IsNullOrEmpty(input))
                return;

            _enterCommandField.text = string.Empty;

            var success = false;

            foreach (var consoleCommand in _commands)
            {
                if (consoleCommand.TryExecute(input) == false)
                    continue;

                _inputs.Add(input);
                AddLog($"[Command] {input}: OK", EConsoleLogType.Success);

                success = true;
                break;
            }

            if (success == false && input != "`" && input != "ё" && input != "Ё")
            {
                _inputs.Add(input);
                AddLog($"[Command] {input}: not found", EConsoleLogType.Warning);
            }

            _enterCommandField.ActivateInputField();
            _enterCommandField.Select();
        }

        private void SetLastInput()
        {
            if (_inputs.Count <= 0)
                return;

            _lastInputIndex = Mathf.Clamp(_lastInputIndex, 1, _inputs.Count);

            _enterCommandField.text = _inputs[^_lastInputIndex];
            _enterCommandField.caretPosition = _enterCommandField.text.Length;
        }

        public override EInputDispatchResult ReceiveCommand(ECommand command)
        {
            if (command.IsCommandDown(ECommand.Escape) && _isConsoleActive)
            {
                Hide();
                return EInputDispatchResult.BlockAll;
            }

            if (command.IsCommandDown(ECommand.ShowConsole))
            {
                if (_isConsoleActive)
                    Hide();
                else
                    Show();

                return EInputDispatchResult.BlockAll;
            }

            if (_isConsoleActive && _inputs.Count > 0)
            {
                if (command.IsCommandDown(ECommand.UpArrow))
                {
                    _lastInputIndex--;
                    SetLastInput();
                }
                else if (command.IsCommandDown(ECommand.DownArrow))
                {
                    _lastInputIndex++;
                    SetLastInput();
                }

                return EInputDispatchResult.BlockAll;
            }

            return _isConsoleActive ? EInputDispatchResult.BlockAll : EInputDispatchResult.Ignore;
        }

        private void OnScroll(PointerEventData eventData)
        {
            LinesSkipped += eventData.scrollDelta.y > 0 ? -LINES_SKIP_COUNT : LINES_SKIP_COUNT;

            _consoleSlider.value = LinesSkipped;
            RefreshParams();
        }

        private void Hide()
        {
            SetVisible(false);
        }

        private void OnDestroy()
        {
            Dispose();

            Application.logMessageReceived -= OnUnityLog;
            _initialized = false;
        }
    }
}