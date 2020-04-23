using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace AspCoreRestAPI.Resources
{
    public class LocalService
    {
        private readonly IStringLocalizer _localizer;

        public LocalService(IStringLocalizerFactory localizerFactory)
        {
            var type = typeof(SharedResources);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = localizerFactory.Create("SharedResources", assemblyName.Name);
        }

        public LocalizedString GetLocalizedHtmlString(string key)
        {
            return _localizer[key];
        }
    }
}
