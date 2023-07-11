using System.Collections.Generic;
using System.Linq;
using Extreal.Core.Common.System;
using Extreal.P2P.Dev;
using UniRx;
using Unity.WebRTC;
using UnityEngine;

namespace Extreal.Chat.Dev
{
    public class NativeVoiceChatClient : VoiceChatClient
    {
        private readonly Dictionary<string, AudioChannel> acDict;
        private readonly AudioSource inAudio;
        private readonly AudioSource outAudio;

        public NativeVoiceChatClient(NativePeerClient peerClient, AudioSource inAudio, AudioSource outAudio)
        {
            acDict = new Dictionary<string, AudioChannel>();
            this.inAudio = inAudio;
            this.outAudio = outAudio;
            peerClient.AddPcCreateHook(CreatePc);
            peerClient.AddPcCloseHook(ClosePc);
        }

        private void CreatePc(string id, bool isOffer, RTCPeerConnection pc)
            => Observable.Start(() => Unit.Default) // To run on the main thread
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    var ac = new AudioChannel();
                    ac.SendAudio(pc, inAudio);
                    ac.ReceiveAudio(pc, outAudio);
                    acDict.Add(id, ac);
                });

        private void ClosePc(string id)
        {
            if (!acDict.ContainsKey(id))
            {
                return;
            }
            acDict[id].Dispose();
            acDict.Remove(id);
        }

        public override void ToggleMute()
        {
            inAudio.mute = !inAudio.mute;
            FireOnMuted(inAudio.mute);
        }

        public override void Clear()
        {
            Microphone.End(null);
            acDict.Values.ToList().ForEach(ac => ac.Dispose());
            acDict.Clear();
            inAudio.Stop();
            outAudio.Stop();
        }

        private class AudioChannel : DisposableBase
        {
            private MediaStream inStream;
            private AudioStreamTrack inTrack;

            private MediaStream outStream;

            protected override void ReleaseManagedResources()
            {
                inTrack.Dispose();
                inStream.Dispose();
                outStream.Dispose();
            }

            public void SendAudio(RTCPeerConnection pc, AudioSource inAudio)
            {
                if (!inAudio.isPlaying)
                {
                    inAudio.loop = true;
                    inAudio.clip =  Microphone.Start(null, true, 10, 44100);
                    while (!(Microphone.GetPosition(null) > 0))
                    {
                        // do nothing
                    }
                    inAudio.Play();
                }
                inStream = new MediaStream();
                inTrack = new AudioStreamTrack(inAudio);
                inTrack.Loopback = true;
                pc.AddTrack(inTrack, inStream);
            }

            public void ReceiveAudio(RTCPeerConnection pc, AudioSource outAudio)
            {
                outStream = new MediaStream();
                outStream.OnAddTrack += evt =>
                {
                    var outTrack = evt.Track as AudioStreamTrack;
                    outAudio.SetTrack(outTrack);
                    if (!outAudio.isPlaying)
                    {
                        outAudio.loop = true;
                        outAudio.Play();
                    }
                };
                pc.OnTrack += evt => outStream.AddTrack(evt.Track);
            }
        }
    }
}
