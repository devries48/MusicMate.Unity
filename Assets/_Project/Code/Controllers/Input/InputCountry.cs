using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class InputCountry : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] string labelText;
        [SerializeField] CountryFlags countryFlags;

        [Header("Elements")] 
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] TMP_InputField inputTextField;
        [SerializeField] GameObject itemsPanel;
        [SerializeField] Transform itemsContent;
        [SerializeField] GameObject dropdownItemPrefab;

        Image _panelImage;
        string _selectedCountryCode3;

        #region Base Class Methods
        protected override void RegisterEventHandlers()
        {
            inputTextField.onSelect.AddListener(OnSelect);
            inputTextField.onDeselect.AddListener(OnDeselect);
            inputTextField.onValueChanged.AddListener(OnSearchChanged);
        }

        protected override void UnregisterEventHandlers()
        {
            inputTextField.onSelect.RemoveListener(OnSelect);
            inputTextField.onDeselect.RemoveListener(OnDeselect);
            inputTextField.onValueChanged.RemoveListener(OnSearchChanged);
        }

        protected override void InitializeComponents()
        {
            _panelImage = itemsPanel.GetComponent<Image>();
        }

        protected override void InitializeValues()
        {
            label.text = labelText;
            Animations.Input.PlayTextNormal(inputTextField);
            itemsPanel.SetActive(false);
        }

        protected override void ApplyColors()
        {
            ChangeColor(MusicMateColor.Background, _panelImage);
            ChangeColor(MusicMateColor.Text, label);
        }
        #endregion

        void OnSearchChanged(string searchText)
        {
            ClearList();
        
            if (string.IsNullOrWhiteSpace(searchText))
            {
                itemsPanel.SetActive(false);
                return;
            }
        
            var filtered = CountryFlags.Countries
                .Where(c => c.Name.ToLower().Contains(searchText.ToLower()))
                .ToList();

            if (filtered.Count == 0)
            {
                itemsPanel.SetActive(false);
                return;
            }
        
            itemsPanel.SetActive(true);
        
            PopulateList(filtered);
        }

        void PopulateList(List<(string Code2, string Code3, string Name)> countries)
        {
            foreach (var country in countries)
            {
                var itemGo = Instantiate(dropdownItemPrefab, itemsContent);
                var item = itemGo.GetComponent<ListItemCountry>();

                item.Initialize(
                    country.Code3,
                    countryFlags.GetFlag(country.Code3),
                    country.Name,
                    OnCountrySelected
                );
            }

            //scrollRect.verticalNormalizedPosition = 1f; // reset scroll to top
        }

        void OnCountrySelected(string code3)
        {
            _selectedCountryCode3 = code3;
            inputTextField.text = CountryFlags.GetCountryName(code3);
            itemsPanel.SetActive(false);
            ClearList(); 
        }

        void ClearList()
        {
            foreach (Transform child in itemsContent)
                Destroy(child.gameObject);
        }

        public string GetCountryCode()
        {
            return _selectedCountryCode3;
        }
    
        public void SetCountryCode(string code)
        {
            OnCountrySelected(code);
        }

        // Animations 
        // ==========
        void OnSelect(string _)
        {
            Animations.Input.PlayTextSelect(inputTextField);
        }

        void OnDeselect(string _)
        {
            Animations.Input.PlayTextNormal(inputTextField);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animations.Input.PlayTextHighlight(inputTextField);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animations.Input.PlayTextNormal(inputTextField);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            label.text = labelText;
        }
#endif
    }
}