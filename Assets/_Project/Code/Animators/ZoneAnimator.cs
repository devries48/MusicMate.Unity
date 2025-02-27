using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoneAnimator : MusicMateBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI m_zoneLabel;
    public Image m_backgroundImage;
    public Image m_zoneImage;

    [SerializeField] string _text;

    protected override void InitializeValues()
    {
        if (m_zoneLabel != null)
            m_zoneLabel.text = _text;
    }

    protected override void ApplyColors()
    {
        ChangeColor(MusicMateColor.Accent, m_backgroundImage, m_zoneImage);
        ChangeColor(MusicMateColor.Accent, m_zoneLabel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animations.Panel.PlayZoneOn(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animations.Panel.PlayZoneOff(this);
    }

    #region Editor-Specific Code

#if UNITY_EDITOR

    // To suppress message: "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate”  
    void OnValidate() { EditorApplication.delayCall += OnValidateDelayed; }

    void OnValidateDelayed()
    {
        if (Application.isPlaying || EditorApplication.isCompiling)
            return;

        InitializeValues();
    }
#endif

    #endregion
}
