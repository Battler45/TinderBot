using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinderBot.Models;

namespace TinderBot
{
    public class UserKeywordsFilter : IUserFilter
    {
        private List<string> Keywords { get; }
        public UserKeywordsFilter(List<string> keywords) => Keywords = keywords;

        public bool Filter(UserData userData)
        {
            return IsJobTitlesContainsKeywords(userData.user.jobs)
                   || IsBioContainsKeywords(userData.user.bio); 
        }

        private bool IsJobTitlesContainsKeywords(IEnumerable<Job> jobs)
            => jobs.Where(j => j.title != null)
                .Any(j => j.title.name.Split().Any(w => Keywords.Contains(w)));
        private bool IsBioContainsKeywords(string bio)
            => bio.Split().Any(w => Keywords.Contains(w));
    }
}
