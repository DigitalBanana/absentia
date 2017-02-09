using System;
using System.Collections.Generic;
using System.Net;
using Absentia.Model;
using Absentia.Model.Entities;
using Moq;
using NUnit.Framework;

namespace Absentia.Domain.Test
{
    [TestFixture]
    public class SubscriptionManagerTest
    {
        private static TimeSpan subscriptionnRenewalTime = new TimeSpan(0, 1, 0, 0);


        [Test]
        public void EnsureSubscribed_HasUsersWithoutSubscriptions_ShouldCheckSubscriptionsForAllUsers()
        {
            //Arrange
            var stubNotificationService = new Mock<INotificationService>();
            var stubUserRepo = new Mock<IRepository>();
            stubUserRepo.Setup(x => x.GetUsers())
                .Returns(new List<DirectoryUser>
                {
                    new DirectoryUser
                    {
                        UserName = "user1",
                        DirectoryUserId = 1
                    },
                    new DirectoryUser
                    {
                        UserName = "user2",
                        DirectoryUserId = 2
                    },
                });
            var mockSubscriptionService = new Mock<ISubscriptionService>();
            var sut = new SubscriptionManager(stubUserRepo.Object, mockSubscriptionService.Object,stubNotificationService.Object,
                subscriptionnRenewalTime);

            //Act
            sut.Process();

            //Assert
            mockSubscriptionService.Verify(x => x.AddSubscription(It.IsAny<string>()), Times.Exactly(2));

        }

        [Test]
        public void EnsureSubscribed_HasUserWithExpiringSubscriptions_ShouldCheckSubscriptionsForAllUsers()
        {
            //Arrange
            var expiringSubscriptionId = "expiring";
            var stubUserRepo = new Mock<IRepository>();
            var stubNotificationService = new Mock<INotificationService>();
            stubUserRepo.Setup(x => x.GetUsers())
                .Returns(new List<DirectoryUser>
                {
                    new DirectoryUser
                    {
                        UserName = "user1",
                        DirectoryUserId = 1,
                    },
                    new DirectoryUser
                    {
                        UserName = "expiringuser2",
                        DirectoryUserId = 2,
                        Subscriptions = new List<Subscription>
                        {
                            new Subscription
                            {
                                ExpirationDateTime = DateTime.UtcNow.AddMinutes(30),
                                Id = expiringSubscriptionId
                            }
                        }
                    }
                });
            var mockSubscriptionService = new Mock<ISubscriptionService>();
            var sut = new SubscriptionManager(stubUserRepo.Object, mockSubscriptionService.Object,
                stubNotificationService.Object,
                subscriptionnRenewalTime);

            //Act
            sut.Process();

            //Assert
            mockSubscriptionService.Verify(x => x.AddSubscription(It.IsAny<string>()), Times.Exactly(1));
            mockSubscriptionService.Verify(x => x.RenewSubscription(expiringSubscriptionId), Times.Exactly(1));

        }

        [Test]
        public void EnsureSubscribed_HasUserWithNonExpiringSubscription_ShouldCheckSubscriptionsForAllUsers()
        {
            //Arrange
            var stubNotificationService = new Mock<INotificationService>();
            var stubUserRepo = new Mock<IRepository>();
            stubUserRepo.Setup(x => x.GetUsers())
                .Returns(new List<DirectoryUser>
                {
                    new DirectoryUser
                    {
                        UserName = "user1",
                        DirectoryUserId = 1,
                         Subscriptions = new List<Subscription>
                        {
                            new Subscription
                        {
                            ExpirationDateTime = DateTime.UtcNow.AddMinutes(90)
                        }
                        }
                    }
                });
            var mockSubscriptionService = new Mock<ISubscriptionService>();
            var sut = new SubscriptionManager(stubUserRepo.Object, mockSubscriptionService.Object,stubNotificationService.Object,
                subscriptionnRenewalTime);

            //Act
            sut.Process();

            //Assert
            mockSubscriptionService.Verify(x => x.AddSubscription(It.IsAny<string>()), Times.Never);
            mockSubscriptionService.Verify(x => x.RenewSubscription(It.IsAny<string>()), Times.Never);

        }
        [Test]
        public void EnsureSubscribed_SubscriptionAttemptFailed_ShouldHandleErrorAndRecordAttempt()
        {
            //Arrange
            var stubNotificationService = new Mock<INotificationService>();
            var stubUserRepo = new Mock<IRepository>();
            stubUserRepo.Setup(x => x.GetUsers())
                .Returns(new List<DirectoryUser>
                {
                    new DirectoryUser
                    {
                        UserName = "user1",
                        DirectoryUserId = 1,
      
                    }
                });
            var stubSubscriptionService = new Mock<ISubscriptionService>();
            stubSubscriptionService.Setup(x => x.AddSubscription(It.IsAny<string>()))
                .Throws(new ApplicationException("failure"));

            var sut = new SubscriptionManager(stubUserRepo.Object, stubSubscriptionService.Object,stubNotificationService.Object,
                subscriptionnRenewalTime);

            //Act
            sut.Process();

            //Assert
            stubUserRepo.Verify(x => x.AddSubscriptionAttempt(It.IsAny<string>(), false, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);

        }

    }
}
