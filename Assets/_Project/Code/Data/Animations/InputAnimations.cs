using DG.Tweening;
using Interfaces.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Data.Animations
{
    [CreateAssetMenu(menuName = "MusicMate/Animations/Input Animations", fileName = "Input Animations")]
    public class InputAnimations : ScriptableObject, IInputAnimations
    {
        [FormerlySerializedAs("_textTransitionDuration")] [Header("InputText")] [SerializeField]
        float textTransitionDuration = .2f;

        IMusicMateManager _manager;

        public void Initialize(IMusicMateManager manager) => _manager = manager;

        public void PlayTextNormal(TMP_InputField input)
        {
            SetColor(input, _manager.AppColors.TextColor);
            SetBackgroundColor(input, _manager.AppColors.BackgroundColor);
        }

        public void PlayTextSelect(TMP_InputField input)
        {
            SetColor(input, _manager.AppColors.AccentColor);
        }

        public void PlayTextHighlight(TMP_InputField input)
        {
            SetColor(input, _manager.AppColors.AccentColor);
            SetBackgroundColor(input, _manager.AppColors.DefaultColor);
        }

        public void PlayListItemNormal(TextMeshProUGUI label, Image background, bool isSelected = false, Image arrow = null)
        {
            var backgroundColor = isSelected ? _manager.AppColors.AccentColor : _manager.AppColors.BackgroundColor;
            var foregroundColor = isSelected ? _manager.AppColors.AccentTextColor : _manager.AppColors.TextColor;

            SetColor(label, foregroundColor);
            SetColor(background, backgroundColor);
            if (arrow)
                SetColor(arrow, foregroundColor);
        }

        public void PlayListItemHighlight(TextMeshProUGUI label, Image background, bool isSelected = false,
            Image arrow = null)
        {
            var backgroundColor =  _manager.AppColors.DefaultColor;
            var foregroundColor = isSelected ? _manager.AppColors.BackgroundColor :_manager.AppColors.AccentColor;

            SetColor(label, foregroundColor);
            SetColor(background, backgroundColor);
            if (arrow)
                SetColor(arrow, foregroundColor);
        }

        void SetColor(TMP_InputField input, Color targetColor) =>
            input.textComponent.DOColor(targetColor, textTransitionDuration);

        void SetColor(TextMeshProUGUI label, Color targetColor) => label.DOColor(targetColor, textTransitionDuration);
        void SetColor(Image image, Color targetColor) => image.DOColor(targetColor, textTransitionDuration);

        void SetBackgroundColor(TMP_InputField input, Color targetColor) =>
            input.image.DOColor(targetColor, textTransitionDuration);
    }
}