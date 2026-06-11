using System.Text.Json;
using Application.Common.Models;
using Application.Interfaces;

namespace Application.Common.Services;

public class AuditChangeFormatter : IAuditChangeFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public IReadOnlyList<AuditFieldChange> GetChanges(object? oldValues, object? newValues)
    {
        if (oldValues == null || newValues == null)
            return [];

        var oldElement = JsonSerializer.SerializeToElement(oldValues, JsonOptions);
        var newElement = JsonSerializer.SerializeToElement(newValues, JsonOptions);

        return GetChanges(oldElement, newElement);
    }

    public string? CreateChangeSummary(string? oldValuesJson, string? newValuesJson)
    {
        if (string.IsNullOrWhiteSpace(oldValuesJson) || string.IsNullOrWhiteSpace(newValuesJson))
            return null;

        try
        {
            using var oldDocument = JsonDocument.Parse(oldValuesJson);
            using var newDocument = JsonDocument.Parse(newValuesJson);

            if (oldDocument.RootElement.ValueKind != JsonValueKind.Object ||
                newDocument.RootElement.ValueKind != JsonValueKind.Object)
                return null;

            return CreateChangeSummary(GetChanges(oldDocument.RootElement, newDocument.RootElement));
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public string? CreateChangeSummary(IReadOnlyCollection<AuditFieldChange> changes)
    {
        return changes.Count == 0
            ? null
            : string.Join("; ", changes.Select(x => $"{x.DisplayName} changed from {x.OldValue} to {x.NewValue}"));
    }

    public string CreateUpdateNote(
        string entityName,
        string displayName,
        IReadOnlyCollection<AuditFieldChange> changes)
    {
        var summary = CreateChangeSummary(changes);

        return summary == null
            ? $"{entityName} updated: {displayName}."
            : $"{entityName} updated: {displayName}; {summary}.";
    }

    private static IReadOnlyList<AuditFieldChange> GetChanges(JsonElement oldElement, JsonElement newElement)
    {
        if (oldElement.ValueKind != JsonValueKind.Object ||
            newElement.ValueKind != JsonValueKind.Object)
            return [];

        var changes = new List<AuditFieldChange>();

        foreach (var newProperty in newElement.EnumerateObject())
        {
            if (!oldElement.TryGetProperty(newProperty.Name, out var oldProperty))
                continue;

            if (ValuesAreEqual(oldProperty, newProperty.Value))
                continue;

            changes.Add(new AuditFieldChange
            {
                FieldName = newProperty.Name,
                DisplayName = FormatPropertyName(newProperty.Name),
                OldValue = FormatValue(oldProperty),
                NewValue = FormatValue(newProperty.Value)
            });
        }

        return changes;
    }

    private static string FormatValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => string.IsNullOrWhiteSpace(value.GetString()) ? "blank" : value.GetString()!,
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "Yes",
            JsonValueKind.False => "No",
            JsonValueKind.Null => "blank",
            _ => value.ToString()
        };
    }

    private static bool ValuesAreEqual(JsonElement oldValue, JsonElement newValue)
    {
        if (TryGetDecimal(oldValue, out var oldDecimal) && TryGetDecimal(newValue, out var newDecimal))
            return oldDecimal == newDecimal;

        return string.Equals(
            FormatValue(oldValue).Trim(),
            FormatValue(newValue).Trim(),
            StringComparison.Ordinal);
    }

    private static bool TryGetDecimal(JsonElement value, out decimal result)
    {
        if (value.ValueKind == JsonValueKind.Number)
            return value.TryGetDecimal(out result);

        if (value.ValueKind == JsonValueKind.String)
            return decimal.TryParse(value.GetString(), out result);

        result = default;
        return false;
    }

    private static string FormatPropertyName(string propertyName)
    {
        return propertyName switch
        {
            "sku" => "SKU",
            "basePrice" => "Base price",
            "requiresSds" => "SDS required",
            "isRestricted" => "Restricted",
            "isActive" => "Active",
            "unNumber" => "UN number",
            _ => string.Concat(propertyName.Select((x, i) =>
                    i > 0 && char.IsUpper(x) ? $" {x}" : x.ToString()))
                .Trim()
                .ToLowerInvariant()
        };
    }
}
