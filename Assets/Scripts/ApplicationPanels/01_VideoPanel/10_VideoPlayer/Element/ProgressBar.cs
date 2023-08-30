using System;
using System.Collections;
using ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__;
using UnityEngine;
using UnityEngine.Events;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element
{
    public class ProgressBar : VideoPlayerButton
    {
        [SerializeField]private PlayPause _playPause;
        [field:SerializeField] protected override Sprite _trueImage { get; set; }
        [field:SerializeField] protected override Sprite _falseImage { get; set; }
        [SerializeField] private Slider _myButtonSlider;
        [field:SerializeField] protected override Image _image {get; set;}
        [field:SerializeField] public override bool ButtonStatus { get; protected set; }
        
        public override void InIt()
        {
            AddListener(_myButtonSlider, OnClick);
            SetSliderMaxValue((float)_videoPlayer.length);
            StartCoroutine(ProgressBarUpdate());
        }
        
        private void SetSliderMaxValue(float videoTimeValue)
        {
            _myButtonSlider.maxValue = videoTimeValue;
        }

        private void Update()
        {
            
        }

        private void AddListener(Slider slider, UnityAction<float> OnClick)
        {
            slider.onValueChanged.AddListener(OnClick);
        }
        private void OnClick(float value)
        {
            _videoPlayer.time = value;
        }
        public void ResetSlideBar()
        {
            _myButtonSlider.value = 0;
            _videoPlayer.time = 0;
        }
        public IEnumerator ProgressBarUpdate()
        {
            while(_playPause.ButtonStatus)
            {
                _myButtonSlider.value = (float)_videoPlayer.time;
                yield return new WaitForSeconds(1f);
            }
            
            yield return null;
        }
    }
}
