﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.Hive.Plugin;

namespace TestPlugin
{
    /* Factory class for Plugin */
    public class PluginFactory : IPluginFactory
    {
        public IGamePlugin Create(IPluginHost gameHost, string pluginName,
                                  Dictionary<string, string> config, out string errorMsg)
        {
            // Create new instance of Test PLugin 
            var plugin = new RaiseEventTestPlugin();
            if (plugin.SetupInstance(gameHost, config, out errorMsg))
            {
                return plugin;
            }
            return null;
        }
    }
}
