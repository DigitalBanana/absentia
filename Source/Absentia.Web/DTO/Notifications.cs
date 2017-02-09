using System.Collections.Generic;
using Absentia.Model.Entities;

namespace Absentia.Web.DTO
{
    /// <summary>
    /// wrapper class to receive data from notification webhook
    /// </summary>
    /// <remarks>only exists to allow deserialisation of webhook payload</remarks>
    public class Notifications
    {
        public Notifications()
        {
            Value = new List<Notification>();
        }
        public List<Notification> Value { get; set; }
    }
}
