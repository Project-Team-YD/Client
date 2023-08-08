using System.Collections;
using UnityEngine;

namespace HSMLibrary.Sound
{
    public enum EFadeStatus
    {
        NONE,
        FADING_OUT,
        FADING_IN,
        BGM_OFF
    }

    public class MusicManager : MonoBehaviour
    {
        private ChannelManager channelManager = null;

        private Channel primaryChannel = null;

        private string primaryMusicName = string.Empty;

        private const float kFadeTime = 0.2f;

        private string holdedNextMusicName = null;
        private int holdedCount = 0;

        private bool isEnabled = true;

        private void Start()
        {
            channelManager = new ChannelManager();
        }

        public void ToggleMusic(bool _enabled)
        {
            if (isEnabled != _enabled)
            {
                isEnabled = _enabled;

                if (_enabled)
                {
                    Hold(false);
                }
                else
                {
                    holdedNextMusicName = primaryMusicName;
                    Stop();
                }
            }
        }

        public void Hold(bool _isHold)
        {
            if (_isHold)
            {
                holdedCount++;
            }
            else
            {
                if (holdedCount > 0)
                {
                    holdedCount--;
                }
            }

            if (holdedCount == 0 && holdedNextMusicName != null)
            {
                Play(holdedNextMusicName);
                holdedNextMusicName = null;
            }
        }

        public void Play(string _musicName)
        {
            if (_musicName.Equals(primaryMusicName))
            {
                return;
            }

            if (holdedCount > 0 || !enabled)
            {
                holdedNextMusicName = _musicName;
                return;
            }

            bool delayNextMusic = false;
            if (primaryChannel != null && primaryChannel.IsPlaying())
            {
                StartCoroutine(CoFadeOutChannel(primaryChannel, kFadeTime));
                delayNextMusic = true;
            }

            primaryMusicName = _musicName;
            primaryChannel = channelManager.StealChannel();
            primaryChannel.Play(_musicName, delayNextMusic ? kFadeTime : 0);
        }

        public void Stop()
        {
            if (primaryChannel != null && primaryChannel.IsPlaying())
            {
                primaryMusicName = null;
                StartCoroutine(CoFadeOutChannel(primaryChannel, kFadeTime));
            }
        }

        private IEnumerator CoFadeOutChannel(Channel _channel, float _fadeTime)
        {
            float fadeVolume = 1.0f;
            float fadeStartTime = Time.time;
            while (true)
            {
                float delta = (Time.time - fadeStartTime);
                fadeVolume = 1.0f - (delta / _fadeTime);

                if (fadeVolume <= 0.0f)
                {
                    fadeVolume = 0.0f;
                }

                _channel.SetVolumeMultiplier(fadeVolume);

                if (fadeVolume == 0.0f)
                {
                    yield return new WaitForEndOfFrame();

                    _channel.Stop();

                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}