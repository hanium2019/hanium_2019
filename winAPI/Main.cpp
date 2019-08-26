#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#include <time.h>

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

//LRESULT CALLBACK WndVideo(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

BOOL CALLBACK EnumWindowMinimize(HWND hwnd, LPARAM lParam);

BOOL CALLBACK EnumWindowRestore(HWND hwnd, LPARAM lParam);

BOOL CALLBACK EnumWindowCapture(HWND hwnd, LPARAM lParam);

HBITMAP WindowCapture(HWND hwnd);

HBITMAP ScreenCapture(HWND hwnd);

LPCTSTR lpszClass = _T("MainWindow");

HINSTANCE hInst;

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR lpszCmdLine, int nCmdShow) {
	HWND hwnd;
	MSG msg;
	WNDCLASS WndClass;
	hInst = hInstance;

	WndClass.style = CS_HREDRAW | CS_VREDRAW;
	WndClass.lpfnWndProc = WndProc;
	WndClass.cbClsExtra = 0;
	WndClass.cbWndExtra = 0;
	WndClass.hInstance = hInstance;
	WndClass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	WndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
	WndClass.hbrBackground = (HBRUSH)GetStockObject(GRAY_BRUSH);
	WndClass.lpszClassName = lpszClass;
	WndClass.lpszMenuName = NULL;

	RegisterClass(&WndClass);

	WndClass.lpfnWndProc = WndEmpty;
	WndClass.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
	WndClass.lpszClassName = _T("DisplayWindow");

	RegisterClass(&WndClass);

	/*WndClass.lpfnWndProc = WndVideo;
	WndClass.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
	WndClass.lpszClassName = _T("VideoWindow");

	RegisterClass(&WndClass);*/

	hwnd = CreateWindow(lpszClass, lpszClass, WS_OVERLAPPEDWINDOW,
		CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
		NULL, (HMENU)NULL, hInstance, NULL);

	ShowWindow(hwnd, nCmdShow);
	UpdateWindow(hwnd);

	while (GetMessage(&msg, NULL, 0, 0)) {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return (int)msg.wParam;
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	PAINTSTRUCT ps;
	//HANDLE hProcess;
	//DWORD pid;

	switch (iMsg) {
	case WM_CREATE:
		CreateWindow(_T("button"), _T("MAX"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 20, 100, 30,
			hwnd, (HMENU)01, hInst, NULL);
		CreateWindow(_T("button"), _T("RESTORE"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 60, 100, 30,
			hwnd, (HMENU)02, hInst, NULL);
		CreateWindow(_T("button"), _T("MIN"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 100, 100, 30,
			hwnd, (HMENU)03, hInst, NULL);
		CreateWindow(_T("button"), _T("MIN ALL"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 140, 100, 30,
			hwnd, (HMENU)04, hInst, NULL);
		CreateWindow(_T("button"), _T("RESTORE ALL"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 180, 100, 30,
			hwnd, (HMENU)05, hInst, NULL);
		CreateWindow(_T("button"), _T("CAPTURE ALL"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 220, 100, 30,
			hwnd, (HMENU)06, hInst, NULL);
		CreateWindow(_T("button"), _T("SCROLL TEST"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 260, 100, 30,
			hwnd, (HMENU)07, hInst, NULL);
		CreateWindow(_T("button"), _T("CLOSE"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 300, 100, 30,
			hwnd, (HMENU)10, hInst, NULL);
		break;
	case WM_COMMAND:
		switch (LOWORD(wParam)) {
		case 01:
			// ShowWindow(hwnd, SW_SHOWMAXIMIZED);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
			break;
		case 02:
			// ShowWindow(hwnd, SW_RESTORE);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
			break;
		case 03:
			//ShowWindow(hwnd, SW_SHOWMINIMIZED);
			//PostMessage(hwnd, WM_SHOWWINDOW, FALSE, SW_OTHERUNZOOM);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
			break;
		case 04:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowMinimize, NULL);
			break;
		case 05:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowRestore, NULL);
			break;
		case 06:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowCapture, NULL);
			//EnumWindows(ShowEnumWindow, NULL);
			// 캡션명을 입력하면 그에 따른 윈도우 호출
			//g_hwnd = FindWindow(NULL, _T("WinAPI - Microsoft Visual Studio"));
			//GetClientRect(g_hwnd, &rct); // 현재 윈도우의 좌표값을 받아온다
			//CreateWindow("DisplayWindow", "DisplayWindow", WS_OVERLAPPEDWINDOW | WS_VISIBLE,
			//	CW_USEDEFAULT, CW_USEDEFAULT, rct.right - rct.left, rct.bottom - rct.top,
			//	NULL, (HMENU)NULL, hInst, NULL);
			//hBit = WindowCapture(g_hwnd);
			break;
		case 07:
			CreateWindow("DisplayWindow", "Empty Window", WS_OVERLAPPEDWINDOW | WS_VISIBLE | WS_VSCROLL,
				CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
				NULL, (HMENU)NULL, hInst, NULL);
			break;
		case 10:
			// pid = GetCurrentProcessId();
			// Process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
			// CreateRemoteThread(hProcess, 0, 0, 0, ExitProcess, 0, 0);
			// 부모가 아닌 독립적인 관계여도 닫기는 공유
			SendMessage(hwnd, WM_SYSCOMMAND, SC_CLOSE, 0);
			break;
		}
	case WM_SIZE:
		switch (wParam) {
		case SIZE_MAXIMIZED:
			SetWindowText(hwnd, "커졌다 !");
			break;
		case SIZE_MINIMIZED:
			SetWindowText(hwnd, "작아졌다 !");
			break;
		case SIZE_RESTORED:
			SetWindowText(hwnd, lpszClass);
			break;
		}
	case WM_PAINT:
		BeginPaint(hwnd, &ps);
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}

HBITMAP hBit = NULL;

HWND hScroll = NULL;

int scroll = 0;

HWND g_hwnd;

RECT rct;

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	HDC hdc, memdc;
	BITMAP bmp;
	HBITMAP hOld;
	PAINTSTRUCT ps;

	switch (iMsg) {
	case WM_CREATE:
		hScroll = CreateWindow("Scrollbar", NULL, WS_CHILD | WS_VISIBLE | SBS_VERT,
			10, 10, 20, 500, hwnd, (HMENU)0, hInst, NULL);
		SetScrollRange(hScroll, SB_CTL, 0, 100, FALSE);
		SetScrollPos(hScroll, SB_CTL, 0, FALSE);
		hBit = ScreenCapture(hwnd);
	case WM_MOUSEWHEEL:
		if ((SHORT)HIWORD(wParam) > 0) { // 휠이 앞으로 돌아갈 때
			if(scroll > 0) scroll--;
			SetScrollPos(hScroll, SB_CTL, scroll, TRUE);
			MessageBox(hwnd, _T("마우스 올림"), _T("Wheel Test"), NULL);
			break;
		}
		else if ((SHORT)HIWORD(wParam) < 0) { // 휠이 뒤로 돌아갈 때
			if (scroll < 255) scroll++;
			SetScrollPos(hScroll, SB_CTL, scroll, TRUE);
			MessageBox(hwnd, _T("마우스 내림"), _T("Wheel Test"), NULL);
			break;
		}
		break;
	case WM_PAINT:
		hdc = BeginPaint(hwnd, &ps);
		if (hBit != NULL) {
			memdc = CreateCompatibleDC(hdc);
			hOld = (HBITMAP)SelectObject(memdc, hBit);
			GetObject(hBit, sizeof(BITMAP), &bmp);
			BitBlt(hdc, 0, 0, bmp.bmWidth, bmp.bmHeight, memdc, 0, 0, SRCCOPY);
			SelectObject(memdc, hOld);
			//DeleteObject(memdc);
			DeleteDC(memdc);
		}
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		if (hBit != NULL)
			DeleteObject(hBit);
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}
/*
HWND hwndVideo;

LRESULT CALLBACK WndVideo(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	switch(iMsg) {
	case WM_LBUTTONDOWN:
		if (hwndVideo) {
			MCIWndClose(hwndVideo);
			MCIWndDestroy(hwndVideo);
			hwndVideo = 0;
		}
		hwndVideo = MCIWndCreate(hwnd, hInst, MCIWNDF_NOTIFYMODE | MCIWNDF_NOTIFYPOS, "Saturn.wmv");
		if (hwndVideo)
			MCIWndPlay(hwndVideo);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}
*/
BOOL CALLBACK EnumWindowMinimize(HWND hwnd, LPARAM lParam) {
	char caption[255];
	GetWindowText(hwnd, caption, sizeof(caption));
	if (IsWindowVisible(hwnd) && strcmp(caption, _T("MainWindow"))) { // 찾은 윈도우가 화면에 보이는 윈도우라면
		//ShowWindow(hwnd, SW_SHOWMINIMIZED);
		//PostMessage(hwnd, WM_SHOWWINDOW, FALSE, SW_OTHERUNZOOM);
		SendMessage(hwnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
	}
	return TRUE; // 계속 열거하려면 TRUE를 반환해야 한다
}

BOOL CALLBACK EnumWindowRestore(HWND hwnd, LPARAM lParam) {
	if (IsWindowVisible(hwnd)) { // 찾은 윈도우가 화면에 보이는 윈도우라면
		SendMessage(hwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
	}
	return TRUE; // 계속 열거하려면 TRUE를 반환해야 한다
}

BOOL CALLBACK EnumWindowCapture(HWND hwnd, LPARAM lParam) {
	HWND l_hwnd;
	char caption[255];
	int len = GetWindowTextLength(hwnd);

	GetWindowText(hwnd, caption, sizeof(caption));

	if (IsWindowVisible(hwnd) && (len > 0) && (!IsIconic(hwnd)) && strcmp(caption, _T("MainWindow"))
		&& strcmp(caption, _T("NVIDIA GeForce Overlay")) && strcmp(caption, _T("Microsoft Store"))
		&& strcmp(caption, _T("Program Manager"))) {
		//SetWindowPos(hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_HIDEWINDOW);
		g_hwnd = FindWindow(NULL, _T(caption));
		GetClientRect(g_hwnd, &rct); // 현재 윈도우의 좌표값을 받아온다
		l_hwnd = CreateWindow("DisplayWindow", caption, WS_OVERLAPPEDWINDOW | WS_VISIBLE,
			CW_USEDEFAULT, CW_USEDEFAULT, rct.right, rct.bottom,
			NULL, (HMENU)NULL, hInst, NULL);
		hBit = WindowCapture(g_hwnd);
		//SetWindowPos(g_hwnd, NULL, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_HIDEWINDOW);
		//SendMessage(g_hwnd, WM_SYSCOMMAND, SC_MINIMIZE, NULL);
		//ShowWindow(g_hwnd, SW_HIDE);
		//InvalidateRect(g_hwnd, NULL, FALSE);
		//UpdateWindow(l_hwnd);
		Sleep(100);
	}
	return TRUE; // 계속 열거하려면 TRUE를 반환해야 한다
}

HBITMAP WindowCapture(HWND hwnd) {
	HDC hScrdc = GetDC(hwnd);
	HDC hMemdc = CreateCompatibleDC(hScrdc); // hdc와 호환되는 memdc 생성
	HBITMAP hBitmap = NULL;
	HBITMAP hOldmap = NULL;

	// 비트맵이 남아 있으면 지운다
	if (hBitmap) DeleteObject(hBitmap);

	// hdc와 호환되는 비트맵 hBit 생성
	hBitmap = CreateCompatibleBitmap(hScrdc, rct.right, rct.bottom);
	hOldmap = (HBITMAP)SelectObject(hMemdc, hBitmap); // memdc에 hBit 형식의 그림을 그리기 위해서 사용

	BitBlt(hMemdc, 0, 0, rct.right, rct.bottom, hScrdc, 0, 0, SRCCOPY);
	//SetStretchBltMode(hScrdc, COLORONCOLOR);
	//StretchBlt(hMemdc, 0, 0, rct.right-rct.left, rct.bottom-rct.top, hScrdc, 0, 0, rct.right - rct.left, rct.bottom - rct.top, SRCCOPY);
	//PrintWindow(hwnd, hMemdc, PW_CLIENTONLY);

	SelectObject(hMemdc, hOldmap);
	ReleaseDC(hwnd, hScrdc);
	DeleteDC(hMemdc);

	return hBitmap;
}

HBITMAP ScreenCapture(HWND hwnd) {
	int ScreenWidth = GetSystemMetrics(SM_CXSCREEN);
	int ScreenHeight = GetSystemMetrics(SM_CYSCREEN);

	HDC hScrdc = CreateDC("DISPLAY", NULL, NULL, NULL);
	HDC hMemdc = CreateCompatibleDC(hScrdc);
	HBITMAP hBit = CreateCompatibleBitmap(hScrdc, ScreenWidth, ScreenHeight);
	HBITMAP hOld = (HBITMAP)SelectObject(hMemdc, hBit);

	BitBlt(hMemdc, 0, 0, ScreenWidth, ScreenHeight, hScrdc, 0, 0, SRCCOPY);

	SelectObject(hMemdc, hOld);
	ReleaseDC(hwnd, hMemdc);
	DeleteDC(hScrdc);
	InvalidateRect(hwnd, NULL, TRUE);
	return hBit;
}

/*
BOOL CALLBACK ShowEnumWindow(HWND hwnd, LPARAM lParam) {
	char caption[255];
	int len = GetWindowTextLength(hwnd);

	GetWindowText(hwnd, caption, sizeof(caption));

	if ((len > 0) && (!IsIconic(hwnd)) && strcmp(caption, _T("Program Manager"))
		&& strcmp(caption, _T("NVIDIA GeForce Overlay")) && strcmp(caption, _T("MainWindow"))) {
		SetWindowPos(hwnd, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
	}
	return TRUE;
}

void FindPID(void) {
	HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hSnapshot == INVALID_HANDLE_VALUE) {
		printf("Invalid Snapshot Handle\n");
		exit(EXIT_FAILURE);
	}
	PROCESSENTRY32 PE32; // 프로세스에 대한 정보를 담는 변수
	PE32.dwSize = sizeof(PE32); // PE32의 구조체 크기 정의
	if (Process32First(hSnapshot, &PE32)) {
		do {
			printf("%s, [%d]\n", PE32.szExeFile, PE32.th32ProcessID); // name of process, PID
		} while (Process32Next(hSnapshot, &PE32));
	}
	CloseHandle(hSnapshot);
	return;
}
*/