using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element
{
    public class UnityPlayPause : VideoPlayerButton
    {
        [FormerlySerializedAs("_progressBar")] [SerializeField] private UnityProgressBar unityProgressBar;
        [field:SerializeField] protected override Sprite _trueImage {get; set;}
        [field:SerializeField] protected override Sprite _falseImage {get; set;}
        [field:SerializeField] protected override Button _myButton {get; set;}
        [field:SerializeField] protected override Image _image {get; set;}
        [field:SerializeField] public override bool ButtonStatus { get; protected set; }
        
        public override void InIt()
        {
            base.AddListener(_myButton, OnClick);
        }
        public override void OutIt()
        {
            base.RemoveListener(_myButton, OnClick);
        }
       
        protected override void OnClick()
        {
            if(!ButtonStatus)
            {
                ChangeToTrueStatus();
            }
            else
            {
                ChangeToFalseStatus();
            }
        }
    
        public override void ChangeToTrueStatus()
        {
            _videoPlayer.Play();
            _image.sprite = _trueImage;
            ButtonStatus = true;
            StartCoroutine(unityProgressBar.ProgressBarUpdate());
        }
        public virtual void ChangeToFalseStatus()
        {
            _videoPlayer.Pause();
            _image.sprite = _falseImage;
            ButtonStatus = false;
        }
    
       
    
    }
}
