using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
