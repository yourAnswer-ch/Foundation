using Microsoft.Azure.Cosmos;

namespace Foundation.CosmosDb;

internal static class QueryDefinitionExtensions
{
    internal static QueryDefinition CreateQuery(this string query, IDictionary<string, object> parameters, bool useCamelCase)
    {
        var queryDefinition = new QueryDefinition(query);
        foreach (var parameter in parameters)
        {
            queryDefinition.WithParameter(FormatName(parameter.Key, useCamelCase), parameter.Value);
        }

        return queryDefinition;
    }

    internal static QueryDefinition CreateQueryFromObject(this string query, object parameters, bool useCamelCase)
    {
        var queryDefinition = new QueryDefinition(query);

        if (parameters == null)
            return queryDefinition;

        var type = parameters.GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(parameters);
            queryDefinition.WithParameter(FormatName(property.Name, useCamelCase), value);
        }

        return queryDefinition;
    }

    private static string FormatName(string input, bool useCamelCase)
    {       
        if (string.IsNullOrEmpty(input))
            return input; // or throw an exception, depending on your needs

        var name = input.TrimStart('@');
        
        return !useCamelCase ? $"@{name}" : $"@{char.ToLower(name[0])}{name[1..]}";
    }
}