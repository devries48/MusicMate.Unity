using Controllers.Input;
using UnityEngine;

namespace Editors
{
    public class ReleaseTitleEditor : MonoBehaviour, IEditorComponent<ReleaseModel>
    {
        [SerializeField] InputSuggestion artist;
        [SerializeField] InputText title;
        [SerializeField] InputText subTitle;
        [SerializeField] InputEnum releaseType;
  
        ReleaseModel _releaseModel;

        public void SetModel(ReleaseModel model)
        {
            _releaseModel = model;

            artist.SetValue( model.Artist.Id,model.Artist.Text);
            title.ValueText = model.Title;
            subTitle.ValueText = model.SubTitle;
        
            if (model.ReleaseType != null) 
                releaseType.SetValue((ReleaseType)model.ReleaseType);
            else
                releaseType.SetValue(ReleaseType.Album);
        }

        public ReleaseModel GetModel()
        {
            var result = artist.GetValue;
            _releaseModel.Artist.Id =result.Id ;
            _releaseModel.Artist.Text=result.Name;

            _releaseModel.Title=title.ValueText  ;
            _releaseModel.SubTitle=subTitle.ValueText  ;

            _releaseModel.ReleaseType=releaseType.GetValue<ReleaseType>();
            
            return _releaseModel;
        }
    }
}