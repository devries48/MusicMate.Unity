using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.Input
{
    public class ListBox : MusicMateBehavior
    {
        [SerializeField] string labelText;

        [Header("Elements")] [SerializeField] TextMeshProUGUI label;
        [SerializeField] GameObject listPanel;
        [SerializeField] Transform listContent;
        [SerializeField] GameObject listItemPrefab;

        [Header("Toolbar")] [SerializeField] ButtonAnimator deleteButton;
        [SerializeField] ButtonAnimator downButton;
        [SerializeField] ButtonAnimator upButton;

        Image _panelImage;
        int _selectedIndex;

        #region Base Class Methods

        protected override void RegisterEventHandlers()
        {
            deleteButton.OnButtonClick.AddListener(OnItemDelete);
            downButton.OnButtonClick.AddListener(OnItemDown);
            upButton.OnButtonClick.AddListener(OnItemUp);
        }

        protected override void UnregisterEventHandlers()
        {
            deleteButton.OnButtonClick.RemoveListener(OnItemDelete);
            downButton.OnButtonClick.RemoveListener(OnItemDown);
            upButton.OnButtonClick.RemoveListener(OnItemUp);
        }

        protected override void InitializeComponents()
        {
            _panelImage = listPanel.GetComponent<Image>();
        }

        protected override void InitializeValues()
        {
            label.text = labelText;
            _selectedIndex = -1;

            ValidateToolbar();
        }

        protected override void ApplyColors()
        {
            ChangeColor(MusicMateColor.Background, _panelImage);
            ChangeColor(MusicMateColor.Text, label);
        }

        #endregion

        public void AddResult(SuggestionResult result)
        {
            var item = Instantiate(listItemPrefab, listContent);
            var listItem = item.GetComponent<ListItem>();
            listItem.Result = result;

            item.name = result.Name;
            item.GetComponentInChildren<TMP_Text>().text = result.Name;
            item.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(listItem));
        }

        public IEnumerable<SuggestionResult> GetResults()
        {
            return listContent.GetComponentsInChildren<ListItem>()
                .Select(listItem => listItem.Result).ToList();
        }

        void OnItemSelected(ListItem listItem)
        {
            if (listItem.IsSelected)
            {
                listItem.IsSelected = false;
                _selectedIndex = -1;
            }
            else
            {
                listItem.IsSelected = true;
                _selectedIndex = listItem.transform.GetSiblingIndex();

                for (var i = 0; i < listContent.childCount; i++)
                {
                    if (i == _selectedIndex) continue;
                    var item = listContent.GetChild(i).GetComponent<ListItem>();
                    item.IsSelected = false;
                }
            }

            ValidateToolbar();
        }

        void OnItemUp()
        {
            MoveItem(_selectedIndex, _selectedIndex--);
        }

        void OnItemDown()
        {
            MoveItem(_selectedIndex, _selectedIndex++);
        }

        void OnItemDelete()
        {
            if (_selectedIndex < 0 || _selectedIndex >= listContent.childCount) return;

            Destroy(listContent.GetChild(_selectedIndex).gameObject);

            if (listContent.childCount == 0)
                _selectedIndex = -1;
            else
            {
                if (_selectedIndex >= listContent.childCount)
                    _selectedIndex = listContent.childCount - 1;

                var item = listContent.GetChild(_selectedIndex).GetComponent<ListItem>();
                item.IsSelected = true;
            }

            ValidateToolbar();
        }

        void MoveItem(int from, int to)
        {
            var item = listContent.GetChild(from);
            item.SetSiblingIndex(to);
            _selectedIndex = to;

            ValidateToolbar();
        }

        void ValidateToolbar()
        {
            deleteButton.SetInteractable(_selectedIndex > -1);
            downButton.SetInteractable(_selectedIndex > -1 && _selectedIndex < listContent.childCount - 1);
            upButton.SetInteractable(_selectedIndex > 0);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            label.text = labelText;
        }
#endif
    }
}