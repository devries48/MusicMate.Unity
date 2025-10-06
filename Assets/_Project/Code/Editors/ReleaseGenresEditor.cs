using System.Linq;
using Controllers.Input;
using UnityEngine;

namespace Editors
{
    public class ReleaseGenresEditor : MusicMateBehavior, IEditorComponent<ReleaseModel>
    {
        [SerializeField] InputSuggestion selectGenre;
        [SerializeField] ListBox mainGenreListbox;
        [SerializeField] ListBox subGenreListbox;
        [SerializeField] ButtonAnimator addToMainButton;
        [SerializeField] ButtonAnimator addToSubButton;

        ReleaseModel _releaseModel;

        #region Base Class Methods

        protected override void RegisterEventHandlers()
        {
            addToMainButton.OnButtonClick.AddListener(OnAddMainGenre);
            addToSubButton.OnButtonClick.AddListener(OnAddSubGenre);
            selectGenre.valueTextChanged.AddListener(OnValueChanged);
        }

        protected override void UnregisterEventHandlers()
        {
            addToMainButton.OnButtonClick.RemoveListener(OnAddMainGenre);
            addToSubButton.OnButtonClick.RemoveListener(OnAddSubGenre);
            selectGenre.valueTextChanged.RemoveListener(OnValueChanged);
        }

        protected override void InitializeValues()
        {
            OnValueChanged();
        }

        #endregion

        public void SetModel(ReleaseModel model)
        {
            _releaseModel = model;

            foreach (var genre in model.Genres)
            {
                var result = new SuggestionResult { Id = genre.Id, Name = genre.Text };
                if (genre.IsMainGenre)
                    mainGenreListbox.AddResult(result);
                else
                    subGenreListbox.AddResult(result);
            }
        }

        public ReleaseModel GetModel()
        {
            var genres = mainGenreListbox.GetResults()
                .Select(result => new GenreResult{ IsMainGenre = true, Id = result.Id, Text = result.Name }).ToList();
            genres.AddRange(mainGenreListbox.GetResults()
                .Select(result => new GenreResult { Id = result.Id, Text = result.Name }));
          
            _releaseModel.Genres = genres;
            
            return _releaseModel;
        }

        void OnValueChanged()
        {
            addToMainButton.SetInteractable(selectGenre.HasValue);
            addToSubButton.SetInteractable(selectGenre.HasValue);
        }

        void OnAddSubGenre()
        {
            subGenreListbox.AddResult(selectGenre.GetValue);
            selectGenre.ClearValue();
        }

        void OnAddMainGenre()
        {
            mainGenreListbox.AddResult(selectGenre.GetValue);
            selectGenre.ClearValue();
        }
    }
}