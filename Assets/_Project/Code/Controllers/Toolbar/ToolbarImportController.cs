using System.Collections;
using UnityEngine;

public class ToolbarImportController : ToolbarControllerBase
{
    [SerializeField] ToolbarButtonAnimator _scanFolderButton;
    [SerializeField] ToolbarButtonAnimator _scanLastFmButton;

    readonly float _checkRunningInterval = 10f;

    void OnDisable() => CancelInvoke();

    protected override void InitElements()
    {
        _scanFolderButton.SetInteractable(true);
        _scanFolderButton.OnButtonClick.AddListener(() => OnScanFolderClicked());
    }

    protected override IEnumerator SetElementStates()
    {
        if (_scanFolderButton.CanShowSpinner)
        {
            _scanFolderButton.ShowSpinner();
            m_ApiService.FolderImportStart((callback) => FolderImportCallback(callback));
            InvokeRepeating(nameof(CheckImportRunning), _checkRunningInterval, _checkRunningInterval);
        }
        else if (_scanFolderButton.CanHideSpinner)
        {
            CancelInvoke(nameof(CheckImportRunning));
            _scanFolderButton.HideSpinner();
        }

        yield return null;
    }

    void OnScanFolderClicked()
    {
        _scanFolderButton.IsSpinning = true;
        ChangeElementStates();
    }

    void CheckImportRunning()
    {
        m_ApiService.IsFolderImportRunning((isRunning) =>
        {
            if (_scanFolderButton.IsSpinning != isRunning)
            {
                _scanFolderButton.IsSpinning = isRunning;
                ChangeElementStates();
            }
        });
    }

    void FolderImportCallback(string result)
    {
        print("Folder import: " + result);
    }

}