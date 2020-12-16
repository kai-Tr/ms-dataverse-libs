using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qlt.MsDataverse.Consoles
{
    public class ApplicationStartup
    {
        private readonly ILogger<ApplicationStartup> _logger;
        public ApplicationStartup(ILogger<ApplicationStartup> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("Serilog logger information");
        }
    }
}
