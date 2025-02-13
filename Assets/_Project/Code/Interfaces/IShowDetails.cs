public interface IShowDetails<T1,T2>
{
    void OnInit(T1 result);
    void OnUpdated(T2 model);
}
