using UnityEngine;
using UnityEngine.UI;

public class SpectrumBarController : MonoBehaviour
{
    [SerializeField] int _band;

    Image _image;

    void Start() => _image = GetComponent<Image>();

    void Update() => _image.fillAmount = AudioSpectrumController.m_AudioBandBuffer[_band];
}
