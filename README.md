# Funda Assessment
Assumptions:
1. "Your implementation should mitigate (avoid) errors and handle any errors that occur, so take both into account"
  I used Polly.NET and created 2 policies:
  a) Rate Limiter policy which should spread and control load on external Funda API. "mitigate (avoid) errors" part
  b) Wait and retry policy for my HttpClient to handle some errors that occur. "handle any errors that occur" part
  We have a rate limiter and retry logic for the Funda Api client.
  I did not tune parameters for rate limiter and retry logic. I used some common sense values.
2. I implemented a Clean Architecture pattern.
   The domain layer is separated from anything else. We can easily write unit tests without any mocks.
   The Application layer only references the Domain layer. We can use the application with any type of UI (Console, Web, WPF).
   The Infrastructure layer encapsulates work with Funda API.


As soon as it is test assessment and I have restricted time:
1. I did not implement proper error handling logic. I also could use the Result pattern instead of exceptions.
2. I created unit tests only for domain service. My solution is testable. It is not a problem to write some integration tests as well.
3. It is not a production quality code.
4. I did not use MediatR, AutoMapper, etc. Time to set all tools up I put in implementing bare minimum.
5. I added cancellation tokens for demonstration purposes. For a real application, we should also implement trigger for cancellation.
6. I did not make the API of my use cases generic. User can not specify city or any other option except "has a garden" and "top count".
7. I did not create any specific exception for the Domain layer and used standard exceptions. (For example, I could create a "negative top count exception" and handle it)
8. I did not put configuration for policies in appsettings.json to save time. I create a simple static class with configuration.
9. All my code is only for demonstration purposes. We can discuss all the missing elements in an interview.

Technical: I used .NET6 framework, XUnit, FluentAssertions, Polly.NET.


Requirements:
Determine which makelaar's in Amsterdam have the most object listed for sale. 
Make a table of the top 10. 
Then do the same thing but only for objects with a tuin which are listed for sale. 
For the assignment you may write a program in any object oriented language of your choice and you may use any libraries that you find useful.
Tips:
If you perform too many API requests in a short period of time (>100 per minute), the API will
reject the requests. Your implementation should mitigate (avoid) errors and handle any errors
that occur, so take both into account.
