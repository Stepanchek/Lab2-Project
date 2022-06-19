using System;
using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PartyQuiz.Constructor
{
    internal sealed class PackView : UIElement
    {
        [SerializeField] private TextMeshProUGUI _packNameLabel;
        [SerializeField] private Button _exportButton;

        [SerializeField] private Button _deleteButton;
        
        internal void Show(Pack pack, Action<Pack> onDeleteButton)
        {
            var onNameChanged = pack.Name.SubscribeToText(_packNameLabel);
            DC.AddDisposable(onNameChanged);

            var onExport = _exportButton.OnClickAsObservable().SubscribeRx(_ => pack.Export());
            DC.AddDisposable(onExport);

            var onDelete = _deleteButton.OnClickAsObservable().Subscribe(_ => onDeleteButton(pack));
            DC.AddDisposable(onDelete);
            
            ShowGameObject();
        }
    }
}