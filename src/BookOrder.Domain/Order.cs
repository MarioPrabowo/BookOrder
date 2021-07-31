using BookOrder.Domain.Common;
using System;

namespace BookOrder.Domain
{
    public class Order
    {
        public string BookKey { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Covers { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public OrderOperationResult CancelOrder(CancelRequest cancelRequest)
        {
            if(cancelRequest.BookKey != this.BookKey)
            {
                throw new BusinessLogicException("Wrong book key");
            }

            if (this.Status == OrderStatus.Cancelled)
            {
                return OrderOperationResult.NoChangesRequired;
            }

            this.Status = OrderStatus.Cancelled;
            this.Covers = cancelRequest.Covers;
            this.UpdatedAt = cancelRequest.CancellationTime;
            return OrderOperationResult.ChangesMade;
        }
    }
}
