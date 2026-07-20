namespace Application.Interfaces;

public interface IAuditChangeFormatter
{
    string? CreateChangeSummary(string? oldValuesJson, string? newValuesJson);
}
