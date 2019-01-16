using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShacoTricksTests.CSScript.CSScript
{
    [TestClass()]
    public class CSScriptTests
    {
        static string classCode = @"public class ScriptedClass
                                    {
                                        public string HelloWorld {get;set;}
                                        public ScriptedClass()
                                        {
                                            //{$comment}
                                            HelloWorld = ""Hello Roslyn!"";
                                        }
                                    }";

        /// <summary>
        /// Gets the unique class code so caching is disabled.
        /// </summary>
        static string uniqueClassCode
        {
            get { return classCode.Replace("{$comment}", Guid.NewGuid().ToString()); }
        }

        [TestMethod]
        public void CompileCode()
        {
            string scriptAsm =CSScriptLibrary.CSScript.CompileCode(uniqueClassCode);
            Console.WriteLine(scriptAsm);
            Console.ReadKey();
        }

    }
}
