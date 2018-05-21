using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PenController : MonoBehaviour
{
    public PressPen press_pen;

    public Vector3 direction; // 笔尖z轴朝向（即朝向正十二面体中心）

    // active状态可以用于激活其他物体
    public bool isActivated = true;
    public int id = 0;

    public int[] pressure_level_division = new int[2]{1000, 3000};
	public enum PRESSURE_LEVEL
	{
		LEVEL_0, LEVEL_1, LEVEL_2
	}
    public PRESSURE_LEVEL pressure_level{
        get { return _pressure_level; }
        set {
            switch(value-_pressure_level){
                case 0:
                    break;
                case 1:
                    if (_pressure_level == PRESSURE_LEVEL.LEVEL_0) OnPressureExceed_01.Invoke();
                    else if (_pressure_level == PRESSURE_LEVEL.LEVEL_1) OnPressureExceed_12.Invoke();
                    OnPressureLevelChange.Invoke();
                    break;
                case -1:
                    if (_pressure_level == PRESSURE_LEVEL.LEVEL_1) OnPressureDeceed_01.Invoke();
                    else if (_pressure_level == PRESSURE_LEVEL.LEVEL_2) OnPressureDeceed_12.Invoke();
                    OnPressureLevelChange.Invoke();
                    break;
                default:
                    Debug.LogError("wrong pressure level change!");
                    break;
            }
            _pressure_level = value;
        }
    } 
	private PRESSURE_LEVEL _pressure_level = PRESSURE_LEVEL.LEVEL_0;

	public UnityEvent OnPoseChange = new UnityEvent();
	public UnityEvent OnPressureLevelChange = new UnityEvent();
	public UnityEvent OnPressureExceed_01 = new UnityEvent();
	public UnityEvent OnPressureDeceed_01 = new UnityEvent();
	public UnityEvent OnPressureExceed_12 = new UnityEvent();
	public UnityEvent OnPressureDeceed_12 = new UnityEvent();


	// Use this for initialization
	void Start () {
        press_pen.OnDetectDodeca.AddListener(_OnPressureChange);
	}
	
	// Update is called once per frame
	void Update () {
        TestUpdate();
	}

    void TestUpdate(){
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Translate(new Vector3(0.0f, 0.0f, 0.1f));
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Translate(new Vector3(0.0f, 0.0f, -0.1f));
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(new Vector3(0.0f, 0.1f, 0.0f));
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(new Vector3(0.0f, -0.1f, 0.0f));
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(new Vector3(-0.1f, 0.0f, 0.0f));
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(new Vector3(0.1f, 0.0f, 0.0f));
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pressure_level += 1;
            if (pressure_level > PRESSURE_LEVEL.LEVEL_2)
                pressure_level = PRESSURE_LEVEL.LEVEL_2;
            Debug.Log(pressure_level);
            switch (pressure_level)
            {
                case PRESSURE_LEVEL.LEVEL_1:
                    OnPressureExceed_01.Invoke();
                    break;
                case PRESSURE_LEVEL.LEVEL_2:
                    OnPressureExceed_12.Invoke();
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            pressure_level -= 1;
            if (pressure_level < PRESSURE_LEVEL.LEVEL_0)
                pressure_level = PRESSURE_LEVEL.LEVEL_0;
            Debug.Log(pressure_level);
            switch (pressure_level)
            {
                case PRESSURE_LEVEL.LEVEL_0:
                    OnPressureDeceed_01.Invoke();
                    break;
                case PRESSURE_LEVEL.LEVEL_1:
                    OnPressureDeceed_12.Invoke();
                    break;
            }
        }
    }

    void _OnPressureChange(){
        int press = press_pen.getFrame().pressure;
        if (press > pressure_level_division[1]) pressure_level = PRESSURE_LEVEL.LEVEL_2;
        else if (press > pressure_level_division[0]) pressure_level = PRESSURE_LEVEL.LEVEL_1;
        else pressure_level = PRESSURE_LEVEL.LEVEL_0;

        OnPoseChange.Invoke();

        Matrix4x4 trasformation_mat = press_pen.getFrame().rt_mat * press_pen.dodeca_tracker_thread.dodeca_tracker.pen_tip_pose;
        direction = trasformation_mat.ExtractRotation() * new Vector3(0.0f, 0.0f, 1.0f);
    }
}
