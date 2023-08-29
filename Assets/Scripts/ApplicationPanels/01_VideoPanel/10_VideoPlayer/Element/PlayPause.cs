using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element
{
    public class PlayPause : VideoPlayerButton
    {
    
        [field:SerializeField] protected override Sprite _trueImage {get;  set;}
        [field:SerializeField] protected override Sprite _falseImage {get;  set;}
        [field:SerializeField] protected override Button _myButton {get;  set;}
        [field:SerializeField] protected override Image _image {get;  set;}
        [field:SerializeField] protected bool _buttonStatus {get;  set;}

        public override void InIt()
        {
            AddListener(_myButton, OnClick);
            OnClick();
        }
    
        protected override void AddListener(Button button, UnityAction OnClick)
        {
            button.onClick.AddListener(OnClick);
        }
    
        protected override void OnClick()
        {
            if(!_buttonStatus)
            {
                TrueStatus();
            }
            else
            {
                FalseStatus();
            }
        }
    
        protected override void TrueStatus()
        {
            _videoPlayer.Play();
            _image.sprite = _trueImage;
            _buttonStatus = true;
        }
        protected virtual void FalseStatus()
        {
            _videoPlayer.Pause();
            _image.sprite = _falseImage;
            _buttonStatus = false;
        }
    
    
    
    }
}
