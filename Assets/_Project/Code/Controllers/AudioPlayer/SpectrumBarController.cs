using UnityEngine;
using UnityEngine.UI;

public class SpectrumBarController : MusicMateBehavior
{
    [SerializeField] int _band;

    Image _image;

    protected override void InitializeComponents() => _image = GetComponent<Image>();

    protected override void ApplyColors() => ChangeColor(MusicMateColor.Default, _image);

    void Update() => _image.fillAmount = AudioSpectrumController.m_AudioBandBuffer[_band];
}
