using ApplicationPanels.__Common__;
using ApplicationPanels._01_VideoPanel._10_VideoPlayer;
using ApplicationPanels._01_VideoPanel._11_EpisodesList;
using UnityEngine;

namespace ApplicationPanels._01_VideoPanel
{
    public class VideoPanel : Panel
    {
        [SerializeField] private VideoPlayerController _videoPlayerController;
        [SerializeField] private EpisodeListController _episodeList;
        private void OnEnable()
        {
            _videoPlayerController.InIt();
        }
    }
}
