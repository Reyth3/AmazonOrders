using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonOrders.Models
{
    class Order
    {
        public string Placed { get; set; }
        public string Total { get; set; }
        public List<Item> Items { get; set; }

        public override string ToString()
        {
            return string.Format("Placed on: {0} -- {1} -- Items: {2}", Placed, Total, Items.Count());
        }
    }
}
