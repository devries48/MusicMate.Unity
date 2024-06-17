using System.Collections;
using TMPro;
using UnityEngine;

public class Marquee : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _delay;
    [SerializeField] RectTransform _viewPort;
    [SerializeField] RectTransform _content;

    TextMeshProUGUI _contentText;
    bool _isInitialized, _waiting = false;

     void Awake() => _contentText =_content.GetComponent<TextMeshProUGUI>();

    void Update()
    {
        if (_waiting) return;

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

    public void SetColor(Color32 color) => _contentText.color = color;

    public void SetText(string text) => _contentText.text = text;

    public void ClearText() => _contentText.text = "";

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(_delay);
        _waiting = false;
    }

 }