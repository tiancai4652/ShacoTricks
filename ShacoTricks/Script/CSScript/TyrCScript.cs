using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShacoTricks.Script.CSScript
{
    /// <summary>
    /// 尝试CScript脚本
    /// </summary>
    class TyrCScript
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


    }
}
