# Serverless Compilation using Azure Functions

A small proof of concept just to demonstrate how to do a simple compilation from Azure Functions.

## TODO List:
* Obtain everything from the body of the request as alternative to using the query string
* Add a parameters option in the body to accept passing parameters to the method
* Create a query triggered version

## Example to call the function using POST

### URL
http://localhost:7071/api/Execute?namespace=Something&class=SomethingElse&method=SomethingDifferent
### Body
```
{
    "csharpcode": "
    using System;
    namespace Something
    {
        public class SomethingElse
        {
            public string SomethingDifferent()
            {
                return \"H3ll0 W0rld!\";
            }
        }
    }"
}
```