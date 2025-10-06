using DG.Tweening;
using log4net.DateFormatter;
using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorWindow : MusicMateBehavior
{
    [SerializeField] EditorPrefabConfig _editorPrefabConfig; // Reference to the ScriptableObject

    [Header("Elements")]
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] GameObject _content;
    [SerializeField] ButtonAnimator _acceptButton;
    [SerializeField] ButtonAnimator _cancelButton;
    public GameObject m_modalBackground;

    Image _panel;
    RectTransform _panelRect;
    RectTransform _contentRect;
    SizeF _initialPanelSize;
    SizeF _initialContentSize;

    GameObject _currentEditorInstance;
    object _currentModel;
    MusicMateZone _currentZone;

    public RectTransform PanelRect
    {
        get
        {
            InitPanel();
            return _panelRect;
        }
    }
    
    public event Action<MusicMateZone, object> OnEditorAccepted;

    #region Base Class Methods
    protected override void RegisterEventHandlers()
    {
        _cancelButton.OnButtonClick.AddListener(OnCancelClicked);
        _acceptButton.OnButtonClick.AddListener(OnAcceptClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        _cancelButton.OnButtonClick.RemoveListener(OnCancelClicked);
        _acceptButton.OnButtonClick.RemoveListener(OnAcceptClicked);
    }

    protected override void InitializeComponents() { InitPanel(); }
    
    protected override void ApplyColors() 
    { 
        ChangeColor(MusicMateColor.Accent, _title); 
        ChangeColor(MusicMateColor.Panel, _panel); 
    }
    #endregion

    void InitPanel()
    {
        _panel = GetComponent<Image>();
        _panelRect = GetComponent<RectTransform>();
        _contentRect = _content.GetComponent<RectTransform>();

        _initialPanelSize = new SizeF(_panelRect.rect.width, _panelRect.rect.height);
        _initialContentSize = new SizeF(_contentRect.rect.width, _contentRect.rect.height);
    }

    public void SetEditor(ZoneAnimator zone)
    {
        _currentZone = zone.m_editorZone;
        
        if (_currentEditorInstance)
        {
            DestroyImmediate(_currentEditorInstance);
            _currentEditorInstance = null;
        }

        var prefab = _editorPrefabConfig.GetPrefab(zone.m_editorZone);
        if (!prefab)
        {
            Debug.LogWarning($"No prefab assigned for {zone.m_editorZone} in EditorPrefabConfig.");
            return;
        }
        var prefabRect = prefab.GetComponent<RectTransform>();
        var prefabSize = new SizeF(prefabRect.rect.width, prefabRect.rect.height);
        
        _currentEditorInstance = Instantiate(prefab, _content.transform);
        _currentEditorInstance.transform.localScale = Vector3.one;

        var model = zone.m_parent.ReleaseModel;

        if (_currentEditorInstance.TryGetComponent(out IEditorComponent<ReleaseModel> editorComponent))
            editorComponent.SetModel(model);

        ResizeToFitContent(prefabSize);
    }

    void ResizeToFitContent(SizeF childSize)
    {
        if (_content.transform.childCount == 0)
            return;

        var childRect = _content.transform.GetChild(0).GetComponent<RectTransform>();
        if (!childRect)
            return;

        var width = childSize.Width > _initialContentSize.Width
            ? _initialPanelSize.Width + childSize.Width - _initialContentSize.Width
            : _initialPanelSize.Width;

        var height = childSize.Height > _initialContentSize.Height
            ? _initialPanelSize.Height + childSize.Height - _initialContentSize.Height
            : _initialPanelSize.Height;

        _panelRect.DOSizeDelta(new Vector2(width, height), 0.2f);
    }

    void OnAcceptClicked()
    {
        if (_currentEditorInstance == null)
            return;

        if (_currentEditorInstance.TryGetComponent(out IEditorComponent<object> editorComponent))
        {
            _currentModel = editorComponent.GetModel();
            OnEditorAccepted?.Invoke(_currentZone, _currentModel);
        }

        Manager.HideEditor();
    }

    void OnCancelClicked() { Manager.HideEditor(); }
}


