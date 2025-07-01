using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Abstractions;

public interface IActivityService
{
    Task<IEnumerable<Slot>> FindSlots(SearchRequest request);
}