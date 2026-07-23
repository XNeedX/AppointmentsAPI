using AppointmentsAPI.Domain.Enums;

namespace AppointmentsAPI.Domain.Extensions;

public static class ServiceCategoryExtensions
{
    public static int GetDurationMinutes(this ServiceCategory category)
    {
        return category switch
        {
            ServiceCategory.Analyses => 10,
            ServiceCategory.Consultations => 20,
            ServiceCategory.Diagnostics => 30,
            _ => 10 
        };
    }
}