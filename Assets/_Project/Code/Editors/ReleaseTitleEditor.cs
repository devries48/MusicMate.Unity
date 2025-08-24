using UnityEngine;

public class ReleaseTitleEditor : MonoBehaviour, IEditorComponent<ReleaseModel>
{
    private ReleaseModel _releaseModel;

    public void SetModel(ReleaseModel model)
    {
        _releaseModel = model;
        // Populate UI fields with model data
    }

    public ReleaseModel GetModel()
    {
        // Retrieve and return the updated model
        return _releaseModel;
    }
}