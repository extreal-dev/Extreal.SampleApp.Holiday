using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MyCustom
{
    public abstract class CryptoAssetBundleProviderBase : ResourceProviderBase
    {
        public abstract ICryptoStreamGetter CryptoStreamGetter { get; }

        public override void Provide(ProvideHandle providerInterface)
        {
            var res = new CustomAssetBundleResource(providerInterface, CryptoStreamGetter);
            res.Fetch();
        }

        public override Type GetDefaultType(IResourceLocation location) => typeof(IAssetBundleResource);

        public override void Release(IResourceLocation location, object asset)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (asset == null)
            {
                Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", location);
                return;
            }

            if (asset is CustomAssetBundleResource bundle)
            {
                bundle.Unload();
            }
        }
    }
}
