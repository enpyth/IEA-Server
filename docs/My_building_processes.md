0. Prepare env

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

1. Create Model Class

analyse business flow, create data strcture by drawing a class diagram

vibe coding files in /Models

2. Create DTO Model

/Dtos

3. Database context (delete)

The database context is the main class that coordinates Entity Framework functionality for a data model. 

use DI declare what kind of db want to use in main file

4. Scaffold a Controller

https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio-code#scaffold-a-controller