using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LibVLCSharp;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


///This script controls all the GUI for the VLC Unity Canvas Example
///It sets up event handlers and updates the GUI every frame
///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
public class WGQPlayerGui : MonoBehaviour
{
	//VideoPlayer
	[SerializeField]private WGQPlayer _wgqPlayer;
	//GUI Elements
	[SerializeField]private Canvas _canvas;
	[SerializeField]private GameObject _videoPlayer;
	[SerializeField]private GameObject _episodeList;
	[SerializeField]private GameObject _staticBar;
	[SerializeField]private VerticalLayoutGroup _videoPanelVerticalGroup;
	[SerializeField]private RawImage _screen;
	[SerializeField]private AspectRatioFitter _screenAspectRatioFitter;
	[SerializeField]private Button _videoPlayerGUIOpenButton;
	[SerializeField]private GameObject _videoPlayerGUI;
	[SerializeField]private Slider _seekBar;
	[SerializeField]private Button _playBarButton;
	[SerializeField]private Button _pauseBarButton;
	[SerializeField]private Button _playMainButton;
	[SerializeField]private Button _pauseMainButton;
	[SerializeField]private Button _volumeOnButton;
	[SerializeField]private Button _volumeOffButton;
	[SerializeField]private Button _forwardButton;
	[SerializeField]private Button _backwardButton;
	[SerializeField]private Button _fullScreenButton;
	[SerializeField]private Button _smallScreenButton;
	[SerializeField]private TextMeshProUGUI _timeText;
	[SerializeField]private GameObject _tracksButtonsGroup; //Group containing buttons to switch video, audio, and subtitle tracks
	[SerializeField]private Color _unselectedButtonColor; //Used for unselected track text
	[SerializeField]private Color _selectedButtonColor; //Used for selected track text
	[SerializeField]private float _uIHideTime = 3000f; //How long to wait before hiding the GUI
	[SerializeField]private long _forwardTime = 10000L; //How many milliseconds to seek forward when the forward button is pressed
	[SerializeField]private long _backwardTime = 5000L; //How many milliseconds to seek backward when the backward button is pressed
	//Configurable Options
	public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

	//State variables
	private bool _isPlaying = false; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 
	private bool _isDraggingSeekBar = false; //We advance the seek bar every frame, unless the user is dragging it
	private bool _isMuted = false;
	private bool _isFullScreen = false;
	private string _durationText;
	private float _closeGUIPanelCounter = 0;
	private bool _isCounterActive;
	///Unity wants to do everything on the main thread, but VLC events use their own thread.
	///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
	///This is not actually thread safe and should be gone soon!
	private bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
	private bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

	List<Button> _videoTracksButtons = new List<Button>();
	List<Button> _audioTracksButtons = new List<Button>();
	List<Button> _textTracksButtons = new List<Button>();
    
	void Start()
	{
		AddListener();
		SetEventHandler();
		SetSeekBarEvent();
		OnStartPrepareVideoPlayer();
		SetAspectRatioFitter();
		ResetAutoHideTime();
		SetVideoDuration();
		/*//Path Input
		_pathInputField.text = wgqPlayer.path;
		_pathGroup.SetActive(false);

		//Track Selection Buttons
		_tracksButtonsGroup.SetActive(false);
		*/

	}

	public void ResetAutoHideTime()
	{
		_isCounterActive = true;
		_closeGUIPanelCounter = 3000f;
	}

	void AutoHideGUICounter()
	{
		if(_isCounterActive && !_isDraggingSeekBar)
		{
			_closeGUIPanelCounter -= Time.deltaTime * 1000f;
			if(_closeGUIPanelCounter <= 0)
			{
				ToggleElement(_videoPlayerGUI);
				ToggleElement(_videoPlayerGUIOpenButton.gameObject);
				_isCounterActive = false;
			}
		}
	}
	private void OnStartPrepareVideoPlayer()
	{
		UpdatePlayPauseButton(_wgqPlayer.playOnAwake);
		UpdateFullScreenButton(_isFullScreen);
		UpdateVolumeOnOffButton(_wgqPlayer.muteOnAwake);
		_wgqPlayer.SetVolumeValue(maxVolume);
		_isPlaying = _wgqPlayer.playOnAwake;
	}
	
