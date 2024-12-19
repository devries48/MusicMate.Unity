using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlaylistController : MonoBehaviour
{
    [SerializeField] GameObject _prefabPlaylistItemTemplate;
    [SerializeField] VerticalLayoutGroup _verticalLayout;

    IAudioPlayerService _playerService;
    IMusicMateManager _manager;
    Transform _trans;
    
    void OnEnable() => _playerService.SubscribeToActionChanged(OnActionChanged);

    void OnDisable() => _playerService.UnsubscribeFromActionChanged(OnActionChanged);

    void Awake()
    {
        _playerService = AudioPlayerService.Instance;
        _manager = MusicMateManager.Instance;
        _trans = _verticalLayout.transform;
    }

    void Start() => DestroyItems();

    void OnActionChanged(object sender, ActionChangedEventArgs e)
    {
        if (e.Action == PlayerAction.PlaylistChanged)
        {
            if (e.PlaylistAction == PlaylistAction.NewList)
                StartCoroutine(SetPlaylist());
        
            if (e.PlaylistAction==PlaylistAction.None)
                SelectItem();
        }
    }

    void SelectItem()
    {
        var item = _trans.Find($"{_playerService.CurrentIndex}_item") ;
        var ctrl = item.GetComponent<PlaylistItemController>();
        
        ctrl.SetColor(_manager.AccentColor);
    }

    void DestroyItems()
    {
        for (int i = _trans.childCount - 1; i >= 0; i--)
            DestroyImmediate(_trans.GetChild(i).gameObject);
    }

    IEnumerator SetPlaylist()
    {
        DestroyItems();
        yield return null;

        var list = _playerService.GetPlaylist();
        for (int i = 0; i < list.Count; i++)
        {
            var track = list[i];
            var item = Instantiate(_prefabPlaylistItemTemplate, _verticalLayout.transform);
            var controller = item.GetComponent<PlaylistItemController>();
            controller.Initialize(track, i + 1);

            item.name = $"{i}_item";
            item.SetActive(true);

            yield return null;
        }
        SelectItem();
    }


}
