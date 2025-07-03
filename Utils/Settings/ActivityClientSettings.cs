namespace Utils.Settings;

public class ActivityClientSettings
{
    public string Token { get; set; }
    public Dictionary<string, ActivityClientData> Clients { get; set; } = new();
}

public class ActivityClientData
{
    public string BaseAddress { get; set; }
    public string CompanyId { get; set; }
}