using Certes;
using System.Text;

namespace Foundation.Azure.CertManager.Core;

public static class Formator
{
    public static string Exception(AcmeRequestException exception, string commandName)
    {
        var builder = new StringBuilder();            
        
        builder.AppendLine($"Pipeline - command '{commandName}' failt to execute: {exception.Message}");

        if (exception.Error != null)
        {
            if (exception.Error.Identifier != null)
                builder.AppendLine($"Identifier type: {exception.Error.Identifier.Type} value: {exception.Error.Identifier.Value}");

            builder.AppendLine($"Details: {exception.Error.Detail}");
            builder.AppendLine($"Type: {exception.Error.Type}");

            if (exception.Error.Subproblems != null)
            {
                foreach (var p in exception.Error.Subproblems)
                {
                    if (exception.Error.Identifier != null)
                        builder.AppendLine($"Identifier type: {p.Identifier.Type} value: {p.Identifier.Value}");

                    builder.AppendLine($"Details: {p.Detail}");
                    builder.AppendLine($"Type: {p.Type}");
                }
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}