	private void SetSeekBarEvent()
	{
		//Seek Bar Events
		var seekBarEvents = _seekBar.GetComponent<EventTrigger>();

		EventTrigger.Entry seekBarPointerDown = new EventTrigger.Entry();
		seekBarPointerDown.eventID = EventTriggerType.PointerDown;
		seekBarPointerDown.callback.AddListener((data) => { _isDraggingSeekBar = true; ResetAutoHideTime();});
		seekBarEvents.triggers.Add(seekBarPointerDown);

		EventTrigger.Entry seekBarPointerUp = new EventTrigger.Entry();
		seekBarPointerUp.eventID = EventTriggerType.PointerUp;
		seekBarPointerUp.callback.AddListener((data) => { 
			_isDraggingSeekBar = false;
			_wgqPlayer.SetTime((long)((double)_wgqPlayer.Duration * _seekBar.value));
			
		});
		seekBarEvents.triggers.Add(seekBarPointerUp);
	}
	
	
	
    
	private void AddListener()
	{
		//Buttons
		_playMainButton.onClick.AddListener(() => { _wgqPlayer.Play();  ResetAutoHideTime();});
		_pauseMainButton.onClick.AddListener(() => { _wgqPlayer.Pause();  ResetAutoHideTime();});
		_pauseBarButton.onClick.AddListener(() => { _wgqPlayer.Pause();  ResetAutoHideTime();});
		_playBarButton.onClick.AddListener(() => { _wgqPlayer.Play(); ResetAutoHideTime();});
		_volumeOnButton.onClick.AddListener(() => { _wgqPlayer.SetVolumeOnOff(false); UpdateVolumeOnOffButton(false); ResetAutoHideTime();});
		_volumeOffButton.onClick.AddListener(() => { _wgqPlayer.SetVolumeOnOff(true); UpdateVolumeOnOffButton(true); ResetAutoHideTime();});
		_fullScreenButton.onClick.AddListener(() => { FullScreen(true); UpdateFullScreenButton(true); ResetAutoHideTime();});
		_smallScreenButton.onClick.AddListener(() => { FullScreen(false); UpdateFullScreenButton(false); ResetAutoHideTime();});
		_forwardButton.onClick.AddListener(() => { _wgqPlayer.Seek(_forwardTime); ResetAutoHideTime();});
		_backwardButton.onClick.AddListener(() => { _wgqPlayer.Seek(-_backwardTime); ResetAutoHideTime();});
		_videoPlayerGUIOpenButton.onClick.AddListener(() => { ToggleElement(_videoPlayerGUI);ToggleElement(_videoPlayerGUIOpenButton.gameObject); ResetAutoHideTime(); });
	}
	private async void SetVideoDuration()
	{
		TimeSpan duration = TimeSpan.FromMilliseconds(await _wgqPlayer.GetMediaDuration());
		if(duration.Hours == 0)
			_durationText = string.Format($"{duration.Minutes}.{duration.Seconds}");
		else if(duration.Hours == 0 && duration.Minutes == 0)
			_durationText = string.Format($"{duration.Seconds}");
		else if(duration.Hours == 0 && duration.Minutes == 0 && duration.Seconds == 0)
			_durationText = string.Format($"");
		else
			_durationText = string.Format($"{duration.Hours}.{duration.Minutes}.{duration.Seconds}");
	}
	private void UpdateTimeText()
	{
		TimeSpan time = TimeSpan.FromMilliseconds(_wgqPlayer.Time);
		var text = String.Empty;
		if(time.Hours == 0)
			text = string.Format($"{time.Minutes}.{time.Seconds}");
		else if(time.Hours == 0 && time.Minutes == 0)
			text = string.Format($"{time.Seconds}");
		else if(time.Hours == 0 && time.Minutes == 0 && time.Seconds == 0)
			text = string.Format($"");
		else
			text = string.Format($"{time.Hours}.{time.Minutes}.{time.Seconds}");
		_timeText.text = $"{text}/{_durationText}";
	}
	private void SetAspectRatioFitter()
	{
		//Update screen aspect ratio. Doing this every frame is probably more than is necessary.
		if(_wgqPlayer.texture != null)
			_screenAspectRatioFitter.aspectRatio = (float)_wgqPlayer.texture.width / (float)_wgqPlayer.texture.height;
	}
	void Update()
	{
		
		UpdateSeekBar();
		UpdatePlayPauseButton(_isPlaying);
		AutoHideGUICounter();
		UpdateTimeText();
		if (_shouldUpdateTracks)
		{
			//SetupTrackButtons();
			_shouldUpdateTracks = false;
		}

		if (_shouldClearTracks)
		{
			//ClearTrackButtons();
			_shouldClearTracks = false;
		}

	}

