namespace MyCustom
{
    [System.ComponentModel.DisplayName("AES-CBC AssetBundle Provider")]
    public class AesCbcAssetBundleProvider : CryptoAssetBundleProviderBase
    {
        public override ICryptoStreamGetter CryptoStreamGetter => new AesCbcStreamGetter();
    }
}
