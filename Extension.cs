using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redox.Patcher
{
    public static class Extension
    {
        public static MethodDefinition GetMethod(this Collection<MethodDefinition> methods,string name)
        {
            return methods.First(x => x.Name == name);
        }
    }
}
