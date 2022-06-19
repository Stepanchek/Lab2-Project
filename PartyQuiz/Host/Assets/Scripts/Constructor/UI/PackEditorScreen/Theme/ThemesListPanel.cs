using PartyQuiz.Structure.Runtime;
using PartyQuiz.Utils;
using TMPro;
using UnityEngine;

namespace PartyQuiz.Constructor
{
    internal sealed class ThemesListPanel : UIElement
    {
        [SerializeField] private TextMeshProUGUI _roundNameLabel;
        
        [SerializeField] private ThemeView _themeViewTemplate;
        [SerializeField] private RectTransform _container;
        
        [SerializeField] private NewListView _newThemeListView;
        [SerializeField] private MessageWindow _confirmationWindow;
        
        private Round _round;

        internal void Show(Round round, int index)
        {
            _round = round;
            
            DC.Dispose();
            
            var viewList = new BindableViewList<Theme, ThemeView>(
                _round.Themes,
                _themeViewTemplate,
                _container,
                (item, view) =>
                {
                    view.Show(item, OnDeleteButtonPressedHandler);
                    
                    _newThemeListView.SetAsLast();
                    ValidateAddQuestionButton();
                });

            DC.AddDisposable(viewList);
            
            _round.Themes.OnItemRemoved += OnItemRemovedHandler;
            DC.AddDisposable(() => _round.Themes.OnItemRemoved -= OnItemRemovedHandler);

            _newThemeListView.Show(OnNewThemeCreatedHandler);
            _newThemeListView.SetAsLast();

            _roundNameLabel.text = $"Round {index + 1}";

            ValidateAddQuestionButton();
            ShowGameObject();
        }

        private void OnDeleteButtonPressedHandler(Theme theme)
        {
            _confirmationWindow.Show(null, () => _round.Themes.Remove(theme));
        }

        private void AddTheme(Theme theme)
        {
            _round.Themes.Add(theme);
        }

        private void OnNewThemeCreatedHandler()
        {
            AddTheme(new Theme("New Theme"));
        }

        private void OnItemRemovedHandler(Theme theme)
        {
            ValidateAddQuestionButton();
        }

        private void ValidateAddQuestionButton()
        {
            if (_round.Themes.Count >= Serialization.Version3.ThemeV3.MAX_QUESTIONS_COUNT)
                _newThemeListView.HideGameObject();
            else
                _newThemeListView.ShowGameObject();
        }
    }
}