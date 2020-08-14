using TinderBot.Models;

namespace TinderBot
{
    public class UserNonFilter : IUserFilter
    {
        public bool Filter(UserData userData) => true;
    }
}