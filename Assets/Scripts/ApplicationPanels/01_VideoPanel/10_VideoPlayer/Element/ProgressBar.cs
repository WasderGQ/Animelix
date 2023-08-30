using System;
using System.Collections;
using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element
{
    public class ProgressBar : VideoPlayerButton ,IEndDragHandler,IBeginDragHandler
    {
        [SerializeField]private PlayPause _playPause;
        [field:SerializeField] protected override Sprite _trueImage { get; set; }
        [field:SerializeField] protected override Sprite _falseImage { get; set; }
        [SerializeField] private Slider _myButtonSlider;
        [field:SerializeField] protected override Image _image {get; set;}
        [field:SerializeField] public override bool ButtonStatus { get; protected set; }
        
        public override void InIt()
        {
            SetSliderMaxValue((float)_videoPlayer.length);
            StartCoroutine(ProgressBarUpdate());
        }
        
        private void SetSliderMaxValue(float videoTimeValue)
        {
            _myButtonSlider.maxValue = videoTimeValue;
        }
        private void OnDragEnd(float value)
        {
            if (ButtonStatus)
            {
                _videoPlayer.time = value;
            }
        }
        public void ResetSlideBar()
        {
            _myButtonSlider.value = 0;
        }
        public IEnumerator ProgressBarUpdate()
        {
            ButtonStatus = false;
            while(_playPause.ButtonStatus && !ButtonStatus && _videoPlayer.time < _videoPlayer.length)
            {
                yield return new WaitForSeconds(0.1f);
                _myButtonSlider.value = (float)_videoPlayer.time;
            }
            
            yield return null;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd(eventData.pointerDrag.GetComponent<Slider>().value);
            StartCoroutine(ProgressBarUpdate());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ButtonStatus = true;
        }
        
        
    }
}
