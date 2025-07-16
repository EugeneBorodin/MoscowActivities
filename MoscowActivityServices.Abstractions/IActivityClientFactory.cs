namespace MoscowActivityServices.Abstractions;

public interface IActivityClientFactory
{
    IActivityClient GetClient(string clientName, string companyId, int bookingFormId);
}