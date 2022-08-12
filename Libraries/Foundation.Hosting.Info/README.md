# Foundation.Hosting.Info

It happens that everything works locally but does not work on the system. The libraries list system environment information and package versions and writes them to the log and also the console. This information helps to easily detect if the correct version was rolled out. In addition, the program startup is embellished with some ASCI art.

## Code adaptation in Program.cs
```
using Foundation.Hosting.Info;

app.RegisterLifetimeLogger();

```

## Configuration:
```
"  "ApiStatus": {
    "AppName": "Example Api",
    "EnableStatusController": true,
    "EnableConsoleStartupSummery": true,
    "ShowAssemblies": true,
    "ShowEnvironmentVariables": true
  }
```

It is not necessary to insert the entire configuration mandated is only the app name. 

# Screenshots

![Assembls and Environment variables](./Resources/screenshot-01.png)
![Sartup banner](./Resources/screenshot-02.png)