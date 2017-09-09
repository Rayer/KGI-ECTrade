using System;
using Intelligence;
using Package;
using Smart;

namespace KGIECTrade
{
    public class TFComController
    {
        private readonly TaiFexCom _tfcom;

        private readonly IConfigurationProvider _configuration = new DummyConfigurationProvider();

        public TFComController()
        {
            var credential = _configuration.GetCredential();
            var server = _configuration.GetServerInfo();
            _tfcom = new TaiFexCom(server.Item1, server.Item2, "API");
            Console.WriteLine(@"Initialization for TaiFexCom version {0}", _tfcom.version);
            _tfcom.OnRcvMessage += OnRcvMessage_EventHandler;
            _tfcom.OnGetStatus += OnGetStatus_EventHandler;
            _tfcom.OnRcvServerTime += OnRcvServerTime_EventHandler;
            _tfcom.OnRecoverStatus += OnRecoverStatus_EvenHandler;
            _tfcom.LoginDirect(server.Item1, server.Item2, credential.Item1, credential.Item2, ' ');
        }

        private void OnRecoverStatus_EvenHandler(object sender, string topic, RECOVER_STATUS status, uint recovercount)
        {
            Console.WriteLine(@"OnRecoverStatus : {0}, {1}, {2}", topic, status, recovercount);
        }

        private void OnRcvServerTime_EventHandler(object sender, DateTime servertime, int connquality)
        {
            Console.WriteLine(@"OnRcvServerTime : {0}, {1}", servertime, connquality);
        }

        private void OnGetStatus_EventHandler(object sender, COM_STATUS status, byte[] msg)
        {
            Console.WriteLine(@"OnGetStatus : {0}, {1}", status, System.Text.Encoding.Default.GetString(msg));
        }

        private void OnRcvMessage_EventHandler(object sender, PackageBase package)
        {
            Console.WriteLine(@"OnRcvMessage : {0}", package);
        }
    }
}