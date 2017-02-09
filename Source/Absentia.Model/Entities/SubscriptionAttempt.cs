using System;

namespace Absentia.Model.Entities
{
    public class SubscriptionAttempt
    {
        public int SubscriptionAttemptId { get; set; }
        public string UserName { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime AttemptTime { get; set; }
    }
}