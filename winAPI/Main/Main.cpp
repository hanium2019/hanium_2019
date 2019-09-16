#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#include <time.h>
#include "resource.h"

//#pragma comment(linker, "/entry:WinMainCRTStartup /subsystem:console")

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

LRESULT CALLBACK WndScroll(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

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

	WndClass.lpfnWndProc = WndScroll;
	WndClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	WndClass.lpszClassName = _T("ScrollWindow");

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
	HDC hdc;
	PAINTSTRUCT ps;

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
			break;
		case 07:
			CreateWindow("ScrollWindow", "Scroll Test", WS_OVERLAPPEDWINDOW | WS_VISIBLE,
				CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
				NULL, (HMENU)NULL, hInst, NULL);
			break;
		case 10:
			// pid = GetCurrentProcessId();
			// Process = OpenProcess(PROCESS_ALL_ACCESS, FALSE, pid);
			// CreateRemoteThread(hProcess, 0, 0, 0, ExitProcess, 0, 0);
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
		hdc = BeginPaint(hwnd, &ps);
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}

HBITMAP hBitmap = NULL;

LRESULT CALLBACK WndEmpty(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	HDC hdc, memdc;
	BITMAP bmp;
	HBITMAP hOld;
	PAINTSTRUCT ps;

	switch (iMsg) {
	case WM_PAINT:
		hdc = BeginPaint(hwnd, &ps);
		if (hBitmap != NULL) {
			memdc = CreateCompatibleDC(hdc);
			hOld = (HBITMAP)SelectObject(memdc, hBitmap);
			GetObject(hBitmap, sizeof(BITMAP), &bmp);
			BitBlt(hdc, 0, 0, bmp.bmWidth, bmp.bmHeight, memdc, 0, 0, SRCCOPY);
			SelectObject(memdc, hOld);
			//DeleteObject(memdc);
			DeleteDC(memdc);
		}
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		if (hBitmap != NULL)
			DeleteObject(hBitmap);
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}

HBITMAP hBimo = NULL;
HBITMAP hOldBimo = NULL;

RECT crt, rt;

int inc;
int pos; // 전역으로 하지 않을 경우 스크롤 이벤트 발생X
int max; // 전역으로 하지 않을 경우 창의 끝을 무시하고 계속 아래로 내려감

LRESULT CALLBACK WndScroll(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	HDC hdc, memdc;
	PAINTSTRUCT ps;
	BITMAP bmp;

	int lines;
	TCHAR str[128];

	switch (iMsg) {
	case WM_CREATE:
		hBimo = LoadBitmap(hInst, MAKEINTRESOURCE(IDB_BITMAP1));
		break;
	case WM_SIZE:
		max = 20 * 100 - HIWORD(lParam); // 창의 하단에 맞춰서 스크롤 끝 지정(max = 20*100)
		SetScrollRange(hwnd, SB_VERT, 0, max, TRUE); // 스크롤 범위 설정
		SetScrollPos(hwnd, SB_VERT, 0, TRUE); // 스크롤 초기값 설정
		break;
	case WM_LBUTTONDOWN:
		InvalidateRect(hwnd, &rt, TRUE);
		UpdateWindow(hwnd);
		break;
	case WM_MOUSEWHEEL:
		SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, &lines, 0);
		for (int i = 0; i < lines; i++) {
			if ((SHORT)HIWORD(wParam) > 0) // 휠이 앞으로 돌아갈 때
				inc = -lines * 10; // 휠을 한번 올릴 때 10줄 만큼 inc 감소
			//MessageBox(hScroll, _T("마우스 올림"), _T("Wheel Test"), NULL);
			else if ((SHORT)HIWORD(wParam) < 0)
				inc = lines * 20; // 휠을 한번 내릴 때 10줄 만큼 inc 증가
			//MessageBox(hScroll, _T("마우스 내림"), _T("Wheel Test"), NULL);
		}

		// 썸이 밖으로 나가지 않게끔 막아줌
		if (pos + inc < 0)
			inc = -pos;
		if (pos + inc > max)
			inc = max - pos;
		pos += inc; // 썸의 위치를 inc 만큼 변경

		GetClientRect(hwnd, &crt); // 현재 윈도우의 RECT 구조체 선언
		SetRect(&rt, 100, 100, crt.right, crt.bottom); // RECT 구조체의 width, height을 새로 지정
		ScrollWindow(hwnd, 0, -inc, &rt, &rt); // SetRect로 정해준 범위를 제외하고 스크롤 할 수 있게 함
		SetScrollPos(hwnd, SB_VERT, pos, TRUE); // 썸의 위치를 pos의 위치에 맞춰서 변경
		break;
	case WM_PAINT:
		hdc = BeginPaint(hwnd, &ps);
		memdc = CreateCompatibleDC(hdc);
		hOldBimo = (HBITMAP)SelectObject(memdc, hBimo);
		GetObject(hBimo, sizeof(BITMAP), &bmp);

		// pos의 현재 위치에 따라서 이미지를 그려줌(스크롤시 화면이 올라가거나 내려가는 것처럼 보이는 효과 발생)
		BitBlt(hdc, 0, -pos, bmp.bmWidth, bmp.bmHeight, memdc, 0, 0, SRCCOPY); 
		SelectObject(memdc, hOldBimo);
		DeleteDC(memdc);
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		if (hBitmap != NULL)
			DeleteObject(hBitmap);
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}

BOOL CALLBACK EnumWindowMinimize(HWND hwnd, LPARAM lParam) {
	char caption[255];
	GetWindowText(hwnd, caption, sizeof(caption));
	if (IsWindowVisible(hwnd) && strcmp(caption, _T("MainWindow"))) { // 찾은 윈도우가 화면에 보이는 윈도우라면
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

HWND g_hwnd;

RECT rct;

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
		hBitmap = WindowCapture(g_hwnd);
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
	HBITMAP hBit = NULL;
	HBITMAP hOld = NULL;

	// 비트맵이 남아 있으면 지운다
	if (hBit) DeleteObject(hBit);

	// hdc와 호환되는 비트맵 hBit 생성
	hBit = CreateCompatibleBitmap(hScrdc, rct.right, rct.bottom);
	hOld = (HBITMAP)SelectObject(hMemdc, hBit); // memdc에 hBit 형식의 그림을 그리기 위해서 사용

	BitBlt(hMemdc, 0, 0, rct.right, rct.bottom, hScrdc, 0, 0, SRCCOPY);
	//SetStretchBltMode(hScrdc, COLORONCOLOR);
	//StretchBlt(hMemdc, 0, 0, rct.right-rct.left, rct.bottom-rct.top, hScrdc, 0, 0, rct.right - rct.left, rct.bottom - rct.top, SRCCOPY);
	//PrintWindow(hwnd, hMemdc, PW_CLIENTONLY);

	SelectObject(hMemdc, hOld);
	ReleaseDC(hwnd, hScrdc);
	DeleteDC(hMemdc);

	return hBit;
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
	ReleaseDC(hwnd, hScrdc);
	DeleteDC(hMemdc);

	return hBit;
}