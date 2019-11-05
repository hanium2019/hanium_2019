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

public class CaptureWindowProcess : MonoBehaviour
{

    private class User32
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
    }

    

    User32.RECT[] notepadRect = new User32.RECT[10];
    User32.RECT[] msPaintRect = new User32.RECT[10];
    User32.RECT[] powerPointerRect = new User32.RECT[10];
    User32.RECT[] chromeWidgetWin1Rect = new User32.RECT[10];
    User32.RECT[] iEFrameRect = new User32.RECT[10];
    IntPtr unityHandle;
    IntPtr[] hNotepad;// = FindWindowWithClassName("notepad", ref notepadRect);
    IntPtr[] hMSPaintApp;// = FindWindowWithClassName("mspaintapp", ref notepadRect);
    IntPtr[] hPPTFrameClass;// = FindWindowWithClassName("PPTFrameClass", ref powerPointerRect);
    IntPtr[] hChromeWidgetWin1;// = FindWindowWithClassName("Chrome_WidgetWin_1", ref chromeWidgetWin1Rect);
    IntPtr[] hIEFrame;// = FindWindowWithClassName("IEFrame", ref chromeWidgetWin1Rect);

    int index = 0;

    void SetProcessForeground(IntPtr[] handle) { // 프로세스를 포어그라운드로 가져오기
        for(int i = 0; handle[i] != IntPtr.Zero; i++) {
            User32.ShowWindowAsync(handle[i], 1);
            //User32.SetWindowPos(handle[i],  -2, 0, 0, 0, 0, 0x1 | 0x2);
        }
    }

    void GetProcessHandle() // 프로세스 핸들 전체 가져오기
    { 
        hNotepad = FindWindowWithClassName("notepad", ref notepadRect);
        hMSPaintApp = FindWindowWithClassName("mspaintapp", ref msPaintRect);
        hPPTFrameClass= FindWindowWithClassName("PPTFrameClass", ref powerPointerRect);
        hChromeWidgetWin1 = FindWindowWithClassName("Chrome_WidgetWin_1", ref chromeWidgetWin1Rect);
        hIEFrame = FindWindowWithClassName("IEFrame", ref iEFrameRect);
    }

    private IntPtr[] FindWindowWithClassName(string className, ref User32.RECT[] rect) {
        IntPtr hProcessCheck = User32.FindWindow(className, null);
        IntPtr[] hProcess = new IntPtr[10];
        for(int i = 0; hProcessCheck != IntPtr.Zero; i++) {
            if ( User32.GetWindowTextLength(hProcessCheck) >= 1 ) {
                hProcess[i] = hProcessCheck;
                User32.GetWindowRect(hProcess[i], ref rect[i]);
                Debug.Log(className + i + " " + rect[i].left + " " + rect[i].right  + " " + rect[i].top + " " + rect[i].bottom );
            } else {
                i--;
            }
            hProcessCheck = User32.FindWindowEx(IntPtr.Zero, hProcessCheck, className, null);
        }
        return hProcess;
    }

    public Image CaptureScreen()
    {
        return CaptureWindow( User32.GetDesktopWindow() );
    }
    

    public System.Drawing.Bitmap CaptureWindow(IntPtr hWnd)
    {
        System.Drawing.Rectangle rctForm = System.Drawing.Rectangle.Empty;
        using (System.Drawing.Graphics grfx = System.Drawing.Graphics.FromHdc(User32.GetWindowDC(hWnd)))
        {
            rctForm = System.Drawing.Rectangle.Round(grfx.VisibleClipBounds);
        }
        System.Drawing.Bitmap pImage = new System.Drawing.Bitmap(rctForm.Width, rctForm.Height);
        System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(pImage);
        IntPtr hDC = graphics.GetHdc();
        //paint control onto graphics using provided options        

        try
        {
            User32.PrintWindow(hWnd, hDC, 0x1);
        }
        finally
        {
            graphics.ReleaseHdc(hDC);
        }

        return pImage;
    }

    public GameObject[] windowObject = new GameObject[50];
    public GameObject windowBase;

    public void CaptureWindowToFile(IntPtr handle, string filename)
    {
        Image img = CaptureWindow(handle);
        img.Save(filename);
    }

    void Capture() // 프로세스 캡쳐해서 저장하기
    {
        print("Captured");
        
        CaptureMultipleWindowsToFile(hNotepad, "Notepad", "png");
        CaptureMultipleWindowsToFile(hMSPaintApp, "MSPaint", "png");
        CaptureMultipleWindowsToFile(hChromeWidgetWin1, "ChromeWidgetWin1", "png");
        CaptureMultipleWindowsToFile(hIEFrame, "IEFrame", "png");
        CaptureMultipleWindowsToFile(hPPTFrameClass, "PowerPointer", "png");
    }

    private void CaptureMultipleWindowsToFile(IntPtr[] handle, string className, string fileFormatName) 
    {
        for(int i = 0; handle[i] != IntPtr.Zero; i++) {
            CaptureWindowToFile(handle[i], className + "_" + i + "." + fileFormatName);
        }
    }
    
    private IEnumerator captureCoroutine;
    private IntPtr hRefresh;

    public void RefreshTarget(GameObject obj) {
        string strTmp = Regex.Replace(obj.name, @"\D", ""); 
        int nTmp = int.Parse(strTmp);
        int index = 0;
        for (int i = 0; i < 10; i++) {
            if (hNotepad[i] != IntPtr.Zero) {
                if (nTmp == index) {
                    hRefresh = hNotepad[i];
                }
                index++;
            }
        }
        for (int i = 0; i < 10; i++) {
            if (hMSPaintApp[i] != IntPtr.Zero) {
                if (nTmp == index) {
                    hRefresh = hMSPaintApp[i];
                }
                index++;
            }
        }
        for (int i = 0; i < 10; i++) {
            if (hPPTFrameClass[i] != IntPtr.Zero) {
                if (nTmp == index) {
                    hRefresh = hPPTFrameClass[i];
                }
                index++;
            }
        }
        for (int i = 0; i < 10; i++) {
            if (hChromeWidgetWin1[i] != IntPtr.Zero) {
                if (nTmp == index) {
                    hRefresh = hChromeWidgetWin1[i];
                }
                index++;
            }
        }
        for (int i = 0; i < 10; i++) {
            if (hIEFrame[i] != IntPtr.Zero) {
                if (nTmp == index) {
                    hRefresh = hIEFrame[i];
                }
                index++;
            }
        }
        Debug.Log(nTmp + " " + hRefresh);
        
        captureCoroutine = Refresh(obj, nTmp);
    }

    public void RefreshStart() {
        StartCoroutine(captureCoroutine);
        Debug.Log("asdf");
    }

    public void RefreshStop() {
        if (captureCoroutine != null)
            StopCoroutine(captureCoroutine);
    }

    private IEnumerator Refresh(GameObject obj, int index) {
        while(true) {
            try {
                CaptureWindowToFile(hRefresh, "refresh.png");
            }
            catch {

            }
            windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("refresh.png");
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ShowAll() {
        for (int j = 0; j < index; j++) {
            LeanTween.alpha(windowObject[j], 0.7f, 0.4f);
        }
        Debug.Log("asdf");
    }

    public void HideAll() {
        for (int j = 0; j < index; j++) {
            LeanTween.alpha(windowObject[j],0f,0.4f);
            //LeanTween.
            
        }
        Debug.Log("fasdf");
    }


    public void MakeAll() {
        index = 0;
        //GetProcessHandle();
        //Capture();
        for (int j = 0; j < 10; j++) {
            if (hNotepad[j] != IntPtr.Zero) {
                //windowObject[index] = (GameObject) Instantiate(windowBase, new Vector3((notepadRect[j].left)*0.0004f, (notepadRect[j].top)*0.0004f, 0.5f), Quaternion.identity) as GameObject;
                windowObject[index].transform.position = new Vector3((notepadRect[j].left-400)*0.0004f, (notepadRect[j].top-150)*0.001f, 0.4f + 0.01f*(index+1));
                windowObject[index].transform.localScale = new Vector3((notepadRect[j].left-notepadRect[j].right)*0.0004f, (notepadRect[j].top-notepadRect[j].bottom)*0.0004f, 0.001f);
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("Notepad_" + j + ".png");
                index++;
            } 
        }
        for (int j = 0; j < 10; j++) {
            if (hMSPaintApp[j] != IntPtr.Zero) {
                //windowObject[index] = (GameObject) Instantiate(windowBase, new Vector3((msPaintRect[j].left)*0.0004f, (msPaintRect[j].top)*0.0004f, 0.5f), Quaternion.identity) as GameObject;
                windowObject[index].transform.position = new Vector3((msPaintRect[j].left-400)*0.001f, (msPaintRect[j].top-150)*0.001f, 0.4f + 0.01f*(index+1));
                windowObject[index].transform.localScale = new Vector3((msPaintRect[j].left-msPaintRect[j].right)*0.0004f, (msPaintRect[j].top-msPaintRect[j].bottom)*0.0004f, 0.001f); 
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("MSPaint_" + j + ".png");
                index++;
            }
        }
        for (int j = 0; j < 10; j++) {
            if (hPPTFrameClass[j] != IntPtr.Zero) {
                //windowObject[index] = (GameObject) Instantiate(windowBase, new Vector3((powerPointerRect[j].left)*0.0004f, (powerPointerRect[j].top)*0.0004f, 0.5f), Quaternion.identity) as GameObject;
                windowObject[index].transform.position = new Vector3((powerPointerRect[j].left-400)*0.001f, (powerPointerRect[j].top-150)*0.001f, 0.4f + 0.01f*(index+1));
                windowObject[index].transform.localScale = new Vector3((powerPointerRect[j].left-powerPointerRect[j].right)*0.0004f, (powerPointerRect[j].top-powerPointerRect[j].bottom)*0.0004f, 0.001f); 
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("PowerPointer_" + j + ".png");
                index++;
            }
        }
        for (int j = 0; j < 10; j++) {
            if (hChromeWidgetWin1[j] != IntPtr.Zero) {
                //windowObject[index] = (GameObject) Instantiate(windowBase, new Vector3((chromeWidgetWin1Rect[j].left)*0.0004f, (chromeWidgetWin1Rect[j].top)*0.0004f, 0.5f), Quaternion.identity) as GameObject;
                windowObject[index].transform.position = new Vector3((chromeWidgetWin1Rect[j].left-400)*0.001f, (chromeWidgetWin1Rect[j].top-150)*0.001f, 0.4f + 0.01f*(index+1));
                windowObject[index].transform.localScale = new Vector3((chromeWidgetWin1Rect[j].left-chromeWidgetWin1Rect[j].right)*0.0004f, (chromeWidgetWin1Rect[j].top-chromeWidgetWin1Rect[j].bottom)*0.0004f, 0.001f); 
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("ChromeWidgetWin1_" + j + ".png");
                index++;
            }
        }
        for (int j = 0; j < 10; j++) {
            if (hIEFrame[j] != IntPtr.Zero) {
                //windowObject[index] = (GameObject) Instantiate(windowBase, new Vector3((iEFrameRect[j].left)*0.0004f, (iEFrameRect[j].top)*0.0004f, 0.5f), Quaternion.identity) as GameObject;
                windowObject[index].transform.position = new Vector3((iEFrameRect[j].left-400)*0.001f, (iEFrameRect[j].top-150)*0.001f, 0.4f + 0.01f*(index+1));
                windowObject[index].transform.localScale = new Vector3((iEFrameRect[j].left-iEFrameRect[j].right)*0.0004f, (iEFrameRect[j].top-iEFrameRect[j].bottom)*0.0004f, 0.001f); 
                windowObject[index].GetComponent<MeshRenderer>().material.mainTexture = LoadTextureFromFile("IEFrame_" + j + ".png");
                index++;
            }
        }
        for (int j = 0; j < 30; j++) {
            if (j < index) {
                windowObject[index].GetComponent<MeshRenderer>().enabled = true;
            } else {
                windowObject[index].GetComponent<MeshRenderer>().enabled = false;
            }
        }

    }

    public static Texture2D LoadTextureFromFile(string filename)
    {
        // "Empty" texture. Will be replaced by LoadImage
        Texture2D texture = new Texture2D(4, 4);
 
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        byte[] imageData = new byte[fs.Length];
        fs.Read(imageData, 0, (int)fs.Length);
        texture.LoadImage(imageData);
 
        return texture;
    }

    void BitmapToTexture2D(Bitmap bmp) // 비트맵 변수로부터 텍스처2D 생성
    {
        MemoryStream ms= new MemoryStream();
        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var buffer = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(buffer,0,buffer.Length);
        Texture2D t = new Texture2D(1,1);
        t.LoadImage(buffer);
    }


    void Start(){
        unityHandle = User32.GetActiveWindow();
        Debug.Log(unityHandle);
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
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Capture();
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
            ShowAll();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            HideAll();
        }
    }
}