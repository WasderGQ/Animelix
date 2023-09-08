using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LibVLCSharp;
using UnityEngine.Serialization;


///This script controls all the GUI for the VLC Unity Canvas Example
///It sets up event handlers and updates the GUI every frame
///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
public class WGQPlayerGui : MonoBehaviour
{
	public WGQPlayer wgqPlayer;

	//GUI Elements
	[SerializeField]private RawImage _screen;
	[SerializeField]private AspectRatioFitter _screenAspectRatioFitter;
	[SerializeField]private Slider _seekBar;
	[SerializeField]private Button _playBarButton;
	[SerializeField]private Button _pauseBarButton;
	[SerializeField]private Button _playMainButton;
	[SerializeField]private Button _pauseMainButton;
	[SerializeField]private Button _stopButton;
	[SerializeField]private Button _pathButton;
	[SerializeField]private Button _tracksButton;
	[SerializeField]private Button _volumeButton;
	[SerializeField]private Button _forwardButton;
	[SerializeField]private Button _backwardButton;
	[SerializeField]private Button _fullScreenButton;
	[SerializeField]private Button _smallScreenButton;
	[SerializeField]private GameObject _pathGroup; //Group containing pathInputField and openButton
	[SerializeField]private InputField _pathInputField;
	[SerializeField]private Button _openButton;
	[SerializeField]private GameObject _tracksButtonsGroup; //Group containing buttons to switch video, audio, and subtitle tracks
	[SerializeField]private Slider _volumeBar;
	[SerializeField]private GameObject _trackButtonPrefab;
	[SerializeField]private GameObject _trackLabelPrefab;
	[SerializeField]private Color _unselectedButtonColor; //Used for unselected track text
	[SerializeField]private Color _selectedButtonColor; //Used for selected track text
	
	//Configurable Options
	public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

	//State variables
	bool _isPlaying = false; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 
	bool _isDraggingSeekBar = false; //We advance the seek bar every frame, unless the user is dragging it

	///Unity wants to do everything on the main thread, but VLC events use their own thread.
	///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
	///This is not actually thread safe and should be gone soon!
	bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
	bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

	List<Button> _videoTracksButtons = new List<Button>();
	List<Button> _audioTracksButtons = new List<Button>();
	List<Button> _textTracksButtons = new List<Button>();


	void Start()
	{
		//VLC Event Handlers
		wgqPlayer.mediaPlayer.Playing += (object sender, EventArgs e) => {
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

		wgqPlayer.mediaPlayer.Paused += (object sender, EventArgs e) => {
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

		wgqPlayer.mediaPlayer.Stopped += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
				_shouldClearTracks = true;//Clear tracks next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex.ToString());
			}
		};

		//Buttons
		_pauseBarButton.onClick.AddListener(() => { wgqPlayer.Pause(); });
		_playBarButton.onClick.AddListener(() => { wgqPlayer.Play(); });
		_stopButton.onClick.AddListener(() => { wgqPlayer.Stop(); });
		_pathButton.onClick.AddListener(() => { 
			if(ToggleElement(_pathGroup))
				_pathInputField.Select();
		});
		_tracksButton.onClick.AddListener(() => { ToggleElement(_tracksButtonsGroup); SetupTrackButtons(); });
		_volumeButton.onClick.AddListener(() => { ToggleElement(_volumeBar.gameObject); });
		//_openButton.onClick.AddListener(() => { wgqPlayer.Open(_pathInputField.text); });

		UpdatePlayPauseButton(wgqPlayer.playOnAwake);

		//Seek Bar Events
		var seekBarEvents = _seekBar.GetComponent<EventTrigger>();

		EventTrigger.Entry seekBarPointerDown = new EventTrigger.Entry();
		seekBarPointerDown.eventID = EventTriggerType.PointerDown;
		seekBarPointerDown.callback.AddListener((data) => { _isDraggingSeekBar = true; });
		seekBarEvents.triggers.Add(seekBarPointerDown);

		EventTrigger.Entry seekBarPointerUp = new EventTrigger.Entry();
		seekBarPointerUp.eventID = EventTriggerType.PointerUp;
		seekBarPointerUp.callback.AddListener((data) => { 
			_isDraggingSeekBar = false;
			wgqPlayer.SetTime((long)((double)wgqPlayer.Duration * _seekBar.value));
		});
		seekBarEvents.triggers.Add(seekBarPointerUp);

		//Path Input
		_pathInputField.text = wgqPlayer.path;
		_pathGroup.SetActive(false);

		//Track Selection Buttons
		_tracksButtonsGroup.SetActive(false);

		//Volume Bar
		_volumeBar.wholeNumbers = true;
		_volumeBar.maxValue = maxVolume; //You can go higher than 100 but you risk audio clipping
		_volumeBar.value = wgqPlayer.Volume;
		_volumeBar.onValueChanged.AddListener((data) => { wgqPlayer.SetVolume((int)_volumeBar.value);	});
		_volumeBar.gameObject.SetActive(false);

	}

	void Update()
	{
		//Update screen aspect ratio. Doing this every frame is probably more than is necessary.

		if(wgqPlayer.texture != null)
			_screenAspectRatioFitter.aspectRatio = (float)wgqPlayer.texture.width / (float)wgqPlayer.texture.height;

		UpdatePlayPauseButton(_isPlaying);

		UpdateSeekBar();

		if (_shouldUpdateTracks)
		{
			SetupTrackButtons();
			_shouldUpdateTracks = false;
		}

		if (_shouldClearTracks)
		{
			ClearTrackButtons();
			_shouldClearTracks = false;
		}

	}

	//Show the Pause button if we are playing, or the Play button if we are paused or stopped
	void UpdatePlayPauseButton(bool playing)
	{
		_pauseBarButton.gameObject.SetActive(playing);
		_playBarButton.gameObject.SetActive(!playing);
	}

	//Update the position of the Seek slider to the match the VLC Player
	void UpdateSeekBar()
	{
		if(!_isDraggingSeekBar)
		{
			var duration = wgqPlayer.Duration;
			if (duration > 0)
				_seekBar.value = (float)((double)wgqPlayer.Time / duration);
		}
	}

	//Enable a GameObject if it is disabled, or disable it if it is enabled
	bool ToggleElement(GameObject element)
	{
		bool toggled = !element.activeInHierarchy;
		element.SetActive(toggled);
		return toggled;
	}
	
	//Create Audio, Video, and Subtitles button groups
	void SetupTrackButtons()
	{
		Debug.Log("SetupTrackButtons");
		ClearTrackButtons();
	//	SetupTrackButtonsGroup(TrackType.Video, "Video Tracks", _videoTracksButtons);
	//	SetupTrackButtonsGroup(TrackType.Audio, "Audio Tracks", _audioTracksButtons);
	//	SetupTrackButtonsGroup(TrackType.Text, "Subtitle Tracks", _textTracksButtons, true);

	}

	//Clear the track buttons menu
	void ClearTrackButtons()
	{
		var childCount = _tracksButtonsGroup.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Destroy(_tracksButtonsGroup.transform.GetChild(i).gameObject);
		}
	}

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