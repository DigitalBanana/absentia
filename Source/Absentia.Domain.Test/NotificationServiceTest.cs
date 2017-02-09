using System.Collections.Generic;
using Absentia.Model;
using Absentia.Model.Entities;
using AutoMoq;
using Moq;
using NUnit.Framework;

namespace Absentia.Domain.Test
{
    [TestFixture]
    public class NotificationServiceTest
    {

        [Test]
        public void ProcessNotification_HasNotifications_ShouldGetGraphToken()
        {
            //Arrange
            var mocker = new AutoMoqer();
            mocker.GetMock<IRepository>()
                .Setup(x => x.GetPendingNotifications())
                .Returns(new List<Notification>
                {
                    new Notification
                    {
                    }
                });
            mocker.GetMock<IAppSetting>()
                .Setup(x => x.MicrosoftGraphResource)
                .Returns("graph");

            //Act
            var sut = mocker.Resolve<NotificationService>();
            sut.ProcessNotifications();

            //Assert
            mocker.GetMock<ITokenisedHttpClientFactory>()
                .Verify(x => x.GetGraphResourceClient(), Times.Once);
        }

        [Test]
        public void ProcessNotification_HasNotifications_ShouldGetOutlookToken()
        {
            //Arrange
            var mocker = new AutoMoqer();
            mocker.GetMock<IRepository>()
                .Setup(x => x.GetPendingNotifications())
                .Returns(new List<Notification>
                {
                    new Notification
                    {
                    }
                });
            mocker.GetMock<IAppSetting>()
                .Setup(x => x.OutlookResource)
                .Returns("outlook");

            //Act
            var sut = mocker.Resolve<NotificationService>();
            sut.ProcessNotifications();

            //Assert
            mocker.GetMock<ITokenisedHttpClientFactory>()
               .Verify(x => x.GetOutlookResourceClient(), Times.Once);
        }


        [Test]
        public void ProcessNotification_HasNoNotifications_ShouldNotGetTokens()
        {
            //Arrange
            var mocker = new AutoMoqer();

            //Act
            var sut = mocker.Resolve<NotificationService>();
            sut.ProcessNotifications();

            //Assert
            mocker.GetMock<ITokenisedHttpClientFactory>()
               .Verify(x => x.GetOutlookResourceClient(), Times.Never);
            mocker.GetMock<ITokenisedHttpClientFactory>()
               .Verify(x => x.GetGraphResourceClient(), Times.Never);
        }
    }
}