using UnityEngine;

public interface ICellAnimator
{
    bool IsSelected { get; set; }
    Transform CellTransform { get; }   
    RectTransform ActionPanel { get; set; }
    IGridController ParentGrid { get; }
}

public interface IGridController
{
    RectTransform CreateActionPanel(ICellAnimator cell);
    void DestroyActionPanel(ICellAnimator cell);
}
