namespace MoscowActivityServices.Abstractions;

public interface IActivityClientFactory
{
    IActivityClient GetClient(string clientName, ActivityClientCompanySettings settings);
}