	//Show the Pause button if we are playing, or the Play button if we are paused or stopped
	void UpdatePlayPauseButton(bool playing)
	{
		_pauseBarButton.gameObject.SetActive(playing);
		_pauseMainButton.gameObject.SetActive(playing);
		_playMainButton.gameObject.SetActive(!playing);
		_playBarButton.gameObject.SetActive(!playing);
	}

	
	void FullScreen(bool isFullScreen)
	{
		if (isFullScreen)
		{
			_videoPanelVerticalGroup.enabled = false;
			_episodeList.SetActive(false);
			_staticBar.SetActive(false);
			_videoPlayer.GetComponent<AspectRatioFitter>().enabled = true;
			_videoPlayer.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
			_videoPlayer.GetComponent<AspectRatioFitter>().aspectRatio = _canvas.GetComponent<RectTransform>().sizeDelta.y / _canvas.GetComponent<RectTransform>().sizeDelta.x;
			_videoPlayer.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);	
			_videoPlayer.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
			_videoPlayer.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
			_videoPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);	
			_videoPlayer.GetComponent<RectTransform>().localPosition = new Vector3(0, -60,0);
			_videoPlayer.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
			_videoPlayer.GetComponent<RectTransform>().sizeDelta = new Vector2(_canvas.GetComponent<RectTransform>().sizeDelta.y,_canvas.GetComponent<RectTransform>().sizeDelta.x);
			SetAspectRatioFitter();
			
