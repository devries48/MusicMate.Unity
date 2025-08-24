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
        if (_panel != null)
            return;

        _panel = GetComponent<Image>();
        _panelRect = GetComponent<RectTransform>();
        _contentRect = _content.GetComponent<RectTransform>();

        _initialPanelSize = new SizeF(MathF.Abs(_panelRect.sizeDelta.x), MathF.Abs(_panelRect.sizeDelta.y));
        _initialContentSize = new SizeF(MathF.Abs(_contentRect.sizeDelta.x), MathF.Abs(_contentRect.sizeDelta.y));
    }

    public void SetEditor(ZoneAnimator zone)
    {
        _currentZone = zone.m_editorZone;

        InitPanel();

        if (_currentEditorInstance != null)
        {
            DestroyImmediate(_currentEditorInstance);
            _currentEditorInstance = null;
        }

        var prefab = _editorPrefabConfig.GetPrefab(zone.m_editorZone);
        if (prefab == null)
        {
            Debug.LogWarning($"No prefab assigned for {zone.m_editorZone} in EditorPrefabConfig.");
            return;
        }
        var prefabRect = prefab.GetComponent<RectTransform>();
        var prefabSize = new SizeF(MathF.Abs(prefabRect.sizeDelta.x), MathF.Abs(prefabRect.sizeDelta.y));
        
        _currentEditorInstance = Instantiate(prefab, _content.transform);
        _currentEditorInstance.transform.localScale = Vector3.one;

        var model = zone.m_parent.ReleaseModel;

        if (_currentEditorInstance.TryGetComponent(out IEditorComponent<ReleaseModel> editorComponent))
            editorComponent.SetModel((ReleaseModel)model);

        ResizeToFitContent(prefabSize);
    }

    void ResizeToFitContent(SizeF childSize)
    {
        if (_content.transform.childCount == 0)
            return;

        var childRect = _content.transform.GetChild(0).GetComponent<RectTransform>();
        if (childRect == null)
            return;

        float width = childSize.Width > _initialContentSize.Width
            ? _initialPanelSize.Width + childSize.Width - _initialContentSize.Width
            : _initialPanelSize.Width;

        float height = childSize.Height > _initialContentSize.Height
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


