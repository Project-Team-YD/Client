using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSMLibrary.Sound
{
    #region Channel Control
    public sealed class Channel
    {
        int _idx;

        float _volume = 1.0f;
        float _volumeMultiplier = 1.0f;

        internal GameObject _channel;

        public Channel(int i_)
        {
            _idx = i_;
            _channel = new GameObject(String.Format("MusicChan{0}", _idx));
            _channel.transform.parent = GameObject.Find("MusicManager").transform;
        }

        AudioSource GetAudioSource()
        {
            return _channel.GetComponent<AudioSource>();
        }

        public bool IsPlaying()
        {
            return GetAudioSource() != null;
        }

        public void AttachClip(string musicName_)
        {
            AudioClip audioClip = Resources.Load(String.Format("Sound/BGM/{0}", musicName_)) as AudioClip;
            if (audioClip != null)
            {
                AudioSource audioSrc = _channel.AddComponent<AudioSource>();
                audioSrc.clip = audioClip;
            }
        }

        public void Play(string musicName_, float delayTime_ = 0.0f)
        {
            AttachClip(musicName_);

            AudioSource audioSrc = _channel.GetComponent<AudioSource>();
            if (audioSrc != null)
            {
                audioSrc.ignoreListenerVolume = true;
                SetVolumeMultiplier(1.0f);
                audioSrc.loop = true;
                audioSrc.priority = 0;
                audioSrc.volume = _volume * _volumeMultiplier;
                audioSrc.PlayDelayed(delayTime_);
            }
        }

        public void Stop()
        {
            UnityEngine.Object.Destroy(_channel.GetComponent<AudioSource>());
        }

        public void SetVolumeMultiplier(float mul_)
        {
            mul_ = Mathf.Clamp(mul_, 0f, 1f);
            _volumeMultiplier = Mathf.Log(mul_ + 1.0f) / Mathf.Log(2.0f);   // Log2(mul + 1)

            SetVolume(_volume);
        }

        public void SetVolume(float vol_)
        {
            _volume = vol_;

            AudioSource audioSrc = _channel.GetComponent<AudioSource>();
            if (audioSrc != null)
            {
                audioSrc.volume = _volume * _volumeMultiplier;
            }
        }
    }
    #endregion

    public sealed class ChannelManager
    {
        List<Channel> channels = new List<Channel>();

        public ChannelManager()
        {
            for (int i = 0; i < 4; i++)
            {
                channels.Add(new Channel(i));
            }
        }

        Channel GetStoppedChannel()
        {
            for (int i = 0; i < channels.Count; i++)
            {
                if (channels[i]._channel.GetComponent<AudioSource>() == null)
                {
                    return channels[i];
                }
            }

            return null;
        }

        public Channel StealChannel()
        {
            Channel candidate = GetStoppedChannel();
            if (candidate == null)
            {
                candidate = channels[2]; //.. TO Random
            }

            return candidate;
        }
    }
}