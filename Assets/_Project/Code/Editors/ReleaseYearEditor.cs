using Controllers.Input;
using UnityEngine;

public class ReleaseYearEditor : MonoBehaviour, IEditorComponent<ReleaseModel>
{
    [SerializeField] InputText year;
    [SerializeField] InputCountry country;
  
    ReleaseModel _releaseModel;

    public void SetModel(ReleaseModel model)
    {
        _releaseModel = model;

        year.ValueText = model.ReleaseYear.ToString();
        country.SetCountryCode(model.Country);
    }

    public ReleaseModel GetModel()
    {
        int.TryParse(year.ValueText, out var yearNumber);
        _releaseModel.ReleaseYear = yearNumber;
        _releaseModel.Country=country.GetCountryCode();
        
        return _releaseModel;
    }
}