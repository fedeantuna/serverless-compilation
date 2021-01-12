# Serverless Compilation using Azure Functions

A small proof of concept just to demonstrate how to do a simple compilation from Azure Functions.

## TODO List:
* Create a query triggered version

## Example to call the function using POST

### URL
http://localhost:7071/api/Execute

### Body examples
```
{
    "Namespace": "Something",
	"Class": "SomethingElse",
	"Method": "SomethingDifferent",
	"Parameters": null,
    "CSharpCode": "
    using System;
    namespace Something
    {
        public class SomethingElse
        {
            public string SomethingDifferent()
            {
                return $\"H3ll0 W0rld\";
            }
        }
    }"
}
```

```
{
    "Namespace": "Something",
	"Class": "SomethingElse",
	"Method": "SomethingDifferent",
	"Parameters": [
        "H3ll0",
        "W0rld!"
    ],
    "CSharpCode": "
    using System;
    namespace Something
    {
        public class SomethingElse
        {
            public string SomethingDifferent(String left, String right)
            {
                return $\"{left} {right}\";
            }
        }
    }"
}
```