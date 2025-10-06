using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class InputEnum : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] string labelText;
        [SerializeField] private EnumType enumType;

        [Header("Elements")] 
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] TextMeshProUGUI inputTextLabel;
        [SerializeField] Image backgroundImage;
        [SerializeField] Image arrowImage;
        [SerializeField] GameObject itemsPanel;
        [SerializeField] RectTransform itemsContent;
        [SerializeField] GameObject dropdownItemPrefab;

        enum EnumType
        {
            ReleaseStatus,
            ReleaseType
        }

        Image _panelImage;
        Type _activeEnumType;
        Array _enumValues;
        object _currentValue;
        bool _isOpen;

        #region Base Class Methods

        protected override void InitializeComponents()
        {
            _panelImage = itemsPanel.GetComponent<Image>();
        }

        protected override void InitializeValues()
        {
            label.text = labelText;
            itemsPanel.gameObject.SetActive(false);

            switch (enumType)
            {
                case EnumType.ReleaseStatus:
                    _activeEnumType = typeof(ReleaseStatus);
                    break;
                case EnumType.ReleaseType:
                    _activeEnumType = typeof(ReleaseType);
                    break;
                default:
                    Debug.LogError("Unsupported enum type selected!");
                    return;
            }

            _enumValues = Enum.GetValues(_activeEnumType);
            PopulateList();
        }

        protected override void ApplyColors()
        {
            ChangeColor(MusicMateColor.Background, _panelImage);
            ChangeColor(MusicMateColor.Text, label, inputTextLabel);
            ChangeColor(MusicMateColor.Text, arrowImage);
        }

        #endregion

        void PopulateList()
        {
            foreach (Transform child in itemsContent)
                Destroy(child.gameObject);

            foreach (var value in _enumValues)
            {
                var itemGo = Instantiate(dropdownItemPrefab, itemsContent);
                var text = itemGo.GetComponentInChildren<TMP_Text>();
                text.text = value.ToString();

                var button = itemGo.GetComponent<Button>();
                var capturedValue = value; // local copy for closure
                button.onClick.AddListener(() => OnSelectValue(capturedValue));
            }

            SetValueObject(_enumValues.GetValue(0));
        }

        void ToggleDropdown()
        {
            _isOpen = !_isOpen;
            itemsPanel.gameObject.SetActive(_isOpen);
            arrowImage.rectTransform.localEulerAngles = _isOpen ? new Vector3(0, 0, 180) : Vector3.zero;
        }

        void OnSelectValue(object value)
        {
            SetValueObject(value);
            ToggleDropdown();
        }

        void SetValueObject(object value)
        {
            _currentValue = value;
            inputTextLabel.text = value.ToString();
        }

        public void SetValue<T>(T value) where T : Enum
        {
            SetValueObject(value);
        }

        public T GetValue<T>() where T : Enum
        {
            return (T)_currentValue;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animations.Input.PlayListItemHighlight(inputTextLabel, backgroundImage,false, arrowImage);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animations.Input.PlayListItemNormal(inputTextLabel, backgroundImage,false, arrowImage);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleDropdown();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            label.text = labelText;
        }
#endif
    }
}