using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElightContract
{
    //we store all the prefixes in one place to avoid unnecessary runtime errors that
    //are very hard to debug, especially in case of smart-contarcts
    public static class Prefixes
    {
        public const string PROGRAM_PREFIX = "PP";          
        public const string PROGRAM_COUNTER_PREFIX = "PC"; 
    }
}
