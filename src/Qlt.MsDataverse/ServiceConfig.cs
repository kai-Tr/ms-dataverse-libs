using Qlt.MsDataverse.Exceptions;
using System;
using System.Linq;

namespace Qlt.MsDataverse
{
    public class ServiceConfig
    {
        private readonly string connectionString;
        private string authority = "https://login.microsoftonline.com/common";
        private string url = null;
        private string clientId = null;
        private string clientSecret = null;
        private readonly string tenantId;

        /// <summary>
        /// Constructor that parses a connection string
        /// </summary>
        /// <param name="connectionStringParam">The connection string to instantiate the configuration</param>
        public ServiceConfig(string connectionStringParam)
        {
            connectionString = connectionStringParam;

            string authorityValue = GetParameterValue("Authority");
            if (!string.IsNullOrEmpty(authorityValue))
            {
                Authority = authorityValue;
            }

            Url = GetParameterValue("Url");
            ClientId = GetParameterValue("ClientId");
            tenantId = GetParameterValue("TenantId");

            if (Guid.TryParse(GetParameterValue("CallerObjectId"), out Guid callerObjectId))
            {
                CallerObjectId = callerObjectId;
            }

            string versionValue = GetParameterValue("Version");
            if (!string.IsNullOrEmpty(versionValue))
            {
                Version = versionValue;
            }
        }

        public AuthType AuthType { get; set; }

        /// <summary>
        /// The authority to use to authorize user. 
        /// Default is 'https://login.microsoftonline.com/common'
        /// </summary>
        public string Authority
        {
            get => $"{authority}{tenantId}"; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    authority = value;
                }
                else
                {
                    throw new ServiceException("Service.Authority value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The Url to the CDS environment, i.e "https://yourorg.api.crm.dynamics.com"
        /// </summary>
        public string Url
        {
            get => url; set

            {
                if (!string.IsNullOrEmpty(value))
                {
                    url = value;
                }
                else
                {
                    throw new ServiceException("Service.Url value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The id of the application registered with Azure AD
        /// </summary>
        public string ClientId
        {
            get => clientId; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clientId = value;
                }
                else
                {
                    throw new ServiceException("Service.ClientId value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The id of the application registered with Azure AD
        /// </summary>
        public string ClientSecrect
        {
            get => clientSecret; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    clientSecret = value;
                }
                else
                {
                    throw new ServiceException("Service.ClientSecrect value cannot be null.");
                }
            }
        }

        /// <summary>
        /// The Azure AD ObjectId for the user to impersonate other users.
        /// </summary>
        public Guid CallerObjectId { get; set; }

        /// <summary>
        /// The version of the Web API to use
        /// Default is '9.1'
        /// </summary>
        public string Version { get; set; } = "9.1";

        /// <summary>
        /// The maximum number of attempts to retry a request blocked by service protection limits.
        /// Default is 3.
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// The amount of time to try completing a request before it will be cancelled.
        /// Default is 120 (2 minutes)
        /// </summary>
        public ushort TimeoutInSeconds { get; set; } = 120;

        /// <summary>
        /// Extracts a parameter value from a connection string
        /// </summary>
        /// <param name="parameter">The name of the parameter value</param>
        /// <returns></returns>
        private string GetParameterValue(string parameter)
        {
            try
            {
                string value = connectionString
                    .Split(';')
                    .FirstOrDefault(s => s.Trim().StartsWith(parameter))
                    .Split('=')[1];
                if (value.ToLower() == "null")
                {
                    return string.Empty;
                }
                return value;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
