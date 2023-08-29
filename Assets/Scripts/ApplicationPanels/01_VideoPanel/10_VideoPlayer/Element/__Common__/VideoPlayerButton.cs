using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__
{
    public abstract class VideoPlayerButton : MonoBehaviour, IVideoButton
    {
        [SerializeField]protected VideoPlayer _videoPlayer;
        protected virtual Sprite _trueImage { get; set; }
        protected virtual Sprite _falseImage { get; set; }
         protected virtual Button _myButton {get;  set;}
         
         protected virtual Image _image {get;  set;}
         protected bool _buttonStatus {get;  set;}
        public virtual void InIt()
        {
       
        }
    
        protected virtual void AddListener(Button button, UnityAction OnClick)
        {
            button.onClick.AddListener(OnClick);
        }
        protected virtual void OnClick()
        {
        
        }
        protected virtual void TrueStatus()
        {
            _buttonStatus = true;
        }
        protected virtual void FalseStatus()
        {
            _buttonStatus = true;
        }
    
    }
}
