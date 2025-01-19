using System;
using UnityEngine;

public class AnimationManager : SceneSingleton<AnimationManager>, IAnimationManager
{
    [Header("Animations")]
    [SerializeField] LogoAnimations _logoAnimations;
    [SerializeField] PanelAnimations _panelAnimations;
    [SerializeField] ButtonAnimations _buttonAnimations;
    [SerializeField] GridAnimations _gridAnimations;
    [SerializeField] ToolbarAnimations _toolbarAnimations;
    [SerializeField] InputAnimations _inputAnimations;


    [Header("Tooltip Settings")]
    [SerializeField] float _tooltipDelay = .05f;
    [SerializeField] float _tooltipPadding = 10f;
    [SerializeField] float _tooltipPanelWidth = 150f;

    IMusicMateManager Manager
    {
        get
        {
            _manager ??= MusicMateManager.Instance;

            return _manager;
        }
    }

    IMusicMateManager _manager;

    void Awake()
    {
        _buttonAnimations.Initialize(Manager);
        _inputAnimations.Initialize(Manager);
        _toolbarAnimations.Initialize(Manager);
    }

    public float TooltipDelay { get => _tooltipDelay; }
    public float TooltipPadding { get => _tooltipPadding; }
    public float TooltipPanelWidth { get => _tooltipPanelWidth; }

    public IButtonAnimations Button => _buttonAnimations;
    public IGridAnimations Grid => _gridAnimations;
    public IInputAnimations Input => _inputAnimations;
    public IPanelAnimations Panel => _panelAnimations;
    public IToolbarAnimations Toolbar => _toolbarAnimations;

    public void PlayLogoHide(GameObject logo, Action onComplete = null) => _logoAnimations.PlayLogoFade(logo, () =>
        {
            logo.SetActive(false);
            onComplete?.Invoke();
        });
}