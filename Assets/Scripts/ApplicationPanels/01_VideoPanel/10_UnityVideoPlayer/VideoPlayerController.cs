using System;
using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer
{
    public class VideoPlayerController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private UnityPlayPause unityPlayPause;
        [SerializeField] private UnityMute unityMute;
        [SerializeField] private UnityFullScreen unityFullScreen;
        [SerializeField] private UnityProgressBar unityProgressBar;
        public void InIt()
        {
            unityPlayPause.InIt();
            unityMute.InIt();
            unityFullScreen.InIt();
            unityProgressBar.InIt();
            AddListener();
        }
        public void OutIt()
        {
            unityPlayPause.OutIt();
            unityMute.OutIt();
            unityFullScreen.OutIt();
            unityProgressBar.OutIt();
            RemoveListener();
        }
        
        private void AddListener()
        {
            _videoPlayer.loopPointReached += OnVideoCompletion;
        }
        private void RemoveListener()
        {
            _videoPlayer.loopPointReached -= OnVideoCompletion;
        }
        
        private void OnVideoCompletion(VideoPlayer source)
        {
            
            unityProgressBar.ResetSlideBar();
            if(unityPlayPause.ButtonStatus)
                unityPlayPause.ChangeToFalseStatus();
            _videoPlayer.time = 0;
        }

        
    }
}
