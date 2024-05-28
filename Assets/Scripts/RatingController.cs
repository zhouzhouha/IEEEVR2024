using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;
using System.Text;
using Unity.VisualScripting;

public class RatingController : MonoBehaviour
{

    public ActionBasedController rightHandController;
    private RenderController renderControl;
    public TextMeshPro textForSliderValue;

    private MainController mainControl;
    private RenderController renderController;
    private string dataOutputDir;
    private string experimentID;
    private string savePath;
    private string pc_id;
    private int pc_index; 
    public GameObject ButtonBad;
    //public GameObject secondPressTriggerCanvas;
    //bool secondTriggerPressed = false;


    private bool isFinised = false;
    public bool FinishedRating
    {
        get => isFinised;
        private set => isFinised = value;
    }


    void Awake()
    {
        mainControl = FindObjectOfType<MainController>();
        if (mainControl == null)
        {
            Debug.LogError("Can not get a valid object of MainController!");
        }

        

        ButtonBad = GameObject.Find("ButtonBad");
        //secondPressTriggerCanvas = GameObject.Find("SecondPressTrigger"); // Ensure this name matches the canvas in the hierarchy

    }



    private void OnEnable()
    {
        FinishedRating = false;
        ButtonBad.SetActive(true);
    }

    //Deactivates the old FillRect and assigns a new one.
    void Start()
    {
        renderControl = FindObjectOfType<RenderController>();
        if (renderControl == null)
        {
            Debug.LogError("Can not get a valid object of RenderController!");
        }

        dataOutputDir = mainControl.dataSaveDir;
        experimentID = string.Format("{0}_{1}{2}", mainControl.userid, mainControl.Session, ".txt");
        savePath = Path.Combine(dataOutputDir, experimentID);

        string pcdpath = renderControl.GetCurrentPcdPath();
        int pcIndex = renderControl.GetCurrentpcIndex();
        OnCurrDirPathUpdated(pcdpath, pcIndex);
        renderControl.OnCurrDirPathUpdated += this.OnCurrDirPathUpdated;

    }

    private void OnCurrDirPathUpdated(string dirpath, int curPCindex)
    {
        // get "H5_C1_R5" from "E:\DUMP\H5_C1_R5"
        string pcdName = Path.GetFileName(dirpath);
        pc_id = pcdName;
        pc_index = curPCindex;
    }


    //Update is called once per frame
    void Update()
    {

    }


    public void FinishedRatingFun(int ButtonScore)
    {
        Debug.Log("On Click()");

        if (!this.FinishedRating)
        {

            int rating_score = ButtonScore;
            string allInfo = "pc_id: " + pc_id + " " + "pc_Index" + pc_index + " " + "MOS: " + rating_score.ToString() + "\n";
            //SaveRatingScoreButton(allInfo, savePath);
            FinishedRating = true;

        }

    }
    //public void Info2PressTrigger() // need to set this public to be called when click the button
    //{
    //    if (secondTriggerPressed)
    //    {
    //        secondTriggerPressed = false;
    //    }
    //    else 
    //    {
    //        secondPressTriggerCanvas.SetActive(secondTriggerPressed);

    //    }
    //}




    public void SaveRatingScoreButton(string strs, string path)
    {

        if (!File.Exists(path))
        {
            FileStream fs = File.Create(path);
            fs.Dispose();
        }

        using (StreamWriter stream = new StreamWriter(path, true))
        {
            stream.WriteLine(strs);
        }


    }



}


