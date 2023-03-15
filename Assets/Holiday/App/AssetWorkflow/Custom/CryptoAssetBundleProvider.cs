using System.ComponentModel;
using Extreal.Integration.AssetWorkflow.Addressables.Custom.ResourceProviders;

namespace Extreal.SampleApp.Holiday.App.AssetWorkflow.Custom
{
    [DisplayName("Crypto AssetBundle Provider")]
    public class CryptoAssetBundleProvider : CryptoAssetBundleProviderBase
    {
        public override ICryptoStreamFactory CryptoStreamFactory => new CryptoStreamFactory();
    }
}
