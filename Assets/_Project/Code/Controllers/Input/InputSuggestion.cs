using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class InputSuggestion : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] string labelText;
        [SerializeField] LookupSuggestion lookupSuggestion = LookupSuggestion.Artists;

        [Header("Elements")] 
        [SerializeField] TextMeshProUGUI label;
        [SerializeField] TMP_InputField inputTextField;
        [SerializeField] GameObject itemsPanel;
        [SerializeField] Transform itemsContent;
        [SerializeField] GameObject dropdownItemPrefab;

        [Header("Debounce Settings")] [SerializeField]
        float debounceDelay = 0.3f;

        [HideInInspector] public UnityEvent valueTextChanged;

        Image _panelImage;
        Coroutine _debounceCoroutine;

        readonly List<GameObject> _activeSuggestions = new();

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

        string SelectedName { get; set; }
 
        Guid SelectedId { get; set; }

        public SuggestionResult GetValue => new(){Id = SelectedId , Name = SelectedName ?? inputTextField.text };
        
        public void SetValue(Guid id, string text)
        {
            SelectedId = id;
            SelectedName = text;  
            inputTextField.text = text;
        }
    
        public bool HasValue => !string.IsNullOrWhiteSpace(inputTextField.text);

        public void ClearValue()
        {
            SelectedId = Guid.Empty;
            SelectedName = string.Empty;
            inputTextField.text = string.Empty;
        }

        public void SetFocus()
        {
            inputTextField.ActivateInputField();
        }

        void OnSearchChanged(string text)
        {
            if (text == SelectedName)
                return;

            if (_debounceCoroutine != null)
                StopCoroutine(_debounceCoroutine);

            _debounceCoroutine = StartCoroutine(DebounceFetch(text));
        }

        IEnumerator DebounceFetch(string currentText)
        {
            yield return new WaitForSeconds(debounceDelay);

            UpdateSuggestions(currentText);
        }

        void UpdateSuggestions(string text)
        {
            ClearSuggestions();

            if (string.IsNullOrWhiteSpace(text))
            {
                itemsPanel.SetActive(false);
                return;
            }

            ApiService.GetSuggestions(lookupSuggestion, text, callback => GetSuggestionsCallback(callback));
        }

        void GetSuggestionsCallback(IReadOnlyList<SuggestionResult> results)
        {
            if (results.Count == 0)
            {
                itemsPanel.SetActive(false);
                SelectedId = Guid.Empty;
                return;
            }

            itemsPanel.SetActive(true);

            foreach (var result in results)
            {
                var item = Instantiate(dropdownItemPrefab, itemsContent);
                item.GetComponentInChildren<TMP_Text>().text = result.Name;
                item.GetComponent<Button>().onClick.AddListener(() => OnSuggestionSelected(result));

                _activeSuggestions.Add(item);
            }
            valueTextChanged?.Invoke();
        }

        void OnSuggestionSelected(SuggestionResult suggestion)
        {
            SelectedName = suggestion.Name;
            inputTextField.text = suggestion.Name;
            SelectedId = suggestion.Id;
            itemsPanel.SetActive(false);
            ClearSuggestions();
        }

        void ClearSuggestions()
        {
            foreach (var item in _activeSuggestions)
                Destroy(item);

            _activeSuggestions.Clear();
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