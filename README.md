# Overview
This API is a sample project to demonstrate how [MediatR](https://github.com/jbogard/MediatR) and [FluentValidation](https://fluentvalidation.net/) can be leveraged to simplify workflow and validations code when integrating with third-party API. In this example, we will be covering:
- Different types of validations and when to use them,
- Handling third-party payloads,
- Comparing MediatR with repository pattern and services,
- Comparing FluentValidation with attribute-based validations and manual validations, and
- Unit and API/Integration/Component testing using MediatR and FluentValidation

# Requirements
For this example, we are building an API called BookOrder API. The main requirements are:
- User can create a book order by sending book key. The rest of the data will be fetched from [Open Library API](https://openlibrary.org/developers/api) before it is saved to database.
- A third-party API can send a callback to BookOrder API with a payload that contains book key and the command it needs to execute. To simplify things, only cancel order command is supported in callback right now. Similar to create order, this command requires the API to fetch more data from OpenLibrary API before proceeding further.

In addition to that, there are some supporting requirements:
- Callback operation should not return an error if the command in the payload is not supported.
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
- Perform business logic operations and save the data in DB

### 4. Return the newly created book order to user

## Handling third-party callback for cancelling order
When there is a third-party callback to cancel order, the following will happen:
### 1. API will receive `ThirdPartyCallbackPayload`
This will then get packaged inside `GetCallbackCommandQuery` and sent to `GetCallbackCommandQuery.Handler`
- It is better to wrap the payload inside `GetCallbackCommandQuery`, instead of defining `GetCallbackCommandQuery` as the payload. Third-party payload might contain a lot of information and might require some attributes to translate the JSON properties. Adding them to MediatR request will clutter the code too much.
- Same as usual, the `GetCallbackCommandQuery` will get validated before it can reach the handler.

### 2. `GetCallbackCommandQuery` will determine which command needs to be executed.
- As the callback end point is a generic one, we will need to determine the command that needs to be executed based on the payload's content.
- `GetCallbackCommandQuery` is only returning the next MediatR command instead of executing them to prevent nested handlers, which is [heavily discouraged for MediatR](https://jimmybogard.com/mediatr-9-0-released/).
- If the command is not supported, it will return null, which will be ignored by the controller.

### 3. Process the command (`CancelBookOrderCommand`)
- The flow is similar to create order command. However, instead of creating `Order` domain model, it will create `CancelRequest` instead.
- `CancelRequest` is constructed from the same `BookApiResponse` class that is used in `CreateBookOrderCommand`. `CancelBookOrderCommand` also needs to do validation against `BookApiResponse` before converting it into `CancelRequest`, however it will be using different RuleSet to the one used in `CreateBookOrderCommand`.
- Validations inside `MediatR` handlers and domain objects are done manually and not through `FluentValidation`. That is because those validations are meant to be part of the business logic and not simply dealing with invalid data. By keeping those validations there, devs will have clear visibility on the business logic when looking at handler and domain objects code.

### 4. Return success to third-party caller
The callback endpoint usually doesn't give any response objects back as it is a generic end point and the response from different commands might be different. Hence, if everything works, it will just return success.

## Design Considerations
### MediatR request handler and validator are defined as subclasses of the Command/Query class. 
- This will help devs to navigate to command/query handler and validator easily from the controller file.
- It also helps giving all the context of the command/query to dev without requiring them to navigate to multiple files.
- This also simplifies the naming of the handler and the validator. It is now perfectly fine to name all MediatR handlers as `Handler` and all command/query validators as `Validator` since it can only be accessed by specifying the main class so it is descriptive and unique enough, e.g. `CreateBookOrderCommand.Handler` and `CreateBookOrderCommand.Validator`.

### MediatR command and query validations are automatically triggered
- This is done through `ValidationBehaviour`, which utilises [Pipeline Behaviors](https://github.com/jbogard/MediatR/wiki/Behaviors) feature from MediatR.
- This means all commands/queries will be validated before they even reach the handlers and validatione exceptions will be thrown if they are invalid. This will guarantee that MediatR handlers will only deal with valid data, which helps simplifying the code.

# Architecture Review
## Different types of validations and when to use them
In general, for this sample project, we covered 3 types of validations:
### 1. MediatR Command/Query Validations
This type of validation is mainly dealing with bad data in end point payload, e.g. missing required fields, wrong data format, invalid value, etc. They are usually simple validation but there are a lot of them and can be quite distracting when placed in MediatR handler or Domain objects. Using FluentValidation that is triggered automatically is perfect for this type of validation.
### 2. 3rd Party Response Payload Validations
This is pretty similar to the previous one, however the code to retrieve the payload is usually located in the middle of MediatR handler, so a bit tricky to handle it automatically. It's better to trigger the validation inside AutoMapper custom converter or inside bespoke custom adapter class before transforming it to domain object. This will also ensure that the domain object is always in a valid state. Same as above, FluentValidation fits nicely for this type of validation.
### 3. Business Logic Validations
Different to the other two validations, in this case the data is usually valid, however the operation cannot be processed due to the business logic. It is better to handle them inside MediatR handlers or business objects so devs can have clear visibility on them.

## Handling third-party payloads
I used AutoMapper custom converter to convert the OpenLibrary response data to Domain objects. I avoid using standard mapping configuration as the fields are usually too different and the mapping rules can get too complex and unreadable.

Another way to convert the data is by using bespoke custom converter/adapter (not using AutoMapper interface). The effort is probably pretty similar to creating AutoMapper custom converter, but we can get the following benefits:
- Having the method name that suits the use case. Instead of `var cancelRequest = _mapper.Map<CancelRequest>(bookInfo);`, `var cancelRequest = _adapter.CreateCancelRequest(bookInfo);` probably makes more sense.
- Being able to add extra parameters as needed when there are multiple data sources for constructing domain objects, e.g. `var cancelRequest = _adapter.CreateCancelRequest(userId, bookInfo);`

## Comparing MediatR with creating services using ServiceCollection
Based on my experience, MediatR gives the following benefits over regular services:
- Less setups from caller's perspective. For example, `BookOrderController` only needs to know about `IMediator` instead of multiple services it needs for different end points.
- Grouping code based on specific command/query (Request, Handler, Validator in one file) as opposed to grouping them based on type in different classes/namespaces (e.g. all validators in one class/namespace, all services in another class/namespace, etc). This really helps giving dev full picture in one single file.
- Ability to trigger validator automatically through pipeline behaviors, as well as other features such as pub-sub pattern

That being said, it does have some limitations. While most limitations can be mitigated with good conventions (e.g. devs will not forget to create request handler if it's in the same file as the request class), there is one limitation that is really crucial: MediatR does not support nested handlers, i.e. sending requests from anywhere inside a request handler. This is done by design since nested MediaR handlers can lead to unnecessary complexity. Instead of creating nested handlers, consider the following approaches instead:
1. Run them sequentially, similar to how it is done in `BookOrderController.ProcessThirdPartyCallback`
2. Raise events using pub-sub/notification pattern (see example in [here](https://www.rahulpnath.com/blog/avoid-commands-calling-commands/))
3. Create a service using ServiceCollection for common/reusable code and call them from MediatR handlers. This approach is better than using service exclusively, as this will keep the services clean and concise as it will only contain several common and reusable code.

## Comparing FluentValidation with attribute-based validations and manual validations
Overall, I believe FluentValidation is a better approach compared to attribute-based validations and manual validations. This is because:
1. It allows multiple validation rulesets for the same class that can be used separately in different situations, as shown in `BookApiResponseValidator`. This is not possible in attribute-based validations as the attributes are attached to the class definition.
2. It allows the validator to be injected using dependency injection, which is also not possible when using attribute-based validations for the same reason as above.
3. It abstracts out the bad data validation from the main code and keep it clean. Imagine how `CreateBookOrderCommand.Handler` will look if we implement all the validations from `CreateBookOrderCommand.Validator` and `BookApiResponseValidator` as `if-else` blocks.

FluentValidation does have one thing to be cautious of though, and that is to be mindful of not hiding too much validation logic in the code, especially because it is not as explicity as attribute-based validation or manual validation. Hence, FluentValidation shouldn't be used for business logic validations and it is important to make it clear in coding standard/convention when it is being used.

## Unit and API/Integration/Component testing using MediatR and FluentValidation
One of the main advantages of using MediatR and FluentValidation is the ability to decouple the code using Dependency Injection. Below are some of the key takeaways that I want to highlight:
### MediatR tests
- MediatR can be tested just like regular services in ServiceCollection. The dependencies in the handlers can be mocked.
- When unit testing the handlers, Pipeline Behaviors won't be triggered. The Pipeline Behaviors can also be unit tested, although it's probably better to cover them in API/Integration/Component instead of unit tests.
- When writing tests for its consumer, we can mock `IMediator` and its `Send` method

### FluentValidation tests
- Same as MediatR, the validators can be tested just like regular services in ServiceCollection. The dependencies in the validators can be mocked. See `BookApiResponseValidatorTests` for an example.
- It can be a bit tricky to mock the validators when writing tests for its consumer. Firstly, the method that needs to be mocked is `ValidationResult Validate(IValidationContext context);`. Secondly, checking the rulesets and other specific validation strategy might require complicated setup. See `BookApiResponseToCancelRequestConverterTests` and `BookApiResponseToOrderConverterTests` for examples on how to do this.

# Running locally
- The API is using .NET Core 3.1 and can be run with any IDE such as Visual Studio or Rider. 
- The API will try to hit [Open Library API](https://openlibrary.org/developers/api) so internet connection is required to run the API or its tests locally. The rest of the unit tests can be run without any internet connection.
- There are no authentications or special setups required for this sample project.

# Dev Note
- This sample code omitted essential parts of API on purpose (e.g. security, documentation, infrastructure) to keep the solution simple and highlight the problems it is trying to solve. 
- When building real API, please ensure it is fit for production and have those essential parts built-in.
