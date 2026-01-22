using Opc.Ua.Client;
using Opc.Ua;

namespace OPCUATestClient.Classes
{
    public class OPCUAClient
    {
        private ISession? _session;

        public bool Connected
        {
            get
            {
                if (_session != null && _session.Connected)
                {
                    return true;
                }

                return false;
            }
        }

        public async Task OPCUAClien1(string ipAddress, string port, Opc.Ua.ApplicationConfiguration? configuration = null)
        {
            if (configuration == null)
            {
                // Most basic configuration setup required to create session
                configuration = new Opc.Ua.ApplicationConfiguration();
                ClientConfiguration clientConfiguration = new ClientConfiguration();
                configuration.ClientConfiguration = clientConfiguration;
            }

            // Replace with OPC UA Server URL
            // Create an endpoint to connect to
            string serverURL = $"opc.tcp://{ipAddress}:{port}";

            try
            {
                // Discover the client passing configuration and URL endpoint
                var discoveryClient = await DiscoveryClient.CreateAsync(configuration, new Uri(serverURL));
                EndpointDescriptionCollection endpoints = await discoveryClient.GetEndpointsAsync(null);

                EndpointDescription? selectedEndpoint = endpoints.FirstOrDefault();

                if (selectedEndpoint == null)
                {
                    throw new Exception("It's no possible to get anyone endpoint from server");
                }

                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(configuration);
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);

                // Session options
                // Sets whether or not the discovery endpoint is used to update the endpoint description before connecting.
                bool updateBeforeConnect = false;

                // Sets whether or not the domain in the certificate must match the endpoint used
                bool checkDomain = false;

                // The name to assign to the session
                string sessionName = configuration.ApplicationName;

                // The session's timeout interval
                uint sessionTimeout = 30 * 60 * 1000;

                // The identity of the user attempting to connect. This can be anonymous as is used here,
                // or can be specified by a variety of means, including username and password, certificate,
                // or token.
                UserIdentity user = new UserIdentity();

                // List of preferred locales
                List<string> preferredLocales = null;

                // Crear telemetry context if this exists
                // Opción 1: Use the telemetry context configuration
                var serviceMessageContext = configuration.CreateMessageContext();
                ITelemetryContext? telemetryContext = serviceMessageContext?.Telemetry;

                // Create the session
                ISessionFactory sessionFactory = new DefaultSessionFactory(telemetryContext);

                // Create the session
                ISession session = await sessionFactory.CreateAsync(
                    configuration,
                    endpoint,
                    updateBeforeConnect,
                    checkDomain,
                    sessionName,
                    sessionTimeout,
                    user,
                    preferredLocales
                );

                // If the session was successfully created, assign it
                if (session != null && session.Connected)
                {
                    _session = session;
                }
            }
            catch
            {
                return;
            }
        }

        public async Task<ServerStatusDataType> GetServerStatus()
        {
            if (_session == null)
                throw new Exception("Session is null");

            // Get the current DataValue object for the ServerStatus node
            NodeId nodeId = new NodeId(Variables.Server_ServerStatus);
            DataValue dataValue = await _session.ReadValueAsync(nodeId);

            // Unpack the ExtensionObject that the DataValue contains, then return ServerStatusDataType object
            // that represents the current server status
            ExtensionObject extensionObject = (ExtensionObject)dataValue.Value;
            ServerStatusDataType serverStatus = (ServerStatusDataType)extensionObject.Body;

            return serverStatus;
        }

        public async Task<DataValue> GetValue(string tagAddress, ushort namespaceIndex)
        {
            if (_session == null)
                throw new Exception("Session is null");

            // To read a value, you require the node's ID. This can be either its unique integer ID, or a string
            // identifier along with the namespace which that identifier belongs to. Integer IDs are most useful
            // for acquiring nodes defined in the OPC UA standard, such as the ServerStatus node. The namespace
            // of a tag may differ depending on the OPC server being used, with KEPServer having a tag namespace
            // of 2. The only namespace that is guaranteed to remain the same is namespace 0, which contains the
            // nodes defined in the OPC UA standard.

            // Here I am using the tag address in the format "Channel.Device.Tag", along with the KEPServer namespace
            // of 2 to read the tag's value. Both of the subsequent ways of creating a NodeId are equivalent.

            NodeId nodeId = new NodeId(tagAddress, namespaceIndex);
            // NodeId nodeId = new NodeId($"ns={namespaceIndex};s={tagAddress}");

            try
            {
                return await _session.ReadValueAsync(nodeId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
