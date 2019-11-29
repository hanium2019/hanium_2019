using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using PPT = Microsoft.Office.Interop.PowerPoint;

using Microsoft.Office.Core;
public class CaptureWindowProcess : MonoBehaviour
{
    
    #region User32Importing

    public class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow (IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd,ref RECT rect);
        [DllImport("user32")]
        public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] 
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        public static extern int GetScrollPos(IntPtr hwnd, int nBar);

    }
    #endregion

/*
    const Int32 WM_VSCROLL = 0x0115;
    private const int SB_LINEUP = 0; // Scrolls one line up
    private const int SB_LINELEFT = 0;// Scrolls one cell left
    private const int SB_LINEDOWN = 1; // Scrolls one line down
    private const int SB_LINERIGHT = 1;// Scrolls one cell right
    private const int SB_PAGEUP = 2; // Scrolls one page up
    private const int SB_PAGELEFT = 2;// Scrolls one page left
    private const int SB_PAGEDOWN = 3; // Scrolls one page down
    private const int SB_PAGERIGTH = 3; // Scrolls one page right
    private const int SB_PAGETOP = 6; // Scrolls to the upper left
    private const int SB_LEFT = 6; // Scrolls to the left
    private const int SB_PAGEBOTTOM = 7; // Scrolls to the upper right
    private const int SB_RIGHT = 7; // Scrolls to the right
    private const int SB_ENDSCROLL = 8; // Ends scroll

    //private void ScrollWindow(IntPtr hwnd)
    //{
    //    User32.SendMessage(hwnd, WM_VSCROLL,  (IntPtr)SB_LINEDOWN, IntPtr.Zero);
    //    User32.SendMessage(hwnd, WM_VSCROLL, (IntPtr)SB_PAGEDOWN, IntPtr.Zero);
    //}
*/

    public User32.RECT rect_Target;
    User32.RECT[] rect_Notepad = new User32.RECT[10];
    User32.RECT[] rect_MSPaintApp = new User32.RECT[10];
    User32.RECT[] rect_PPTFrameClass = new User32.RECT[10];
    User32.RECT[] rect_ChromeWidgetWin1 = new User32.RECT[10];
    User32.RECT[] rect_IEFrame = new User32.RECT[10];
    User32.RECT[] rect_ApplicationFrameWindow = new User32.RECT[10];
    
    User32.RECT[] rect_All = new User32.RECT[50];

    public IntPtr hTarget;
    IntPtr[] hNotepad;
    IntPtr[] hMSPaintApp;
    IntPtr[] hPPTFrameClass;
    IntPtr[] hChromeWidgetWin1;
    IntPtr[] hIEFrame;
    IntPtr[] hApplicationFrameWindow;
    IntPtr[] hAll = new IntPtr[50];
    
    public HandWatcher handWatcher;
    
    public GameObject mainCamera;
    public GameObject[] windowObject = new GameObject[50];
    
    int WindowObjectIndex = 0;


    
    /// <summary> Detect specific processes and store its handles to memory </summary>                             
    /// To add processes to detect, add lines in the following format:                                             
    /// [ProcessHandleArrayName] = ProcessClassNameToHandleArray("[ProcessHandleClassName]", ref [rect_Process])   
    void GetProcessHandle() {  
        hNotepad = ProcessClassNameToHandleArray("notepad", ref rect_Notepad);
        hMSPaintApp = ProcessClassNameToHandleArray("mspaintapp", ref rect_MSPaintApp);
        hPPTFrameClass= ProcessClassNameToHandleArray("PPTFrameClass", ref rect_PPTFrameClass);
        hChromeWidgetWin1 = ProcessClassNameToHandleArray("Chrome_WidgetWin_1", ref rect_ChromeWidgetWin1);
        hIEFrame = ProcessClassNameToHandleArray("IEFrame", ref rect_IEFrame);
        hApplicationFrameWindow = ProcessClassNameToHandleArray("ApplicationFrameWindow", ref rect_ApplicationFrameWindow);
        //

    }    

    /// <summary> Create objects using all stored process handles </summary>                                           
    /// To add objects to create, add lines in the following format:                                                   
    /// ProcessHandleClassToUnityObject([ProcessHandleArrayName], [rect_Process], [flag], "[ProcessHandleClassName]")  
    /// -flag : 0 -> Handle -> Bitmap -> Texture2D,      
    ///         1 -> Handle -> Bitmap -> Image File(.PNG) -> Texture2D  
    public void MakeAll() {
        WindowObjectIndex = 0;
        ProcessHandleClassToUnityObject(hNotepad, rect_Notepad, 0, "Notepad");
        ProcessHandleClassToUnityObject(hMSPaintApp, rect_MSPaintApp, 0, "MSPaintApp");
        ProcessHandleClassToUnityObject(hPPTFrameClass, rect_PPTFrameClass, 0, "PPTFrameClass");
        ProcessHandleClassToUnityObject(hChromeWidgetWin1, rect_ChromeWidgetWin1, 0, "ChromeWidgetWin1");
        ProcessHandleClassToUnityObject(hIEFrame, rect_IEFrame, 0, "IEFrame");
        ProcessHandleClassToUnityObject(hApplicationFrameWindow, rect_ApplicationFrameWindow, 0, "WindowUICore");

        for (int j = 0; j < 30; j++) {
            if (j < WindowObjectIndex) {
                windowObject[j].GetComponent<MeshRenderer>().enabled = true;
            } else {
                windowObject[j].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    /// <summary> Create png files using process handles </summary>                                           
    /// To add png files to create, add lines in the following format:                                                   
    /// ProcessHandleClassToImagefiles([ProcessHandleArrayName], [rect_Process], "FileName", "FileType(png is recommended")  
    void AllHandleClassesToImageFiles() // Capture Process and Save to PNG file
    {
        ProcessHandleClassToImagefiles(hNotepad, rect_Notepad, "Notepad", "png");
        ProcessHandleClassToImagefiles(hMSPaintApp, rect_MSPaintApp, "MSPaintApp", "png");
        ProcessHandleClassToImagefiles(hChromeWidgetWin1, rect_ChromeWidgetWin1, "ChromeWidgetWin1", "png");
        ProcessHandleClassToImagefiles(hIEFrame, rect_IEFrame, "IEFrame", "png");
        ProcessHandleClassToImagefiles(hPPTFrameClass, rect_PPTFrameClass, "PPTFrameClass", "png");
        ProcessHandleClassToImagefiles(hApplicationFrameWindow, rect_ApplicationFrameWindow, "WindowUICore", "png");
    }

    

    /// <summary> Trace the Handle corresponding to the currently selected Object and save the handle and rect in hTarget and rect_Target respectively. </summary>
    /// To add object to detext, add lines in the following format:                                                   
    /// HandleArrayIndexing([ProcessHandleArrayName], [rect_Process], nTmp);
    int refreshTargetIndex;
    public void FindTargetHandle(GameObject obj) {
        string strTmp = Regex.Replace(obj.name, @"\D", ""); 
        int nTmp = int.Parse(strTmp);
        refreshTargetIndex = 0;
        
        hTarget = hAll[nTmp];
        rect_Target = rect_All[nTmp];
        /*
        HandleArrayIndexing(hNotepad, rect_Notepad, nTmp);
        HandleArrayIndexing(hMSPaintApp, rect_MSPaintApp, nTmp);
        HandleArrayIndexing(hPPTFrameClass, rect_PPTFrameClass, nTmp);
        HandleArrayIndexing(hChromeWidgetWin1, rect_ChromeWidgetWin1, nTmp);
        HandleArrayIndexing(hIEFrame, rect_IEFrame, nTmp);
        HandleArrayIndexing(hApplicationFrameWindow, rect_ApplicationFrameWindow, nTmp);
        */

        captureCoroutine = Refresh(obj, nTmp);
    }




    #region Internal

    /// <summary> Returns the handle and rect size of all processes that use a particular process class name. </summary>                             
    /// Use the User32.FindWindowEx function to sequentially search for a process handle using [className] 
    /// and add it to the array to be returned if there is nothing wrong with the name and rect.
    /// If there are no more processes using [className], return an array.
    private IntPtr[] ProcessClassNameToHandleArray(string className, ref User32.RECT[] rect) {
        IntPtr hProcessCheck = User32.FindWindow(className, null);
        IntPtr[] hProcess = new IntPtr[10];
        for(int i = 0; hProcessCheck != IntPtr.Zero; i++) {
            // If no process name exists, the handle is not used.
            if ( User32.GetWindowTextLength(hProcessCheck) >= 1 ) { 
                hProcess[i] = hProcessCheck;
                User32.GetWindowRect(hProcess[i], ref rect[i]);
                // If there is an error in the position or size of the process, the handle is not used.
                if ( (rect[i].left == 0 && rect[i].top == 0) || 
                      rect[i].top == 1 || rect[i].top == 8 || rect[i].top == rect[i].bottom ||
                      rect[i].left < -30000) { 
                    hProcess[i] = IntPtr.Zero;
                    i--;
                } else {
                    Debug.Log(className + i + " " + rect[i].left + " " + rect[i].right  + " " + rect[i].top + " " + rect[i].bottom );
                }
            } else {
                i--;
            }
            // Put the next process handle using [className] into hProcessCheck.
            hProcessCheck = User32.FindWindowEx(IntPtr.Zero, hProcessCheck, className, null);
        }
        return hProcess;
    }

    /// <summary> Set the texture using the process handle and the object's position and scale using the rect. </summary>                             
    /// screenWidth and screenHeight are needed to position each process window similar to the layout on the monitor.
    /// Their position or size ratio can be changed by adjusting the float numbers.
    int screenWidth = 1920;
    int screenHeight = 1080;
    private void ProcessHandleClassToUnityObject(IntPtr[] handle, User32.RECT[] rect, int flag, string className) {
        for (int j = 0; j < 10; j++) {
            if (handle[j] != IntPtr.Zero) {
                // Set position and scale
                windowObject[WindowObjectIndex].transform.position = new Vector3(   ((rect[j].left+rect[j].right)*0.5f-screenWidth/2)*0.0005f,  // x position
                                                                                   -((rect[j].top+rect[j].bottom)*0.5f-screenHeight/2)*0.0005f, // y position
                                                                                    0.4f + 0.025f*(WindowObjectIndex+1)     );                  // z position
                windowObject[WindowObjectIndex].transform.localScale = new Vector3( (rect[j].left-rect[j].right)*0.0004f,                       // x scale
                                                                                    (rect[j].top-rect[j].bottom)*0.0004f,                       // y scale
                                                                                    0.001f  );                                                  // z scale
                
                // Set texture
                if (flag == 0) 
                    windowObject[WindowObjectIndex].GetComponent<MeshRenderer>().material.mainTexture = ProcessHandleToTexture2D(handle[j], rect[j]);
                else
                    windowObject[WindowObjectIndex].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile(className + "_" +  j + ".png");
                
                hAll[WindowObjectIndex] = handle[j];
                rect_All[WindowObjectIndex] = rect[j];
                WindowObjectIndex++;
            } 
        }
    }
    
    
    /// <summary> Takes a process handle and a rect as arguments and returns a Texture2D of that size. </summary>           
    public Texture2D ProcessHandleToTexture2D(IntPtr handle, User32.RECT rect) {
        if (handle != IntPtr.Zero) {
            Bitmap bmp = ProcessHandleToBitmap(handle, rect);
            Texture2D texture = BitmapToTexture2D(bmp);
            return texture;
        }
        else {
            return null;
        }
    }

    /// <summary> Takes a process handle and a rect as arguments and returns a Bitmap of that size. </summary>     
    private System.Drawing.Bitmap ProcessHandleToBitmap(IntPtr hWnd, User32.RECT rect)
    {
        System.Drawing.Bitmap pImage = new System.Drawing.Bitmap(rect.right-rect.left, rect.bottom-rect.top);
        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(pImage);
        IntPtr hDC = graphics.GetHdc();

        try
        {
            //The PrintWindow function is more convenient than the BitBlt function, but it is resource intensive.
            //BitBlt is recommended for continuous output, such as video output.
            User32.PrintWindow(hWnd, hDC, 0x2);
        }
        finally
        {
            graphics.ReleaseHdc(hDC);
        }

        return pImage;
    }

    /// <summary> Bitmap To Texture conversion of memory record and load method </summary>
    /// To prevent crashes in continuous playback in the build version, Need to use BitBlt or manage MemoryStream
    Texture2D BitmapToTexture2D(Bitmap bmp) // 비트맵 변수로부터 텍스처2D 생성
    {
        MemoryStream ms = new MemoryStream();
        bmp.Save(ms, ImageFormat.Png); 
        var buffer = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(buffer,0,buffer.Length);
        Texture2D texture = new Texture2D(1,1);
        texture.LoadImage(buffer);

        return texture;
    }

    /// <summary> Image File To Texture conversion of file record and load method </summary>
    public static Texture2D LoadTextureFromFile(string filename)
    {
        Texture2D texture = new Texture2D(1, 1);
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        byte[] imageData = new byte[fs.Length];
        fs.Read(imageData, 0, (int)fs.Length);
        texture.LoadImage(imageData);
 
        return texture;
    }

    /// <summary> Takes an array of process handles as arguments and saves all corresponding processes as image files </summary>
    private void ProcessHandleClassToImagefiles(IntPtr[] handle, User32.RECT[] rect, string className, string fileFormatName) 
    {
        for(int i = 0; handle[i] != IntPtr.Zero; i++) {
            ProcessHandleToImageFile(handle[i], rect[i] , className + "_" + i + "." + fileFormatName);
        }
    }

    /// <summary> Takes a process handles as arguments and saves corresponding process as image file </summary>
    public void ProcessHandleToImageFile(IntPtr handle, User32.RECT rect, string filename)
    {
        Image img = ProcessHandleToBitmap(handle, rect);
        img.Save(filename);
    }

    /// <summary> For all processes with handles stored, switch to foreground if process is in background </summary>
    void SetProcessForeground(IntPtr[] handle) { 
        for(int i = 0; handle[i] != IntPtr.Zero; i++) {
            User32.ShowWindowAsync(handle[i], 1);
        }
    }

    #endregion



    

    #region Continuous playback Internal

    //MemoryStream refreshMemoryStream;
    private IEnumerator captureCoroutine;

    /// <summary> Initiate continuous playback of the process corresponding to hTarget. </summary>
    public void RefreshStart() {
        if (captureCoroutine != null)
            StartCoroutine(captureCoroutine);
    }

    /// <summary> Stop continuous playback of the process corresponding to hTarget. </summary>
    public void RefreshStop() {
        if (captureCoroutine != null)
            StopCoroutine(captureCoroutine);
    }

    /// <summary> Coroutine that continuously updates the object corresponding to hTarget </summary>
    float targetFrames = 10f;
    private IEnumerator Refresh(GameObject obj, int index) {
        while(true) {
            try {
                //ProcessHandleToImageFile(hTarget, rect_Target, "refresh.png");
                //windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("refresh.png");
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = ProcessHandleToTexture2D(hTarget, rect_Target);
            }
            catch {

            }
            yield return new WaitForSeconds(1f/targetFrames);
        }
    }

    /// <summary> Index the handle to find the desired process and save the handle and rect in hTarget and rect_Target respectively. </summary>
    void HandleArrayIndexing(IntPtr[] handle, User32.RECT[] rect, int nTmp) {
        for (int i = 0; i < 10; i++) {
            if (handle[i] != IntPtr.Zero) {
                if (nTmp == refreshTargetIndex) {
                    hTarget = handle[i];
                    rect_Target = rect[i];
                    Debug.Log(refreshTargetIndex + " Founded");
                }
                refreshTargetIndex++;
            }
        }
    }

    #endregion

    

    
    
    #region Toggle Objects State

    bool isWindowObjectsAreOn = false;
    bool isWindowObjectsAreSpreaded = false;

    /// <summary> Show all process windows </summary>
    public void ShowAll() {
        if (!isWindowObjectsAreOn) {
            isWindowObjectsAreOn = true;
            StartCoroutine("ShowWindows");
        }
    }

    /// <summary> Hide all process windows </summary>
    public void HideAll() {
        if (isWindowObjectsAreOn) {
            isWindowObjectsAreOn = false;
            StartCoroutine("HideWindows");
        }
    }

    /// <summary> Spread out all process windows </summary>
    float radius = 0.8f;
    public void SpreadAll() {
        if (!isWindowObjectsAreSpreaded) {
            isWindowObjectsAreSpreaded = true;
            StartCoroutine("SpreadWindows");
        }
    }   

    /// <summary> Collapse all process windows </summary>
    public void CollapseAll() {
        if (isWindowObjectsAreSpreaded) {
            isWindowObjectsAreSpreaded = false;
            StartCoroutine("CollapseWindows");
        }
          
    }

    /// <summary> Coroutine with a Tween that sets the transparency of all process windows to 0.7f </summary>
    private IEnumerator ShowWindows() {
        for (int j = 0; j < WindowObjectIndex; j++) {
            LeanTween.alpha(windowObject[j], 0.7f, 0.4f);
            yield return new WaitForSeconds(0.075f);
        }
        //Debug.Log("ShowCoroutine");
    }
    
    /// <summary> Coroutine with a Tween that sets the transparency of all process windows to 0f </summary>
    private IEnumerator HideWindows() {
        for (int j = 0; j < WindowObjectIndex; j++) {
            LeanTween.alpha(windowObject[j],0f,0.4f);
            yield return new WaitForSeconds(0.075f);
        }
        //Debug.Log("HideCoroutine");
    }
    
    /// <summary> Coroutine with a Tween that   </summary>
    private IEnumerator SpreadWindows() {
        float rad = 2f * Mathf.PI / (float)WindowObjectIndex;
        Quaternion quater = Quaternion.identity;
        for (int j = 0; j < WindowObjectIndex; j++) {
            quater = Quaternion.LookRotation(new Vector3(radius * Mathf.Cos(rad*j), 0, radius * Mathf.Sin(rad*j)));

            LeanTween.cancel( windowObject[j] );
            LeanTween.move( windowObject[j], 
                            new Vector3(    radius * Mathf.Cos(rad*j) ,         // x position
                                            0,          // y position
                                            radius * Mathf.Sin(rad*j)     ),     // z position
                            0.4f ).setEase(LeanTweenType.easeOutCubic);
            LeanTween.rotate( windowObject[j], quater.eulerAngles, 0.4f).setEase(LeanTweenType.easeOutCubic);
            yield return new WaitForSeconds(0.04f);
        }
    }
    
    /// <summary> Coroutine with a Tween that   </summary>
    private IEnumerator CollapseWindows() {
        for (int j = 0; j < WindowObjectIndex; j++) {
            LeanTween.cancel( windowObject[j] );
            LeanTween.move( windowObject[j], 
                            new Vector3(   ((rect_All[j].left + rect_All[j].right)*0.5f-screenWidth/2)*0.0005f,  // x position
                                          -((rect_All[j].top + rect_All[j].bottom)*0.5f-screenHeight/2)*0.0005f, // y position
                                            0.4f + 0.025f*(WindowObjectIndex+1)     ), 
                            0.4f ).setEase(LeanTweenType.easeOutCubic);
            LeanTween.rotate( windowObject[j], Vector3.zero, 0.4f).setEase(LeanTweenType.easeOutCubic);
            yield return new WaitForSeconds(0.04f);
        }     
        //Debug.Log("HideCoroutine");
    }

    /// <summary> Toggle virtual mirror </summary>
    public GameObject virtualMirror;
    bool mirrorOn = false;
    public void TurnMirror() {
        if (mirrorOn) {
            mirrorOn = false;
            virtualMirror.SetActive(false);
        } 
        else {
            mirrorOn = true;
            virtualMirror.SetActive(true);
        }
    }

    #endregion


    

    public GameObject WindowTarget;

    void Start(){
        GetProcessHandle();
        MakeAll();
        HideAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SetProcessForeground(hNotepad);
            SetProcessForeground(hMSPaintApp);
            SetProcessForeground(hChromeWidgetWin1);
            SetProcessForeground(hIEFrame);
            SetProcessForeground(hPPTFrameClass);
            SetProcessForeground(hApplicationFrameWindow);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AllHandleClassesToImageFiles();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetProcessHandle();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            MakeAll();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            handWatcher.windowState = 1;
            ShowAll();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            handWatcher.windowState = 0;
            HideAll();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            FindTargetHandle(WindowTarget);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RefreshStart();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            RefreshStop();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpreadAll();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CollapseAll();
        }
        
    }
}