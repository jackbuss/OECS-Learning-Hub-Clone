using System;
using System.Text.RegularExpressions;
using Umbraco.Core.Logging;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Umbraco.Core.Media;
using Umbraco.Web.Media.EmbedProviders;
using Umbraco.Web.WebApi;

namespace TWS.ThinkBlue.Core
{
    /// <summary>
    /// A controller used for the embed dialog
    /// </summary>
    public class UnristrictedRteEmbedController : UmbracoApiController
    {
        private EmbedProvidersCollection _embedCollection;

        public UnristrictedRteEmbedController(EmbedProvidersCollection embedCollection)
        {
            _embedCollection = embedCollection ?? throw new ArgumentNullException(nameof(embedCollection));
        }

        public OEmbedResult GetEmbed(string url, int width, int height)
        {
            var result = new OEmbedResult();
            var foundMatch = false;
            IEmbedProvider matchedProvider = null;

            foreach (var provider in _embedCollection)
            {
                //Url Scheme Regex is an array of possible regex patterns to match against the URL
                foreach (var urlPattern in provider.UrlSchemeRegex)
                {
                    var regexPattern = new Regex(urlPattern, RegexOptions.IgnoreCase);
                    if (regexPattern.IsMatch(url))
                    {
                        foundMatch = true;
                        matchedProvider = provider;
                        break;
                    }
                }

                if (foundMatch)
                    break;
            }

            if (foundMatch == false)
            {
                //No matches return/ exit
                result.OEmbedStatus = OEmbedStatus.NotSupported;
                return result;
            }

            try
            {
                result.SupportsDimensions = true;
                result.Markup = matchedProvider.GetMarkup(url, width, height);
                result.OEmbedStatus = OEmbedStatus.Success;
            }
            catch (Exception ex)
            {
                Logger.Error<UnristrictedRteEmbedController>(ex, "Error embedding url {Url} - width: {Width} height: {Height}", url, width, height);
                result.OEmbedStatus = OEmbedStatus.Error;
            }

            return result;
        }
    }

    public class MarkupFinder
    {
        public string OEmbedStatus { get; set; }
        public bool SupportsDimensions { get; set; }
        public string Markup { get; set; }
    }
}