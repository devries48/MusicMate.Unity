public interface IEditorComponent<T>
{
    void SetModel(T model);
    T GetModel();
}