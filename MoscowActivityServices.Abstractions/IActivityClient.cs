using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Abstractions;

public interface IActivityClient
{
    Task<SearchResponse> Search(SearchRequest request);
    Task Book(BookingRequest request);
}