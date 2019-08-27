#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#include <time.h>

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
	WndClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
	WndClass.lpszClassName = lpszClass;
	WndClass.lpszMenuName = NULL;

	RegisterClass(&WndClass);

	hwnd = CreateWindow(lpszClass, lpszClass, WS_OVERLAPPEDWINDOW | WS_VSCROLL,
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

RECT crt, rt;

int inc = 0;
int pos = 0; // 전역으로 하지 않을 경우 스크롤 이벤트 발생X
int max = 20 * 100; // 전역으로 하지 않을 경우 창의 끝을 무시하고 계속 아래로 내려감

LRESULT CALLBACK WndProc(HWND hwnd, UINT iMsg, WPARAM wParam, LPARAM lParam) {
	TCHAR str[128];
	HDC hdc;
	PAINTSTRUCT ps;

	int lines;

	switch (iMsg) {
	case WM_CREATE: // 스크롤의 범위 및 초기값 정해줌
		CreateWindow("button", "버튼1", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 20, 100, 25, hwnd, (HMENU)0, hInst, NULL);
		CreateWindow("button", "버튼2", WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON,
			20, 50, 100, 25, hwnd, (HMENU)0, hInst, NULL);
		//SetScrollRange(hwnd, SB_VERT, 0, ymax, TRUE);
		//SetScrollPos(hwnd, SB_VERT, 0, TRUE);
		break;
	case WM_SIZE:
		max = 20*100 - HIWORD(lParam); // 창의 하단에 맞춰서 스크롤 끝 지정
		SetScrollRange(hwnd, SB_VERT, 0, max, TRUE); // 스크롤 범위 설정
		SetScrollPos(hwnd, SB_VERT, 0, TRUE); // 스크롤 초기값 설정
		break;
	case WM_KEYDOWN:
		switch (wParam) {
		case VK_UP:
			SendMessage(hwnd, WM_VSCROLL, MAKELONG(SB_PAGEUP, 0), 0);
			break;
		case VK_DOWN:
			SendMessage(hwnd, WM_VSCROLL, MAKELONG(SB_PAGEDOWN, 0), 0);
			break;
		}
	case WM_MOUSEWHEEL:
		SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, &lines, 0);
		for (int i = 0; i < lines; i++) {
			if ((SHORT)HIWORD(wParam) > 0) // 휠이 앞으로 돌아갈 때
				inc = -lines * 20;
				//MessageBox(hwnd, _T("마우스 올림"), _T("Wheel Test"), NULL);
			else if ((SHORT)HIWORD(wParam) < 0) // 휠이 뒤로 돌아갈 때
				inc = lines * 20;
				//MessageBox(hwnd, _T("마우스 내림"), _T("Wheel Test"), NULL);
		}

		if (pos + inc < 0)
			inc = -pos;
		if (pos + inc > max)
			inc = max - pos;
		pos = pos + inc;

		GetClientRect(hwnd, &crt);
		SetRect(&rt, 150, 100, crt.right, crt.bottom);
		ScrollWindow(hwnd, 0, -inc, &rt, &rt);
		SetScrollPos(hwnd, SB_VERT, pos, TRUE); // 표준 스크롤바
		//SetScrollPos(hScroll, SB_CTL, scroll, TRUE); // 스크롤바 컨트롤
		break;
	case WM_VSCROLL:
		inc = 0;
		switch (LOWORD(wParam)) {
		case SB_LINEUP:
			inc = -20;
			break;
		case SB_LINEDOWN:
			inc = 20;
			break;
		case SB_PAGEUP:
			inc = -200;
			break;
		case SB_PAGEDOWN:
			inc = 200;
			break;
		case SB_THUMBTRACK:
			inc = HIWORD(wParam) - pos;
			break;
		}
		
		if (pos + inc < 0)
			inc = -pos;
		if (pos + inc > max)
			inc = max - pos;
		pos = pos + inc;

		GetClientRect(hwnd, &crt);
		SetRect(&rt, 150, 100, crt.right, crt.bottom);
		ScrollWindow(hwnd, 0, -inc, &rt, &rt);
		SetScrollPos(hwnd, SB_VERT, pos, TRUE);
		break;
	case WM_PAINT:
		hdc = BeginPaint(hwnd, &ps);
		for (int i = 0; i < 100; i++) {
			wsprintf(str, _T("Line Number : %d"), i);
			TextOut(hdc, 150, i * 20 - pos, str, lstrlen(str));
		}
		EndPaint(hwnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	}
	return DefWindowProc(hwnd, iMsg, wParam, lParam);
}