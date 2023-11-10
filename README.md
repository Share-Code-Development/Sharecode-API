# SHARECODE-BACKEND

The backend for share code is written in .net 8. It utilizes
 - EF 8 for ORM
 - SQL Server (Azure)
 - Cloudflare KV (KeyVault)

The architecture I am planning would be Clean architecture taking advantage of MediatR

Sharecode.Backend.Utilities
 - KeyValueClient
   The key value client is capable of fetching all the keys of the specified namespace, which would then serve as the
   key vault for this project. 

   Cloudflare authentication can be configured in the appsettings.json