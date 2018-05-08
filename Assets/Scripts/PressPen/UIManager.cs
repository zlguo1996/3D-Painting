using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public PressPen press_pen;

	public Text stat_text;

	public Text pressure_text;
	public Slider pressure_slider;

	private string status_string{
		get { return _status_string;}
		set {
			stat_text.text = value;
			_status_string = value;
		}
	}
	private string _status_string;

	private UnityAction stat_change_action;
	private UnityAction pressure_chage_action;

	// Use this for initialization
	void Start ()
	{
		status_string = "None";

		stat_change_action += on_stat_change;
		press_pen.dodeca_tracker_thread.dodeca_tracker.statChangeEvent.AddListener (stat_change_action);

		pressure_chage_action += on_pressure_change;
		press_pen.press_measure.OnDetectPressure.AddListener (pressure_chage_action);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void on_stat_change(){
		switch (press_pen.dodeca_tracker_thread.dodeca_tracker.stat) {
		case DodecaTracker.STATUS.NONE:
			status_string = "None";
			break;
		case DodecaTracker.STATUS.CALIB:
			status_string = "Calibration";
			break;
		case DodecaTracker.STATUS.TRACK:
			status_string = "Tracking";
			break;
		default:
			break;
		}
	}

	public void StartTracking(){
		press_pen.dodeca_tracker_thread.startTracking ();
	}
	public void StopTracking(){
		press_pen.dodeca_tracker_thread.stopTracking ();
	}
	public void StartCalibration(){
		press_pen.dodeca_tracker_thread.startCalibration ();
	}
	public void StopCalibration(){
		press_pen.dodeca_tracker_thread.stopCalibration ();
	}

	void on_pressure_change(){
		pressure_text.text = press_pen.press_measure.pressure.ToString();
		pressure_slider.value = (float)press_pen.press_measure.pressure;
	}
}

