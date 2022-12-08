using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.MultiplayCommon
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
