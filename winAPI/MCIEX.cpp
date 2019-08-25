#include <vfw.h>
#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#pragma comment(lib, "vfw32.lib")

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

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

HWND hAVI = 0;
BOOL play = FALSE, pause = FALSE;
int vol = 1;
HWND hEdit = 0;

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	HDC hdc;
	RECT rt;
	PAINTSTRUCT ps;

	switch (iMsg) {
	case WM_CREATE: // 프로그램 실행하면 자동 재생
		if (hAVI) {
			MCIWndClose(hAVI);
			MCIWndDestroy(hAVI);
			hAVI = NULL;
		}
		char szFileName[] = "Sample.wmv";
		hAVI = MCIWndCreate(hwnd, hInst, MCIWNDF_NOTIFYANSI |
			MCIWNDF_NOMENU | MCIWNDF_NOTIFYALL | MCIWNDF_NOPLAYBAR, szFileName);
		if (hAVI) {
			GetClientRect(hwnd, &rt);
			SetWindowPos(hAVI, NULL, 0, 0, rt.right, rt.bottom, SWP_NOZORDER | SWP_NOMOVE);
			MCIWndPlay(hAVI);
		}
		break;
	case WM_KEYDOWN:
		switch (wParam) {
		case VK_ESCAPE:
			MCIWndClose(hAVI);
			MCIWndDestroy(hAVI);
			hAVI = 0;
			break;
		}
		case VK_SPACE:
			if (play) {
				switch (pause) {
				case TRUE:
					MCIWndResume(hAVI);
					pause = FALSE;
					UpdateWindow(hwnd);
					break;
				case FALSE:
					MCIWndPause(hAVI);
					pause = TRUE;
					UpdateWindow(hwnd);
					break;
				}
			}
			break;
		case VK_UP:
			if (play) {
				vol += 50;
				if (vol >= 1000) vol = 1000;
				MCIWndSetVolume(hAVI, vol);
				break;
			}
		case VK_DOWN:
			if (play) {
				vol += 50;
				if (vol <= 0) vol = 0;
				MCIWndSetVolume(hAVI, vol);
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