using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Absentia.Model.Entities;

namespace Absentia.Model
{
    public interface IRepository
    {
        IEnumerable<DirectoryUser> GetUsers();
        IEnumerable<Notification> GetPendingNotifications();
        void SetNotificationProcessingResult(int notificationId, bool result, string message);
        bool AddSubscription(string username, Subscription subscription);
        Subscription GetSubscription(string subscriptionId);
        bool AddNotifcation(Notification notification);
        void AddSubscriptionAttempt(string username, bool success, string message, DateTime attemptTime);
        void UpdateSubscriptionExpiry(string subscriptionId, DateTime subscriptionExpiryUtc);
        string GetUsernameFromSubscription(string subscriptionId);
    }
    public class Repository : IRepository
    {
        public IEnumerable<DirectoryUser> GetUsers()
        {
            using (var ctx = new AbsentiaDbContext())
            {
                return ctx.Users.Include(u => u.Subscriptions).ToList();
            }
        }

        public IEnumerable<Notification> GetPendingNotifications()
        {
            using (var ctx = new AbsentiaDbContext())
            {
                return ctx.Notifications.Where(x => !x.ProcessingResult.HasValue).ToList();
            }
        }

        public void SetNotificationProcessingResult(int notificationId, bool result, string message)
        {

            using (var ctx = new AbsentiaDbContext())
            {
                Notification pr = new Notification()
                {
                    NotificationId = notificationId,
                };

                ctx.Notifications.Attach(pr);
                pr.ProcessingResult = result;
                pr.ProcessingMessage = message;
                pr.NotificationProcessedTimeUtc = DateTime.UtcNow;

                ctx.SaveChanges();
            }
        }

        public bool AddSubscription(string username, Subscription subscription)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                var user = ctx.Users.Single(x => x.UserName.Equals(username));
                user.Subscriptions.Add(subscription);
                return ctx.SaveChanges() > 0;
            }
        }

        public Subscription GetSubscription(string subscriptionId)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                return ctx.Subscriptions.SingleOrDefault(x => x.Id.Equals(subscriptionId));
            }
        }

        public bool RemoveNotification(int notificationId)
        {

            using (var ctx = new AbsentiaDbContext())
            {
                var ns = ctx.Notifications;
                foreach (var notification in ns)
                {
                    notification.ProcessingResult = null;
                }
                return ctx.SaveChanges() > 0;
            }
        }

        public bool AddNotifcation(Notification notification)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                ctx.Notifications.Add(notification);
                return ctx.SaveChanges() > 0;
            }
        }

        public void AddSubscriptionAttempt(string username, bool success, string message, DateTime attemptTime)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                ctx.SubscriptionAttempts.Add(new SubscriptionAttempt
                {
                    UserName = username,
                    Message = message,
                    AttemptTime = attemptTime
                });
                ctx.SaveChanges();
            }
        }

        public void UpdateSubscriptionExpiry(string subscriptionId, DateTime subscriptionExpiryUtc)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                Subscription pr = new Subscription()
                {
                    Id = subscriptionId,
                };

                ctx.Subscriptions.Attach(pr);
                pr.ExpirationDateTime = subscriptionExpiryUtc;

                ctx.SaveChanges();
            }
        }

        public string GetUsernameFromSubscription(string subscriptionId)
        {
            using (var ctx = new AbsentiaDbContext())
            {
                var user = ctx.Users.SingleOrDefault(u => u.Subscriptions.Any(x => x.Id.Equals(subscriptionId)));
                return user?.UserName;
            }
        }
    }
}
