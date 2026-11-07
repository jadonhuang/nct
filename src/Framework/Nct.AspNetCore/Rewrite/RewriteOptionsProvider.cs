
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Nct.Ioc;
using System.Collections.Generic;

namespace Nct.AspNetCore.Rewrite
{
    public class RewriteOptionsProvider : ISingletonDependency
    {
        private readonly IConfiguration _configuration;
        public RewriteOptionsProvider(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public RewriteOptions GetOptions()
        {

            var rules = new List<RuleOption>();
            _configuration.GetSection("UrlRewrites").Bind(rules);

            if (rules.Count == 0)
                return null;


            var options = new RewriteOptions();

            foreach (var rule in rules)
            {
                if (rule.Type == ActionType.Rewrite)
                    options.Add(new RewriteRule(rule.From, rule.To, true));
                else
                    options.Add(new RedirectRule(rule.From, rule.To, rule.AppendQuery, StatusCodes.Status302Found));
            }

            return options;

        }


    }
}
