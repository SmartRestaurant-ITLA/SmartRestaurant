using SmartRestaurant.Application.DTOs.Dashboard;

namespace SmartRestaurant.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}