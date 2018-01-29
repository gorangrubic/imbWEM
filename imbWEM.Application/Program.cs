using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using imbACE.Services.application;
using imbWEM.Core.console;
using imbACE.Core.application;

namespace imbWEM.Application
{
    public class Program:aceConsoleApplication<analyticConsole>
    {
        static void Main(string[] args)
        {
            var app = new Program();

            app.StartApplication(args);
        }

        public override void setAboutInformation()
        {
            appAboutInfo = new aceApplicationInfo{
                applicationVersion = "0.1v",
                software = "imbWEM Tool",
                author = "Goran Grubić",
                organization = "Faculty for Organizational Sciences, University of Belgrade",
                copyright = "Copyright (c) 2017.",
                comment = "Tool for web crawling, content mining and analysis",
                welcomeMessage = "",
                license = "GNU GPL v3.0"
            };

        }
    }
}
