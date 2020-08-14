using TinderBot.Models;

namespace TinderBot
{
    public interface IUserFilter
    {
        bool Filter(UserData userData);
    }
}