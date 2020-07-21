# GraphQL - Workshop

[![Build Status](https://dev.azure.com/dotnet/AspNetCoreWorkshop/_apis/build/status/ASP.NET%20Workshop-ASP.NET%20Core%203.x?branchName=master)](https://dev.azure.com/dotnet/AspNetCoreWorkshop/_build/latest?definitionId=71&branchName=master)

[BackEnd Web API](https://aspnetcorews-backend.azurewebsites.net) | [FrontEnd Web App](https://aspnetcorews-frontend.azurewebsites.net)

## Prerequisites

For this workshop we need a couple of prerequisites. First, we need the [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1).

Then we need some IDE/Editor in order to do some proper C# coding, you can use [VSCode](https://code.visualstudio.com/) or if you have already on your system Visual Studio or JetBrains Rider.

Last but not least we will use our GraphQL IDE [Banana Cakepop](https://hotchocolate.io/docs/banana-cakepop).

> Note: When installing Visual Studio you only need to install the `ASP.NET and web development` workload.

## What you'll be building

In this workshop, you'll learn by building a full-featured GraphQL Server with ASP.NET Core from scratch. We'll start from File/New and build up to an API back-end application, a web front-end application, and a common library for shared data transfer objects using .NET Standard.

### Application Architecture

![Architecture Diagram](/docs/images/ConferencePlannerArchitectureDiagram.svg)

### Database Schema

![Database Schema Diagram](/docs/conference-planner-db-diagram.png)

## Sessions

| Session | Topics |
| ----- | ---- |
| [Session #1](/docs/1_creating-a-graphql-server-project) | Building a basic GraphQL server API |
| [Session #2](/docs/2_building-out-the-graphql-server.md) | Controlling nullability, adding more database models and adding more queries and mutations.  |  |
| [Session #3](/docs/) | Adding Paging, Filtering, DataLoader and relay compliance |
| [Session #4](/docs/) | Adding security concepts |
| [Session #5](/docs/) | Add user association and personal agenda |
| [Session #6](docs/) | Deployment, Azure and other production environments, configuring environments, diagnostics |
| [Session #7](/docs/) | Challenges |
| [Session #8](/docs/8.%20SPA%20FrontEnd.md) | SPA front-end |
