﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute("LuckIndiaUI",typeof(LuckIndia.UI.Startup))]
namespace LuckIndia.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
