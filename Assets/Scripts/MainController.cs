using GazeMetrics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using ViveSR.anipal.Eye;
using Cwipc;
using System;

public class MainController : MonoBehaviour
{
    private int flag = 0;  // 0: render, 1: rating, 2: calibration  init state only the render is working
    private RenderController renderController;
    private RatingController ratingController;
    private CustomCalGazeMetric cusGazeMetricController;
    private PointCloudPlayback pointcloudPlayback;
    GameObject NextPointCloudHelper;
    GameObject PressTriggerCanvas;
    GameObject ThirdPressCanvas;


    [Header("Experiment setting")]
    public string userid = "001";
    public string Session = "A";
    public string dataSaveDir = @"C:\xuemei\RawData";
   

    [Header("RightHand Controller")]
    public ActionBasedController rightHandController;
    [Tooltip("The Input System Action that will go to the next stage")]
    [SerializeField] InputActionProperty m_nextStageAction;
    private float ignoreNextUntil;

    public InputActionProperty nextStageAction { get => m_nextStageAction;  }


    private void Awake()
    {

        // test dataSaveDir
        if (string.IsNullOrWhiteSpace(dataSaveDir))
        {
            Debug.LogError("dataSaveDir is empty!");
        }
       

        dataSaveDir = Path.Combine(dataSaveDir, $"user_{userid}");
        if (!System.IO.Directory.Exists(dataSaveDir))
        {
            try
            {
                Directory.CreateDirectory(dataSaveDir);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Create dataSaveDir error! [{dataSaveDir}]  {ex.Message}");
                throw;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        renderController = FindObjectOfType<RenderController>();
        ratingController = FindObjectOfType<RatingController>();
        cusGazeMetricController = FindObjectOfType<CustomCalGazeMetric>();
        pointcloudPlayback = FindObjectOfType<PointCloudPlayback>();
        NextPointCloudHelper = GameObject.Find("ShowingNextPointCloud");
        PressTriggerCanvas = GameObject.Find("SecondPressTriggerCanvas");
        ThirdPressCanvas = GameObject.Find("ThirdPressHomeCanvas");

        //// TODO
        if ( ratingController == null || cusGazeMetricController == null || pointcloudPlayback == null)
        {
            Debug.LogError("renderController == null || ratingController == null || cusGazeMetricController == null || pointcloudPlayback == null !!!");
            UnityEditor.EditorApplication.isPlaying = false;
        }

        renderController.gameObject.SetActive(false);
        pointcloudPlayback.gameObject.SetActive(true);
        ratingController.gameObject.SetActive(false);
        cusGazeMetricController.gameObject.SetActive(false);
        NextPointCloudHelper.SetActive(false);
        PressTriggerCanvas.SetActive(false);
        ThirdPressCanvas.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

#if oldcoderemovedbyJacktotestnewercode
        bool nextWasTriggered = rightHandController.selectAction.action.triggered;
#endif
        bool nextWasTriggered = m_nextStageAction.action.triggered;
        
        if ( pointcloudPlayback.isRenderFinished && flag == 0)  // the number of loops has been played 
        {
            Debug.Log("Now flag is 0 and will disable playing the Point cloud!");
            flag = 1;
            pointcloudPlayback.isRenderFinished = false;
            
        }

        else if (ratingController.FinishedRating && flag == 1)
        {
            
            renderController.SetRenderActive(false);
            pointcloudPlayback.gameObject.SetActive(false);
            ratingController.gameObject.SetActive(false);
            NextPointCloudHelper.SetActive(false);
            cusGazeMetricController.gameObject.SetActive(true);  // in this scene only user used the trigger, the eye-ball will show up
            Debug.Log("Now flag is 1 and from Rating to Error Profiling!");
            flag = 2;
        }
        // now: calib switch to next render
        // before switch to next render, need do the re-calibration
        else if (nextWasTriggered && flag == 2)  // this is guided by Jack so I used the trackpad
        {
            if (cusGazeMetricController.Finished_calibration)
            {
                ThirdPressCanvas.SetActive(false); // for the third button
                // added by xuemei.zykk, 2022-1-5, need to do the calibration again
                bool calibrationsucssful = SRanipal_Eye_v2.LaunchEyeCalibration();
                while (!calibrationsucssful)
                {
                    Debug.LogError("LaunchEyeCalibration failed!");
                    calibrationsucssful = SRanipal_Eye_v2.LaunchEyeCalibration();
                }

                Debug.Log("LaunchEyeCalibration Successuful!");
                
                renderController.RenderNext();
                cusGazeMetricController.gameObject.SetActive(false);
                ratingController.gameObject.SetActive(false);
                NextPointCloudHelper.SetActive(false);
                pointcloudPlayback.gameObject.SetActive(true);
                renderController.SetRenderActive(true); // Todo: remove the frist 800 miliseconds data
                pointcloudPlayback.Play(pointcloudPlayback.dirName);
                Debug.Log("Now flag is 2 and doing the Calibration!");
                flag = 0;
            }
                
        }
        

    }

    internal void NewSequenceHasStarted()
    {
        ignoreNextUntil = Time.realtimeSinceStartup + 10;
    }
}
