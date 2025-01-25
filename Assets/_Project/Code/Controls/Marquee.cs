using System.Collections;
using TMPro;
using UnityEngine;

public class Marquee : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _delay;
    [SerializeField] RectTransform _viewPort;
    [SerializeField] RectTransform _content;

    const float _defaultMargin = 2;
    const float _italicMargin = 4;

    public Color32 TextColor
    {
        get => ContentText.color;
        set => ContentText.color = value;
    }

    public TextMeshProUGUI ContentText
    {
        get
        {
            if (_contentText == null)
                _contentText = _content.GetComponent<TextMeshProUGUI>();

            return _contentText;
        }
    }

    TextMeshProUGUI _contentText;
    bool _isInitialized, _waiting = false;

    void Update()
    {
        if (_waiting)
            return;

        if (_content.rect.width > _viewPort.rect.width)
        {
            if (!_isInitialized && Mathf.Abs(_content.pivot.x) <= .01f)
            {
                _waiting = true;
                _isInitialized = true;
                StartCoroutine(Wait());
            }
            else
            {
                var x = _content.pivot.x + _speed / 5000f;

                if (x > 1f)
                {
                    _content.pivot = new Vector3(-1f, .5f);
                    _isInitialized = false;
                }
                else
                    _content.pivot = new Vector3(x, .5f);
            }
        }
    }

    public void SetText(string text)
    {
        ContentText.text = text;
        AdjustMarginsForItalic(); 
    }

    public void ClearText()
    {
        ContentText.text = "";
        ResetMargins(); 
    }

    /// <summary>
    /// Adjusts the margins dynamically if the text is italicized to prevent clipping.
    /// </summary>
     void AdjustMarginsForItalic()
    {
        if ((ContentText.fontStyle & FontStyles.Italic) != 0) // Check if italic is applied
            ContentText.margin = new Vector4(_italicMargin, 0, _italicMargin, 0); // Add margins for italic text
        else
            ResetMargins();
    }

    void ResetMargins()
    {
        ContentText.margin = new Vector4(_defaultMargin, 0, _defaultMargin, 0); 
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(_delay);
        _waiting = false;
    }
}