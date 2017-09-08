using System;

namespace KGIECTrade
{
    public interface IConfigurationProvider
    {
        Tuple<string, string> GetCredential();
        Tuple<string, int> GetServerInfo();
    }
    
    public class DefaultConfigurationProvider : IConfigurationProvider {
        public Tuple<string, string> GetCredential()
        {
            throw new NotImplementedException();
        }

        public Tuple<string, int> GetServerInfo()
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

        public Tuple<string, int> GetServerInfo()
        {
            return Tuple.Create(KgiUrl, 8000);
        }
    }
        
}