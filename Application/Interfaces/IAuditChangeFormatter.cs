using Application.Common.Models;

namespace Application.Interfaces;

public interface IAuditChangeFormatter
{
    IReadOnlyList<AuditFieldChange> GetChanges(object? oldValues, object? newValues);

    string? CreateChangeSummary(string? oldValuesJson, string? newValuesJson);

    string? CreateChangeSummary(IReadOnlyCollection<AuditFieldChange> changes);

    string CreateUpdateNote(
        string entityName,
        string displayName,
        IReadOnlyCollection<AuditFieldChange> changes);
}
