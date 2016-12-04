using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonOrders.Models
{
    class Item
    {
        public string Name { get; set; }
        public string SoldBy { get; set; }
        public string Price { get; set; }

        public override string ToString()
        {
            return string.Format("{0} -- {1} -- {2}", Name, SoldBy, Price);
        }
    }
}
