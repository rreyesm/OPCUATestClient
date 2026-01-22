using Opc.Ua;
using Opc.Ua.Client;
using System.Text;

namespace OPCUATestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnConnection_Click(object sender, EventArgs e)
        {
            // Replace with OPC UA Server URL
            // Create an endpoint to connect to
            string serverURL = "opc.tcp://YOOUR_IP:YOUR_PORT/";

            // Most basic configuration setup required to create session
            Opc.Ua.ApplicationConfiguration configuration = new Opc.Ua.ApplicationConfiguration();

            ClientConfiguration clientConfiguration = new ClientConfiguration();
            configuration.ClientConfiguration = clientConfiguration;
            configuration.CertificateValidator.AutoAcceptUntrustedCertificates = true; //If you set value "true" you can read values without certificates
            configuration.ApplicationName = "OPCUATestClient";

            try
            {
                //We discover the client passing configuration and URL endpoint
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
                uint sessionTimeout = 60000;

                string password = "YOUR_PASSWORD";
                byte[] bytePasseord = Encoding.UTF8.GetBytes(password);

                // The identity of the user attempting to connect. This can be anonymous as is used here,
                // or can be specified by a variety of means, including username and password, certificate,
                // or token.
                var userIdentity = new UserIdentity("YOUR_USERNAME", bytePasseord);

                // List of preferred locales
                List<string> preferredLocales = null;

                // Crear telemetry context if this exists
                // Opción 1: Use the telemetry context configuration
                var serviceMessageContext = configuration.CreateMessageContext();
                ITelemetryContext? telemetryContext = serviceMessageContext?.Telemetry;

                // Create the session
                ISessionFactory sessionFactory = new DefaultSessionFactory(telemetryContext);

                ISession session = await sessionFactory.CreateAsync(
                    configuration,
                    endpoint,
                    updateBeforeConnect,
                    checkDomain,
                    sessionName,
                    sessionTimeout,
                    userIdentity,
                    preferredLocales
                );

                // Set variable to read on session
                NodeId nodeId = new NodeId("ns=2;s=MACHINE R_Frame.OPC_Drives_db._000.MainDrive.Meter.Absolut.X"); //ns=4;i=3
                var value = await session.ReadValueAsync(nodeId);

                // Close session after we read
                if (session.Connected)
                    await session.CloseAsync();
            }
            catch
            {
                return;
            }

        }


    }
}