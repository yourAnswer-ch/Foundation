# Foundation

The Fondation project includes a collection of libraries that we use as basic building blocks of our own software projects. The projects are shown here as reference examples, but no corresponding Nuget packages are offered at the moment.

Our projects are written in C# and hosted as Docker containers on Azure Kubernetes. By default we use Azure KeyVault, Storage and other Azure services.

The purpose of the libraries is to have standardized project structures that can be easily and quickly assembled and used as a base. Typically we use two basic project types. API projects with Kestrel or self hosted projects whose input usually consists of a queue or a scheduler.


# The repo is divided into:
- Libraries - The actual libraries
- Examples - examples of how to use them
- Tools - Workshows that can be used with the libraries.

# Our Zen:
- Stay as close to the standard as possible.
- Organization of the services via dependency injection if it makes sense.
- As small as possible independent modules for maximum flexibility without ballast.
- As short as possible code with a lot of configurability.
Additional Nuget packages used:
- Microsoft.Extension.Azure (allows Azure clients to use dependency inject).

# Libraries:

## Foundation.Hosting.Kestrel.CertBinding
Configures Kestrel bindings and loads certificates from Azure KeyVault.

# Remarks:
Currently all libraries are programmed in .net7. In principle they would also work with earlier ones. Support for earlier versions may be added later.	
