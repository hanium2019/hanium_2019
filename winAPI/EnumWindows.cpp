#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#include <iostream>

#pragma comment(linker, "/entry:WinMainCRTStartup /subsystem:console") 

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam);

BOOL CALLBACK EnumWindowProc(HWND hwnd, LPARAM lParam);

LPCTSTR lpszClass = _T("MainWindow");

HINSTANCE hInst;

HDC hdc;

char caption[255];

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

	hwnd = CreateWindow(lpszClass, lpszClass, WS_POPUPWINDOW,
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
	switch (iMsg) {
	case WM_CREATE:
		EnumWindows(EnumWindowProc, NULL);
		break;
	case WM_QUIT:
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}

BOOL CALLBACK EnumWindowProc(HWND hwnd, LPARAM lParam) {
	int len = ::GetWindowTextLength(hwnd);
	if (IsWindowVisible(hwnd) && len > 0) {
		GetWindowText(hwnd, caption, sizeof(caption));
		printf("%s\n", caption);
	}
	return TRUE;
}