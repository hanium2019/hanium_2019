using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Test : MonoBehaviour
{
    [DllImport("DLLTest.dll")]
    public static extern delegate IntPtr WndProc(IntPtr hwnd, uint iMsg, int wParam, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate IntPtr WndEmpty(IntPtr hwnd, uint iMsg, int wParam, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate IntPtr WndScroll(IntPtr hwnd, uint iMsg, int wParam, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate bool EnumWindowMinimize(IntPtr hwnd, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate bool EnumWindowRestore(IntPtr hwnd, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate bool EnumWindowCapture(IntPtr hwnd, int lParam);

    [DllImport("DLLTest.dll")]
    public static extern delegate PVOID WindowCapture(IntPtr hwnd);

    // 뭔지 잘모르겠다
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]

   struct WNDCLASS
    {
        public uint style;
        public IntPtr lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;
    }

    [DllImport("user32.dll")]
    static extern System.UInt16 RegisterClass([In] ref WNDCLASS lpWndClass);

    [DllImport("user32.dll")]
    static extern IntPtr CreateWindow(UInt32 dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpszClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpszWindowname,
        UInt32 dwStyle, Int32 x, Int32 y, Int32 nWidth, Int32 nHeight,
        IntPtr hwndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll")]
    static extern bool DefWindowProc(IntPtr hwnd, uint iMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool DestroyWindow(IntPtr hwnd);

    public CustomWindow(string name)
    {

        if (name == null) throw new Exception("class_name is null");
        if (name == String.Empty) throw new Exception("class_name is empty");

        m_wnd_proc_delegate = CustomWndProc;

        // Create WNDCLASS
        WNDCLASS wind_class = new WNDCLASS();
        wind_class.lpszClassName = name;
        wind_class.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(m_wnd_proc_delegate);

        UInt16 class_atom = RegisterClass(ref wind_class);

        int last_error = Marshal.GetLastWin32Error();

        if (class_atom == 0 && last_error != ERROR_CLASS_ALREADY_EXISTS)
        {
            throw new Exception("Could not register window class");
        }

        // Create window
        m_hwnd = CreateWindowExW(
            0,
            class_name,
            String.Empty,
            0,
            0,
            0,
            0,
            0,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero
        );
    }

    private static IntPtr CustomWndProc(IntPtr hwnd, uint iMsg, IntPtr wParam, IntPtr lParam)
    {
        return DefWindowProcW(hnd, iMsg, wParam, lParam);
    }

    private WndProc m_wnd_proc_delegate;

// Start is called before the first frame update
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
