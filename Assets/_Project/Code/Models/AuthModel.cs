using System;

[Serializable]
public class AuthModel
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
