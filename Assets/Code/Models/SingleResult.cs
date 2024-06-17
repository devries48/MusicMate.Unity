public class SingleResult<T>
{
    public AdditionalClientData AdditionalClientData { get; set; }
    public object Errors { get; set; }
    public T Data { get; set; }
    public bool? IsSuccess { get; set; }
    public string Messages { get; set; }
    public long? OperationTime { get; set; }
}

public class AdditionalClientData
{
}
