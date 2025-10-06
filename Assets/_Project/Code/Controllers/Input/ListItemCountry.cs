using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class ListItemCountry : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Elements")] 
        [SerializeField] Image flagImage;
        [SerializeField] TextMeshProUGUI countryNamelabel;
        [SerializeField] Image backgroundImage;

        string _countryCode;
    
        protected override void ApplyColors()
        {
            ChangeColor(MusicMateColor.Background, backgroundImage);
            ChangeColor(MusicMateColor.Text, countryNamelabel);
        }
    
        public void Initialize(string code3, Sprite flag, string country, System.Action<string> onSelected)
        {
            _countryCode = code3;
            flagImage.sprite = flag;
            countryNamelabel.text = country;

            GetComponent<Button>().onClick.RemoveAllListeners();
            GetComponent<Button>().onClick.AddListener(() => onSelected?.Invoke(_countryCode));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animations.Input.PlayListItemHighlight(countryNamelabel,backgroundImage);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animations.Input.PlayListItemNormal(countryNamelabel,backgroundImage);
        }
    }
}
