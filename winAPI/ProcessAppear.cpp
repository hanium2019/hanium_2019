#include <stdio.h>
#include <tchar.h>
#include <windows.h>

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

BOOL CALLBACK EnumWindowMinimize(HWND hwnd, LPARAM lParam);

BOOL CALLBACK EnumWindowRestore(HWND hwnd, LPARAM lParam);

BOOL CALLBACK EnumWindowCapture(HWND hwnd, LPARAM lParam);

HBITMAP WindowCapture(HWND hwnd);

LPCTSTR lpszClass = _T("MainWindow");

HINSTANCE hInst;

HWND g_hwnd;

HBITMAP hBit = NULL;

RECT rct;

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
			hwnd, (HMENU)0101, hInst, NULL);
		CreateWindow(_T("button"), _T("RESTORE"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 60, 100, 30,
			hwnd, (HMENU)0102, hInst, NULL);
		CreateWindow(_T("button"), _T("MIN"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 100, 100, 30,
			hwnd, (HMENU)0103, hInst, NULL);
		CreateWindow(_T("button"), _T("MIN ALL"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 140, 100, 30,
			hwnd, (HMENU)0104, hInst, NULL);
		CreateWindow(_T("button"), _T("RESTORE ALL"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 180, 100, 30,
			hwnd, (HMENU)0105, hInst, NULL);
		CreateWindow(_T("button"), _T("CAPTURE"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 220, 100, 30,
			hwnd, (HMENU)0106, hInst, NULL);
		CreateWindow(_T("button"), _T("CLOSE"),
			WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 260, 100, 30,
			hwnd, (HMENU)0107, hInst, NULL);
		break;
	case WM_COMMAND:
		switch (LOWORD(wParam)) {
		case 0101:
			// ShowWindow(hwnd, SW_SHOWMAXIMIZED);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
			break;
		case 0102:
			// ShowWindow(hwnd, SW_RESTORE);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_RESTORE, 0);
			break;
		case 0103:
			// ShowWindow(hwnd, SW_SHOWMINIMIZED);
			SendMessage(hwnd, WM_SYSCOMMAND, SC_MINIMIZE, 0);
			break;
		case 0104:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowMinimize, NULL);
			break;
		case 0105:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowRestore, NULL);
			break;
		case 0106:
			// 윈도우를 찾을 때마다 EnumWindowsProc 호출
			EnumWindows(EnumWindowCapture, NULL);
			//g_hwnd = FindWindow(NULL, _T("캡처 도구"));
			//GetClientRect(g_hwnd, &rct); // 현재 윈도우의 좌표값을 받아온다
			//CreateWindow("DisplayWindow", "DisplayWindow", WS_OVERLAPPEDWINDOW | WS_VISIBLE,
			//	CW_USEDEFAULT, CW_USEDEFAULT, rct.right - rct.left, rct.bottom - rct.top,
			//	NULL, (HMENU)NULL, hInst, NULL);
			//hBit = WindowCapture(g_hwnd);
			break;
		case 0107:
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

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	HDC hdc, memdc;
	BITMAP bmp;
	PAINTSTRUCT ps;

	switch (iMsg) {
	case WM_PAINT:
		hdc = BeginPaint(hwnd, &ps);
		if (hBit != NULL) {
			memdc = CreateCompatibleDC(hdc);
			SelectObject(memdc, hBit);
			GetObject(hBit, sizeof(BITMAP), &bmp);
			BitBlt(hdc, 0, 0, bmp.bmWidth, bmp.bmHeight, memdc, 0, 0, SRCCOPY);
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
	return(DefWindowProc(hwnd, iMsg, wParam, lParam));
}

BOOL CALLBACK EnumWindowMinimize(HWND hwnd, LPARAM lParam) {
	if (IsWindowVisible(hwnd)) { // 찾은 윈도우가 화면에 보이는 윈도우라면
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
	if (!IsWindowVisible(hwnd))
		return TRUE;
	char caption[255];
	GetWindowText(hwnd, caption, sizeof(caption));
	g_hwnd = FindWindow(NULL, caption);
	GetClientRect(g_hwnd, &rct); // 현재 윈도우의 좌표값을 받아온다
	CreateWindow("DisplayWindow", caption, WS_OVERLAPPEDWINDOW | WS_VISIBLE,
		CW_USEDEFAULT, CW_USEDEFAULT, rct.right - rct.left, rct.bottom - rct.top,
		NULL, (HMENU)NULL, hInst, NULL);
	hBit = WindowCapture(g_hwnd);
	
	return TRUE; // 계속 열거하려면 TRUE를 반환해야 한다
}

HBITMAP WindowCapture(HWND hwnd) {
	HDC hScrdc = GetDC(hwnd);
	HDC hMemdc = CreateCompatibleDC(hScrdc); // hdc와 호환되는 memdc 생성

	// hdc와 호환되는 비트맵 hBit 생성

	HBITMAP hBitmap = CreateCompatibleBitmap(hScrdc, rct.right, rct.bottom);
	HBITMAP hOldmap = (HBITMAP)SelectObject(hMemdc, hBitmap); // memdc에 hBit 형식의 그림을 그리기 위해서 사용

	BitBlt(hMemdc, 0, 0, rct.right-rct.left, rct.bottom-rct.top, hScrdc, 0, 0, SRCCOPY);
	//SetStretchBltMode(hScrdc, COLORONCOLOR);
	//StretchBlt(hMemdc, 0, 0, rct.right-rct.left, rct.bottom-rct.top, hScrdc, 0, 0, rct.right - rct.left, rct.bottom - rct.top, SRCCOPY);
	//PrintWindow(hwnd, hMemdc, 0); 창의 타이틀바까지 캡처함

	SelectObject(hMemdc, hOldmap);
	//DeleteObject(hBitmap);
	DeleteDC(hMemdc);
	DeleteDC(hScrdc);
	ReleaseDC(hwnd, hScrdc);

	return hBitmap;
}

/*
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