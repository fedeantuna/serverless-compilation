using System;

namespace ServerlessCompilation.FunctionApp.Models
{
    public class CodeDefinition
    {
        public String Namespace { get; set; }

        public String Class { get; set; }

        public String Method { get; set; }

        public String[] Parameters { get; set; }

        public String CSharpCode { get; set; }
    }
}