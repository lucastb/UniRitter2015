using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRitter.UniRitter2015.Models
{
    abstract public class Model : IModel
    {
        public Guid? id { get; set; }

        abstract public bool Equals(Model other);

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                return Equals(obj as Model);
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}