using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ApplicationPanels._01_VideoPanel._10_VideoPlayer.Element.__Common__
{
    public abstract class VideoPlayerButton : MonoBehaviour, IVideoButton
    {
        [SerializeField] protected VideoPlayer _videoPlayer;
        protected virtual Sprite _trueImage { get;  set; }
        protected virtual Sprite _falseImage { get;  set; }
        protected virtual Button _myButton { get;  set; }
        protected virtual Image _image { get;  set; }
        [HideInInspector]public virtual bool ButtonStatus { get; protected set; }
        public virtual void InIt()
        {

        }
        public virtual void OutIt()
        {

        }

        protected virtual void AddListener(Button button, UnityAction OnClick)
        {
            button.onClick.AddListener(OnClick);
        }
        protected virtual void RemoveListener(Button button, UnityAction OnClick)
        {
            button.onClick.RemoveListener(OnClick);
        }
        protected virtual void OnClick()
        {

        }

        public virtual void ChangeToTrueStatus()
        {
            ButtonStatus = true;
        }

        public virtual void ChangeToFalseStatus()
        {
            ButtonStatus = true;
        }

    }
}
