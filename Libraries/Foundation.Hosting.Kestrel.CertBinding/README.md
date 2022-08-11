# Foundation.Hosting.Kestrel.CertBinding

The CertBinding library for Kestrel allows you to define the port, ip and certificate binding. The certificates are loaded from the Azure KeyVault.

## Code adaptation in Program.cs
```
using Foundation.Hosting.Kestrel.CertBinding;

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureBindings();
});
```
The certificate binder requires at least one KeyVault certificate client that is registered by name. The KeyVault is then reserved in the configuration.

```
builder.Services.AddAzureClients(builder =>
{
    builder.AddCertificateClient(new Uri("https://kv-fd-certificates.vault.azure.net/")).WithName("KV-FD-Certificates");
});
```

## Configuration:
```
"Hosting": {
    "Kestrel": {
      "AddServerHeader": false,
      "Bindings": [
        {
          "IPAddress": "*",
          "Port": "10443",
          "Protocols": "HTTP2|HTTP3",
          "Certificate": {
            "Source": "KV-FD-Certificates",
            "Name": "wildcard-youranswer-ch",
            "Protocols": "TLS13"
          }
        }
      ]
    }
  }
```