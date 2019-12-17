using System;
using System.Collections.Generic;
using System.Text;

namespace StreamSuppliers
{
    public class InputDto
    {
        public int CountSupplier { get; set; }

        public int CountCustomers { get; set; }

        public int CountTicks { get; set; }

        public int[] MaxSendBySupplier { get; set; }

        public Dictionary<int, int[]> MaxSendBySupplierInTick { get; set; }

        public Dictionary<int, int[]> MaxGetByCustomerInTick { get; set; }

        public Dictionary<int, int[]> SupplierIdsForCustomer { get; set; }
    }
}
