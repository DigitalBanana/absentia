using System.Collections.Generic;

namespace Absentia.Model.Entities
{
    public class DirectoryUser
    {
        public DirectoryUser()
        {
            Subscriptions = new List<Subscription>();
        }
        public string UserName { get; set; }
        public int DirectoryUserId { get; set; }
        public IList<Subscription> Subscriptions { get; set; }
    }
}