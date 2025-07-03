using System.Text;
using MoscowActivityServices.Abstractions.Models;

namespace Utils;

public static class BotExtensions
{
    public static string GenerateAnswer(Slot slot)
    {
        var sb = new StringBuilder();
        sb.AppendLine(slot.StartDateTime);
        sb.AppendLine(slot.Title);
        sb.AppendLine(slot.Location);
        sb.AppendLine(slot.Specialization);
        sb.AppendLine($"Продолжительность: {slot.Duration.TotalMinutes} мин");
        sb.AppendLine($"Осталось мест: {slot.Count}");
        sb.AppendLine();
        sb.AppendLine(slot.BookingLink);
        
        return sb.ToString();
    }
}