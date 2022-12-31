﻿using System.Collections.Generic;
using System.Linq;
using Flow.Launcher.Core.Plugin;

namespace Flow.Launcher.Plugin.PluginIndicator
{
    public class Main : IPlugin, IPluginI18n
    {
        private PluginInitContext context;

        public List<Result> Query(Query query)
        {
            var results =
                from keyword in PluginManager.NonGlobalPlugins.Keys
                let metadata = PluginManager.NonGlobalPlugins[keyword].Metadata
                where (context.API.FuzzySearch(query.Search, keyword).IsSearchPrecisionScoreMet()
                        || context.API.FuzzySearch(query.Search, metadata.Name).IsSearchPrecisionScoreMet()
                        || string.IsNullOrEmpty(query.Search)) // To list all available action keywords
                    && !metadata.Disabled
                select new Result
                {
                    Title = keyword,
                    SubTitle = string.Format(context.API.GetTranslation("flowlauncher_plugin_pluginindicator_result_subtitle"), metadata.Name),
                    Score = 100,
                    IcoPath = metadata.IcoPath,
                    AutoCompleteText = $"{keyword}{Plugin.Query.TermSeparator}",
                    Action = c =>
                    {
                        context.API.ChangeQuery($"{keyword}{Plugin.Query.TermSeparator}");
                        return false;
                    }
                };
            return results.ToList();
        }

        public void Init(PluginInitContext context)
        {
            this.context = context;
        }

        public string GetTranslatedPluginTitle()
        {
            return context.API.GetTranslation("flowlauncher_plugin_pluginindicator_plugin_name");
        }

        public string GetTranslatedPluginDescription()
        {
            return context.API.GetTranslation("flowlauncher_plugin_pluginindicator_plugin_description");
        }
    }
}