			//add fullscreen func.
			return;
		}
		
		_videoPlayer.GetComponent<AspectRatioFitter>().enabled = false;
		_videoPlayer.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1f);
		_videoPlayer.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
		_videoPlayer.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
		_videoPlayer.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
		_videoPlayer.GetComponent<RectTransform>().sizeDelta = new Vector2(_canvas.GetComponent<RectTransform>().sizeDelta.x,750f);
		_videoPanelVerticalGroup.enabled = true;
		_episodeList.SetActive(true);
		_staticBar.SetActive(true);
		//add smallscreen func.
	}
	void UpdateVolumeOnOffButton(bool muted)
	{
		if (!muted)
		{
			_volumeOnButton.gameObject.SetActive(false);
			_volumeOffButton.gameObject.SetActive(true);
			return;
		}
		_volumeOffButton.gameObject.SetActive(false);
		_volumeOnButton.gameObject.SetActive(true);
	}
	void UpdateFullScreenButton(bool isFullScreen)
	{
		if (isFullScreen)
		{
			_smallScreenButton.gameObject.SetActive(true);
			_fullScreenButton.gameObject.SetActive(false);
			_isFullScreen = isFullScreen;
			return;
		}
		_smallScreenButton.gameObject.SetActive(false);
		_fullScreenButton.gameObject.SetActive(true);
		_isFullScreen = isFullScreen;
	}
	//Update the position of the Seek slider to the match the VLC Player
	void UpdateSeekBar()
	{
		if(!_isDraggingSeekBar)
		{
			var duration = _wgqPlayer.Duration;
			if (duration > 0)
				_seekBar.value = (float)((double)_wgqPlayer.Time / duration);
		}
	}

	//Enable a GameObject if it is disabled, or disable it if it is enabled
	bool ToggleElement(GameObject element)
	{
		bool toggled = !element.activeInHierarchy;
		element.SetActive(toggled);
		return toggled;
	}
	
	private void SetEventHandler()
	{
		//VLC Event Handlers
		_wgqPlayer.mediaPlayer.Playing += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				//Because many Unity functions can only be used on the main thread, they will fail in VLC event handlers
				//A simple way around this is to set flag variables which cause functions to be called on the next Update
				_isPlaying = true;//Switch to the Pause button next update
				_shouldUpdateTracks = true;//Regenerate tracks next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex.ToString());
			}
		};

		_wgqPlayer.mediaPlayer.Paused += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex.ToString());
			}
		};

		_wgqPlayer.mediaPlayer.Stopped += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex.ToString());
			}
		};
		//this Muted event when muted once the sound. Event doesnt work again !!!
		_wgqPlayer.mediaPlayer.Muted += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isMuted = true;
				Debug.Log("Muted");
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Muted: \n" + ex.ToString());
			}
		};
		
		
	}
   
    
 
	


	//Create Audio, Video, and Subtitles button groups
	/*void SetupTrackButtons()
	{
		Debug.Log("SetupTrackButtons");
	//	ClearTrackButtons();
	//	SetupTrackButtonsGroup(TrackType.Video, "Video Tracks", _videoTracksButtons);
	//	SetupTrackButtonsGroup(TrackType.Audio, "Audio Tracks", _audioTracksButtons);
	//	SetupTrackButtonsGroup(TrackType.Text, "Subtitle Tracks", _textTracksButtons, true);

	}*/

	//Clear the track buttons menu
	/*void ClearTrackButtons()
	{
		var childCount = _tracksButtonsGroup.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Destroy(_tracksButtonsGroup.transform.GetChild(i).gameObject);
		}
	}*/

	
	//Create Audio, Video, or Subtitle button groups
	/*void SetupTrackButtonsGroup(TrackType type, string label, List<Button> buttonList, bool includeNone = false)
	{
		buttonList.Clear();
		var tracks = wgqPlayer.Tracks(type);
		var selected = wgqPlayer.SelectedTrack(type);

		if (tracks.Count > 0)
		{
			var newLabel = Instantiate(_trackLabelPrefab, _tracksButtonsGroup.transform);
			newLabel.GetComponentInChildren<Text>().text = label;

			for (int i = 0; i < tracks.Count; i++)
			{
				var track = tracks[i];
				var newButton = Instantiate(_trackButtonPrefab, _tracksButtonsGroup.transform).GetComponent<Button>();
				var textMeshPro = newButton.GetComponentInChildren<Text>();
				textMeshPro.text = track.Name;
				if (selected != null && track.Id == selected.Id)
					textMeshPro.color = _selectedButtonColor;
				else
					textMeshPro.color = _unselectedButtonColor;

				buttonList.Add(newButton);
				newButton.onClick.AddListener(() => {
					foreach (var button in buttonList)
						button.GetComponentInChildren<Text>().color = _unselectedButtonColor;
					textMeshPro.color = _selectedButtonColor;
					wgqPlayer.Select(track);
				});
			}
			if (includeNone)
			{
				var newButton = Instantiate(_trackButtonPrefab, _tracksButtonsGroup.transform).GetComponent<Button>();
				var textMeshPro = newButton.GetComponentInChildren<Text>();
				textMeshPro.text = "None";
				if (selected == null)
					textMeshPro.color = _selectedButtonColor;
				else
					textMeshPro.color = _unselectedButtonColor;

				buttonList.Add(newButton); 
				newButton.onClick.AddListener(() => {
					foreach (var button in buttonList)
						button.GetComponentInChildren<Text>().color = _unselectedButtonColor;
					textMeshPro.color = _selectedButtonColor;
					wgqPlayer.Unselect(type);
				});
			}

		}

	}*/
}