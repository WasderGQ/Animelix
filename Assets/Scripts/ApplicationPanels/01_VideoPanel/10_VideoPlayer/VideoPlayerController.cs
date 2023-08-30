using System;
using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element;
using UnityEngine;
using UnityEngine.Video;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer
{
    public class VideoPlayerController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private PlayPause _playPause;
        [SerializeField] private Mute _mute;
        [SerializeField] private FullScreen _fullScreen;
        [SerializeField] private ProgressBar _progressBar;
        public void InIt()
        {
            _playPause.InIt();
            _mute.InIt();
            _fullScreen.InIt();
            _progressBar.InIt();
            AddListener();
        }
        public void OutIt()
        {
            _playPause.OutIt();
            _mute.OutIt();
            _fullScreen.OutIt();
            _progressBar.OutIt();
            RemoveListener();
        }
        
        private void AddListener()
        {
            _videoPlayer.loopPointReached += OnLoopPointReached;
        }
        private void RemoveListener()
        {
            _videoPlayer.loopPointReached += OnLoopPointReached;
        }
        
        private void OnLoopPointReached(VideoPlayer source)
        {
            _progressBar.ResetSlideBar();
            if(_playPause.ButtonStatus)
                _playPause.ChangeToFalseStatus();
        }

        
    }
}
