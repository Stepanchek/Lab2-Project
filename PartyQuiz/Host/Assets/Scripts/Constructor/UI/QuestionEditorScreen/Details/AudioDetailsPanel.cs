using System;
using System.IO;
using JetBrains.Annotations;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;
using File = UnityEngine.Windows.File;

namespace PartyQuiz.Constructor
{
    internal sealed class AudioDetailsPanel : UIElement
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Image _icon;

        [SerializeField] private Button _audioButton;
        [SerializeField] private Button _deleteButton;

        private Action<Media<AudioClip>> _onAudioSet;
        private AudioClip _audioClip;

        internal void Show([CanBeNull] Media<AudioClip> clip, Action<Media<AudioClip>> onAudioSet)
        {
            _onAudioSet = onAudioSet;

            _audioButton.onClick.AddListener(OnAudioButtonPressedHandler);
            DC.AddDisposable(() => _audioButton.onClick.RemoveListener(OnAudioButtonPressedHandler));

            _deleteButton.onClick.AddListener(OnDeleteButtonPressedHandler);
            DC.AddDisposable(() => _deleteButton.onClick.RemoveListener(OnDeleteButtonPressedHandler));

            if (clip != null && clip.Source != null)
            {
                _label.text = clip.Source.name;
                _audioClip = clip.Source;
            }
            else
            {
                _label.text = string.Empty;
                _audioClip = null;
            }

            ValidateIcons();
            ShowGameObject();
        }

        private void OnAudioButtonPressedHandler()
        {
            var result = LoadAudio();
            SetAudio(result);
        }

        private void OnDeleteButtonPressedHandler()
        {
            SetAudio(new Media<AudioClip>(null, string.Empty));
        }

        [CanBeNull]
        private Media<AudioClip> LoadAudio()
        {
            var extensions = new ExtensionFilter(string.Empty, "mp3", "wav", "ogg");

            var paths = StandaloneFileBrowser
                .OpenFilePanel("Open audio", string.Empty, new[] { extensions }, false);

            if (paths.Length == 0)
                return null;

            var path = paths[0];

            var extension = Path.GetExtension(path);
            var clipBytes = File.ReadAllBytes(path);

            var clip = extension switch
            {
                ".ogg" => NAudioPlayer.FromOggData(clipBytes),
                ".mp3" => NAudioPlayer.FromMp3Data(clipBytes),
                ".wav" => NAudioPlayer.FromWavData(clipBytes),
                _ => null
            };

            Debug.Assert(clip != null, nameof(clip) + " != null");

            clip.name = Path.GetFileName(path);

            var tempPath = ConstructorUtils.GetTempPath(path);

            return new Media<AudioClip>(clip, tempPath);
        }

        private void SetAudio([CanBeNull] Media<AudioClip> clip)
        {
            var source = clip?.Source;

            _audioClip = source;
            _label.text = source == null ? string.Empty : source.name;

            _onAudioSet?.Invoke(clip);

            ValidateIcons();
        }

        private void ValidateIcons()
        {
            _label.gameObject.SetActive(_audioClip != null);
            _deleteButton.gameObject.SetActive(_audioClip != null);
            _icon.gameObject.SetActive(_audioClip == null);
        }
    }
}