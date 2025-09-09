using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ToolbarImportController : MusicMateBehavior
{
    [SerializeField] ToolbarButtonAnimator importFolderButton;
    [SerializeField] ToolbarButtonAnimator importLastFmButton;
    [SerializeField] ButtonAnimator startImportButton;
    [SerializeField] GridImportController gridImportController;

    #region MusicMate Base Class Methods
    protected override void RegisterEventHandlers()
    {
        startImportButton.OnButtonClick.AddListener(OnStartImportClicked);
    }

    protected override void UnregisterEventHandlers()
    {
        startImportButton.OnButtonClick.RemoveListener(OnStartImportClicked);
    }
    #endregion
    
    void OnStartImportClicked()
    {
        ApiService.GetFolderImport(1,20,(callback) => FolderImportCallback(callback));
    }
    
    void FolderImportCallback(PagedResult<ImportReleaseResult> result)
    {
        print("Folder import: " + result);
        gridImportController.SetResult(result);
    }
}