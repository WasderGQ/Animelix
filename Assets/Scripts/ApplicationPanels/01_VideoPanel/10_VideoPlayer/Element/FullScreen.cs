using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element
{
    public class FullScreen : VideoPlayerButton
    {
        [field:SerializeField] protected override Sprite _trueImage { get;  set;}
        [field:SerializeField] protected override Sprite _falseImage { get;  set;}
        [field:SerializeField] protected override Button _myButton{  get;  set;}
        [field:SerializeField] protected override Image _image {get;  set;}
        [field:SerializeField] public override bool ButtonStatus { get; protected set; }

        public override void InIt()
        {
            base.AddListener(_myButton, OnClick);
        }
    
        
    
        protected override void OnClick()
        {
            if(!ButtonStatus)
            {
                ChangeToTrueStatus();
            }
            else
            {
                FalseStatus();
            }
        }
        public override void ChangeToTrueStatus()
        {
            //_videoPlayer.transform
            _image.sprite = _trueImage;
            ButtonStatus = true;
        }
        public virtual void FalseStatus()
        {
            _image.sprite = _falseImage;
            ButtonStatus = false;
        }




        
        
        
        
        
    }
}
