# Overview
This API is a sample project to demonstrate how [MediatR](https://github.com/jbogard/MediatR) and [FluentValidation](https://fluentvalidation.net/) can be leveraged to simplify workflow and validations code when integrating with third-party API. In this example, we will be covering:
- Different types of validations and when to use them,
- Handling third-party payloads
- Comparing MediatR with repository pattern and services,
- Comparing FluentValidation with attribute-based validations and manual validations, and
- Unit and API/Integration/Component testing using MediatR and FluentValidation

# Requirements
For this example, we are building an API called BookOrder API. The main requirements are:
- User can create a book order by sending book key. The rest of the data will be fetched from [Open Library API](https://openlibrary.org/developers/api) before it is saved to database.
- A third-party API can send a callback to BookOrder API with a payload that contains book key and the operation it needs to execute. To simplify things, only cancel order operation is supported in callback right now. Similar to create order, this operation requires the API to fetch more data from OpenLibrary API before proceeding further.

In addition to that, there are some supporting requirements:
- Callback operation should not return an error if the operation in the payload is not supported/
- Callback operation should not return an error if the book order has been cancelled. 
- If error is thrown in callback operation, the third-party API will keep retrying.
- Security and user management are not part of the scope for now.
- OpenLibrary API will return the same response data format for both create and cancel order, however each operation will have different required data and will have different validation requirements.

# Code Architecture
The code is structured to use [CQRS](https://martinfowler.com/bliki/CQRS.html) using MediatR (See [Microsoft guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-application-layer-implementation-web-api) and [Jason Taylor's post](https://jasontaylor.dev/clean-architecture-getting-started/) for more references) and we will be using FluentValidation heavily for the validations. We are also using the following technologies:
- [In-memory database](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli) from EF Core
- [XUnit](https://xunit.net/), [Moq](https://github.com/moq/moq4), [FluentAssertion](https://fluentassertions.com/), and [AutoFixture](https://github.com/AutoFixture/AutoFixture) for unit tests and API/integration/component tests. We are also using AutoFixture nuget package for XUnit to simplify generating random data in XUnit tests.
- [AutoMapper](https://automapper.org/) for object to object mapping

## Creating Book Order
When the user creates a book order through API, the following will happen:
### 1. API will receive a command payload to create book order
We will be using MediatR command `CreateBookOrderCommand` as the end point payload.  The command will be sent to its handler as is.

### 2. Command will be validated before reaching the handler
The `CreateBookOrderCommand` will be validated by `CreateBookOrderCommand.Validator` before it reaches `CreateBookOrderCommand.Handler`. See design consideration below on how it happens.

### 3. Command is processed by the handler
- Retrieve more data for book order from bookApiClient. This will give us the data in OpenLibrary response format.
- Convert the data into our domain model (`Order`) using AutoMapper. The OpenLibrary response data is validated using fluent validation inside AutoMapper custom converter. We are utilising `RuleSet` feature from `FluentValidation` to have multiple validation rules for a single `BookApiResponse` class.


## Design Considerations
### MediatR request handler and validator are defined as subclasses of the Command/Query class. 
- This will help devs to navigate to command/query handler and validator easily from the controller file.
- It also helps giving all the context of the command/query to dev without requiring them to navigate to multiple files.
- This also simplifies the naming of the handler and the validator. It is now perfectly fine to name all MediatR handlers as `Handler` and all command/query validators as `Validator` since it can only be accessed by specifying the main class so it is descriptive and unique enough, e.g. `CreateBookOrderCommand.Handler` and `CreateBookOrderCommand.Validator`.

### MediatR command and query validations are automatically triggered
- This is done through `ValidationBehaviour`, which utilises [Pipeline Behaviors](https://github.com/jbogard/MediatR/wiki/Behaviors) feature from MediatR.
- This means all commands/queries will be validated before they even reach the handlers and validatione exceptions will be thrown if they are invalid. This will guarantee that MediatR handlers will only deal with valid data, which helps simplifying the code.



# Architecture Review

# Running locally
- The API is using .NET Core 3.1 and can be run with any IDE such as Visual Studio or Rider. 
- The API will try to hit [Open Library API](https://openlibrary.org/developers/api) so internet connection is required to run the API or its tests locally. The rest of the unit tests can be run without any internet connection.
- There are no authentications or special setups required for this sample project. 

# Dev Note
- This sample code omitted essential parts of API on purpose (e.g. security, documentation, infrastructure) to keep the solution simple and highlight the problems it is trying to solve. 
- When building real API, please ensure it is fit for production and have those essential parts built-in.