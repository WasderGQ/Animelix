using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element;
using UnityEngine;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer
{
    public class VideoPlayerController : MonoBehaviour
    {
        [SerializeField] private PlayPause _playPause;
        [SerializeField] private Mute _mute;
        [SerializeField] private FullScreen _fullScreen;
        [SerializeField] private ProgressBar _progressBar;
        //https://download1349.mediafire.com/bsxy9ynozfygBSztenaBmJ1j1KotI2yUWg0rdLgAGnv346FcfsPI5BVGov6zOUnpkUPyGzl8U69jQu1E2spAsg-U_Rl3bVcZ6Bz-Xd7XouSakRWgC-kBzFFMfNUEra8fQqBd4xXcaipwmtPir-QAJYKcg05M4wykqjP0nOUpnLVgQQ/5lksoaav6dhne6h/Developer+Video_1.mp4
        public void InIt()
        {
            _playPause.InIt();
            _mute.InIt();
            _fullScreen.InIt();
            _progressBar.InIt();
        }
    }
}
