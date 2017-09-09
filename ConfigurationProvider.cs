using System;

namespace KGIECTrade
{
    public interface IConfigurationProvider
    {
        Tuple<string, string> GetCredential();
        Tuple<string, ushort> GetServerInfo();
    }
    
    public class DefaultConfigurationProvider : IConfigurationProvider {
        public Tuple<string, string> GetCredential()
        {
            throw new NotImplementedException();
        }

        public Tuple<string, ushort> GetServerInfo()
        {
            throw new NotImplementedException();
        }
    }

    public class DummyConfigurationProvider : IConfigurationProvider
    {
        private const string KgiUrl = "tradeapi.kgi.com.tw";

        public Tuple<string, string> GetCredential()
        {
            return Tuple.Create("rayer", "aaa1234567");
        }

        public Tuple<string, ushort> GetServerInfo()
        {
            return Tuple.Create(KgiUrl, (ushort)8000);
        }
    }
        
}