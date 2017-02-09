using System;
using System.Linq;
using Absentia.Model;

namespace Absentia.Domain
{
    public class SubscriptionManager
    {
        private IRepository _repository;
        private readonly ISubscriptionService _subscriptionService;
        private TimeSpan _subscriptionRenewalTimeLeftTrigger;
        private INotificationService _notificationService;

        public SubscriptionManager(IRepository repository, ISubscriptionService subscriptionService, INotificationService notificationService,
            TimeSpan subscriptionRenewalTimeLeftTrigger)
        {
            _notificationService = notificationService;
            _subscriptionRenewalTimeLeftTrigger = subscriptionRenewalTimeLeftTrigger;
            _repository = repository;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// runs periodically and ensures subscription exists for all users calendars
        /// </summary>
        public void Process()
        {
            var users = _repository.GetUsers();
            foreach (var directoryUser in users)
            {
                var valid =
                    directoryUser.Subscriptions
                        .Where(x => x.ExpirationDateTime > DateTime.UtcNow)
                        .OrderByDescending(x => x.ExpirationDateTime)
                        .ToList();
                if (!valid.Any())
                {
                    //need to add a subscription
                    try
                    {
                        var addedSubscription = _subscriptionService.AddSubscription(directoryUser.UserName);
                        _repository.AddSubscription(directoryUser.UserName, addedSubscription);
                    }
                    catch (Exception ex)
                    {
                        _repository.AddSubscriptionAttempt(directoryUser.UserName, false, ex.Message, DateTime.UtcNow);
                    }
                }
                else
                {
                    //need to renew
                    if (valid.First().ExpirationDateTime < DateTime.UtcNow.Add(_subscriptionRenewalTimeLeftTrigger))
                    {
                       var newExpiration = _subscriptionService.RenewSubscription(valid.First().Id);
                        _repository.UpdateSubscriptionExpiry(valid.First().Id, newExpiration);
                    }
                }
            }
           _notificationService.ProcessNotifications();
        }
    }
}
