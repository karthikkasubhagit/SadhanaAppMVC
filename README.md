Web application built to streamline daily activities/practices for the members of a non-profit organization (ISKCON). It's hosted entirely on the Azure cloud platform.

Core Technologies:

      Frontend/Backend: ASP.NET Core MVC
      Database: Azure SQL Database
      Hosting: Azure App Service
      Serverless & Messaging: Azure Functions, Azure Service Bus


This application is designed for a restricted set of internal users. While the immediate scope does not necessitate enterprise-grade security features or transaction-level guarantees, utmost care has been taken to secure sensitive configurations, such as connection strings in Azure Key Vault, Single Sign-On (SSO) with Google and Managed Identities. Additional security layers will be integrated as the application's needs and scope expand.
