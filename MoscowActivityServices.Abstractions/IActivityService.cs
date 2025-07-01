using MoscowActivityServices.Abstractions.Models;

namespace MoscowActivityServices.Abstractions;

public interface IActivityService
{
    Task<IEnumerable<ScheduleData>> FindSlots(SearchRequest request);
}