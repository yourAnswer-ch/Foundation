# Foundation.Hosting.Kestrel.CertBinding

Die CertBinding Bibliotheke für Kestrel ermöglicht die Definition der Port, Ip und Zertifikatsbindung. Die Zertifikate werden dabei aus dem Azure KeyVault geladen.

## Codeanpassung im Program.cs
```
using Foundation.Hosting.Kestrel.CertBinding;

builder.WebHost.ConfigureKestrel(o =>
{
    o.ConfigureBindings();
});
```

## Konfiguration:
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