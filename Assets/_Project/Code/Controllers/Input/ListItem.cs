using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class ListItem : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Elements")] 
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] Image backgroundImage;

        bool _isSelected;
        
        public SuggestionResult Result { get; set; } 
        
        public bool IsSelected  
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                ApplyColors();
            } 
        }
        
        protected override void ApplyColors()
        {
            var backgroundColor = IsSelected ? MusicMateColor.Accent : MusicMateColor.Background;
            var foregroundColor = IsSelected ? MusicMateColor.AccentText : MusicMateColor.Text;
            
            ChangeColor(backgroundColor, backgroundImage);
            ChangeColor(foregroundColor, label);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animations.Input.PlayListItemHighlight(label,backgroundImage,IsSelected);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animations.Input.PlayListItemNormal(label,backgroundImage,IsSelected);
        }
    }
}
