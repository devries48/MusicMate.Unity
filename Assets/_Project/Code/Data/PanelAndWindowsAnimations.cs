using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "MusicMate/Panel & Window Animation Settings", fileName = "Panel & Window Animations")]
public class PanelAndWindowsAnimations : ScriptableObject
{
    [Header("General Animation Settings")]
    [SerializeField] float _showAndHideDuration = 0.5f;
    [SerializeField, Tooltip("A delay before hiding the window")]  float _hideDelay = .1f;
    [SerializeField] Ease _showEase = Ease.OutBack;
    [SerializeField] Ease _hideEase = Ease.InBack;

    [Header("Login Window Visibility Animations")]
    [SerializeField,Tooltip("Pivot Y position to hide Login Window")]  float _loginHidePivot = -2f;
    [SerializeField, Tooltip("Pivot Y position to show Login Window")]  float _loginShowPivot = .5f;

    [Header("Error Window Visibility Animations")]
    [SerializeField, Tooltip("Pivot Y position to hide Error Window")]  float _errorHidePivot = 3f;
    [SerializeField, Tooltip("Pivot Y position to show Error Window")]  float _errorShowPivot = .5f;

    public void PlayShowLoginWindow(GameObject loginWindow, float delay) => MoveVertical(true, loginWindow, _loginHidePivot, _loginShowPivot, delay);

    public void PlayHideLoginWindow(GameObject loginWindow) => MoveVertical(false, loginWindow, _loginHidePivot, _loginShowPivot, _hideDelay);

    public void PlayShowErrorWindow(GameObject errorWindow) => MoveVertical(true, errorWindow, _errorHidePivot, _errorShowPivot);

    public void PlayHideErrorWindow(GameObject errorWindow) => MoveVertical(false, errorWindow, _errorHidePivot, _errorShowPivot, _hideDelay);

    void MoveVertical(bool show, GameObject obj, float hidePivot, float showPivot, float delay = 0f)
    {
        if (show)
            obj.SetActive(true);

        var pivotTo = show ? showPivot : hidePivot;
        var easing = show ? _showEase : _hideEase;
        var rect = obj.GetComponent<RectTransform>();

        rect.DOPivotY(pivotTo, _showAndHideDuration).SetEase(easing)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                if (!show)
                    obj.SetActive(false);
            });
    }

}