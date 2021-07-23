using System;

namespace BookOrder.Domain
{
    public class Order
    {
        public string BookKey { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void CancelOrder(CancelRequest cancelRequest)
        {
            // TODO: throw business validation exception if subjects contain fiction
            // TODO: throw business validation exception if cancellation time is greater than threshold

            this.Status = OrderStatus.Cancelled;
        }
    }
}
