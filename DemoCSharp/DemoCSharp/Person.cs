using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCSharp
{
    public class Person<T>
    {
        public string Fullname;

        public Person( string name) {
            this.Fullname = name;
        }
        public string Getname()
        {
            return this.Fullname;
        }
        public string getType(T entity)
        {
            var typeName = typeof(T).Name;
            var name = string.Empty;
            if(entity is Woman)
            {   
                name = entity.GetType().GetField("Fullname").GetValue(entity).ToString();
            }
            return name;
        }
    }
}
