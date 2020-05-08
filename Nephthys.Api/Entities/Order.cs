using System;

namespace Nephthys.Api.Entities
{
    public class Order
    {

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public Decimal OrderValue { get; set; }
        public bool Shipped { get; set; }
    }
}
