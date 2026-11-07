using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nct.AspNetCore.Rewrite
{
    public class RedirectRule : IRule
    {
        private readonly TimeSpan _regexTimeout = TimeSpan.FromSeconds(1);
        public Regex InitialMatch { get; }
        public string Replacement { get; }
        public int StatusCode { get; }
        public bool AppendQuery { get; }

        private bool _isQueryMatch = false;
        public RedirectRule(string regex, string replacement, bool appendQuery, int statusCode)
        {
            if (string.IsNullOrEmpty(regex))
            {
                throw new ArgumentNullException(nameof(regex));
            }

            if (string.IsNullOrEmpty(replacement))
            {
                throw new ArgumentNullException(nameof(replacement));
            }

            InitialMatch = new Regex(regex, RegexOptions.Compiled | RegexOptions.CultureInvariant, _regexTimeout);
            Replacement = replacement;
            StatusCode = statusCode;
            _isQueryMatch = regex.Contains("\\?");
            AppendQuery = appendQuery;
        }

        public virtual void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            var path = request.Path;
            var pathBase = request.PathBase;

            Match initMatchResults;
            if (!path.HasValue)
            {
                return;
            }

            var pathStr = path.ToString();
            if (_isQueryMatch)
                pathStr += context.HttpContext.Request.QueryString;

            initMatchResults = InitialMatch.Match(pathStr.Substring(1));


            if (initMatchResults.Success)
            {
                var newPath = initMatchResults.Result(Replacement);
                var response = context.HttpContext.Response;

                response.StatusCode = StatusCode;
                context.Result = RuleResult.EndResponse;

                string encodedPath;

                if (string.IsNullOrEmpty(newPath))
                {
                    encodedPath = pathBase.HasValue ? pathBase.Value : "/";
                }
                else
                {
                    var scheme = request.Scheme;
                    var host = default(HostString);
                    var schemeSplit = newPath.IndexOf(Uri.SchemeDelimiter, StringComparison.Ordinal);
                    if (schemeSplit >= 0)
                    {
                        scheme = newPath.Substring(0, schemeSplit);
                        schemeSplit += Uri.SchemeDelimiter.Length;
                        var pathSplit = newPath.IndexOf('/', schemeSplit);

                        if (pathSplit == -1)
                        {
                            host = new HostString(newPath.Substring(schemeSplit));
                            newPath = "/";
                        }
                        else
                        {
                            host = new HostString(newPath.Substring(schemeSplit, pathSplit - schemeSplit));
                            newPath = newPath.Substring(pathSplit);
                        }
                    }

                    if (newPath[0] != '/')
                    {
                        newPath = '/' + newPath;
                    }

                    var resolvedQuery = AppendQuery ? request.QueryString : default;
                    var resolvedPath = newPath;

                    var querySplit = newPath.IndexOf('?');
                    if (querySplit >= 0)
                    {
                        if (AppendQuery)
                            resolvedQuery = request.QueryString.Add(QueryString.FromUriComponent(newPath.Substring(querySplit)));
                        resolvedPath = newPath.Substring(0, querySplit);
                    }


                    encodedPath = host.HasValue
                        ? UriHelper.BuildAbsolute(scheme, host, pathBase, resolvedPath, resolvedQuery, default)
                        : UriHelper.BuildRelative(pathBase, resolvedPath, resolvedQuery, default);
                }


                response.Headers.Location = encodedPath;

            }
        }
    }

}
