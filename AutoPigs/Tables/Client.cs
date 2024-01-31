using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPigs
{
    public class Client
    {
        [NotNull]
        public string Name { get; set; }

        public Client() { }

        public Client(string Name)
        {
            this.Name = Name;
        }
    }
}
