// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}



extern "C" {

	__declspec(dllexport) void loadBytesToDll(BYTE * pointer, int datasize) {
		//ree(pSaveBuffer);
		dwDataLength = datasize;
		pSaveBuffer = (PBYTE)realloc(pSaveBuffer, datasize);
		for (int i = 0; i < datasize; i++) {
			pSaveBuffer[i] = pointer[i];
		}
	}

	__declspec(dllexport) void setSampR(int samp) {
		SAMPRATE = samp;
	}

	__declspec (dllexport) void setChannels(int chan) {
		CHANNELS = chan;
	}

	__declspec (dllexport) void bitDepth(int sampps) {
		BITDEPTH = sampps;
	}

	__declspec (dllexport) void setBlockAllign(int allign) {
		waveform.nBlockAlign = allign;
	}

	__declspec(dllexport) int getByteDataSize() {
		return (int)dwDataLength;
	}

	__declspec(dllexport) void storeByteData(BYTE * pointer) {
		for (int i = 0; i < (int)dwDataLength; i++) {
			pointer[i] = pSaveBuffer[i];
		}
	}

	__declspec(dllexport) int postRecordMessage() {
		WPARAM msg = MAKEWPARAM(IDC_RECORD_BEG, 0);
		PostMessage(post.hwnd, WM_COMMAND, msg, 0);
		return 1;
	}
	__declspec(dllexport) int postStopRecord() {
		WPARAM msg = MAKEWPARAM(IDC_RECORD_END, 0);
		PostMessage(post.hwnd, WM_COMMAND, msg, 0);
		return 1;
	}

	__declspec(dllexport) int postPlayMessage() {
		WPARAM msg = MAKEWPARAM(IDC_PLAY_BEG, 0);
		PostMessage(post.hwnd, WM_COMMAND, msg, 0);
		return 1;
	}
	__declspec(dllexport) int postPlayPause() {
		WPARAM msg = MAKEWPARAM(IDC_PLAY_PAUSE, 0);
		PostMessage(post.hwnd, WM_COMMAND, msg, 0);
		return 1;
	}

	__declspec(dllexport) int postQuitMessage() {
		PostMessage(post.hwnd, WM_DESTROY, 0, 0);
		return 1;
	}

	__declspec(dllexport) int postVolumeUp() {

		return 1;
	}

	__declspec(dllexport) int postVolumeDown() {

		return 1;
	}

	__declspec(dllexport) int postVolumeMute() {

		return 1;
	}


	__declspec(dllexport) int hello() {
		return 1;
	}
	__declspec(dllexport) int startProg(HINSTANCE hInstance) {
		static TCHAR szAppName[] = TEXT("HelloWin");
		HWND         hwnd;
		MSG          msg;
		WNDCLASS     wndclass;

		wndclass.style = CS_HREDRAW | CS_VREDRAW;
		wndclass.lpfnWndProc = WndProc;
		wndclass.cbClsExtra = 0;
		wndclass.cbWndExtra = 0;
		wndclass.hInstance = hInstance;
		wndclass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
		wndclass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
		wndclass.lpszMenuName = NULL;
		wndclass.lpszClassName = szAppName;

		if (!RegisterClass(&wndclass))
		{
			MessageBox(NULL, TEXT("This program requires Windows NT!"),
				szAppName, MB_ICONERROR);
			return 0;
		}

		hwnd = CreateWindow(szAppName,                  // window class name
			TEXT("The Hello Program"), // window caption
			WS_OVERLAPPEDWINDOW,        // window style
			CW_USEDEFAULT,              // initial x position
			CW_USEDEFAULT,              // initial y position
			CW_USEDEFAULT,              // initial x size
			CW_USEDEFAULT,              // initial y size
			NULL,                       // parent window handle
			NULL,                       // window menu handle
			hInstance,                  // program instance handle
			NULL);                     // creation parameters

									   //ShowWindow(hwnd, 1);
		UpdateWindow(hwnd);

		if (hwnd == NULL) {
			MessageBeep(MB_ICONEXCLAMATION);
		}
		while (GetMessage(&msg, NULL, 0, 0))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
		return msg.wParam;
	}

}



void ReverseMemory(BYTE * pBuffer, int iLength)
{
	BYTE b;
	int  i;

	for (i = 0; i < iLength / 2; i++)
	{
		b = pBuffer[i];
		pBuffer[i] = pBuffer[iLength - i - 1];
		pBuffer[iLength - i - 1] = b;
	}
}

LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	//HDC         hdc;
	//PAINTSTRUCT ps;
	//RECT        rect;
	static BOOL         bRecording, bPlaying, bReverse, bPaused,
		bEnding, bTerminating;
	static DWORD        dwRepetitions = 1;
	static HWAVEIN      hWaveIn;
	static HWAVEOUT     hWaveOut;
	static PWAVEHDR     pWaveHdr1, pWaveHdr2;
	static TCHAR        szOpenError[] = TEXT("Error opening waveform audio!");
	static TCHAR        szMemError[] = TEXT("Error allocating memory!");



	switch (message)
	{
	case WM_CREATE:

		pWaveHdr1 = (PWAVEHDR)malloc(sizeof(WAVEHDR));
		pWaveHdr2 = (PWAVEHDR)malloc(sizeof(WAVEHDR));

		// Allocate memory for save buffer

		pSaveBuffer = (PBYTE)malloc(1);

		post.hwnd = hwnd;

		return 0;

	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case IDC_RECORD_BEG:
			// Allocate buffer memory
			MessageBeep(MB_ICONEXCLAMATION);
			pBuffer1 = (PBYTE)malloc(INP_BUFFER_SIZE);
			pBuffer2 = (PBYTE)malloc(INP_BUFFER_SIZE);

			if (!pBuffer1 || !pBuffer2)
			{
				if (pBuffer1) free(pBuffer1);
				if (pBuffer2) free(pBuffer2);

				MessageBeep(MB_ICONEXCLAMATION);
				MessageBox(hwnd, szMemError, szAppName,
					MB_ICONEXCLAMATION | MB_OK);
				return TRUE;
			}

			// Open waveform audio for input

			waveform.wFormatTag = WAVE_FORMAT_PCM;
			waveform.nChannels = 1; //1
			waveform.nSamplesPerSec = 22050;
			waveform.nAvgBytesPerSec = 2 * 22050 * 1;
			waveform.nBlockAlign = 1 * 2;
			waveform.wBitsPerSample = 2 * 8;
			waveform.cbSize = 0;

			if (waveInOpen(&hWaveIn, WAVE_MAPPER, &waveform,
				(DWORD)hwnd, 0, CALLBACK_WINDOW))
			{
				free(pBuffer1);
				free(pBuffer2);
				MessageBeep(MB_ICONEXCLAMATION);
				MessageBox(hwnd, szOpenError, szAppName,
					MB_ICONEXCLAMATION | MB_OK);
			}
			// Set up headers and prepare them

			pWaveHdr1->lpData = (LPSTR)pBuffer1;
			pWaveHdr1->dwBufferLength = INP_BUFFER_SIZE;
			pWaveHdr1->dwBytesRecorded = 0;
			pWaveHdr1->dwUser = 0;
			pWaveHdr1->dwFlags = 0;
			pWaveHdr1->dwLoops = 1;
			pWaveHdr1->lpNext = NULL;
			pWaveHdr1->reserved = 0;

			waveInPrepareHeader(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));

			pWaveHdr2->lpData = (LPSTR)pBuffer2;
			pWaveHdr2->dwBufferLength = INP_BUFFER_SIZE;
			pWaveHdr2->dwBytesRecorded = 0;
			pWaveHdr2->dwUser = 0;
			pWaveHdr2->dwFlags = 0;
			pWaveHdr2->dwLoops = 1;
			pWaveHdr2->lpNext = NULL;
			pWaveHdr2->reserved = 0;

			waveInPrepareHeader(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));
			return TRUE;

		case IDC_RECORD_END:
			// Reset input to return last buffer

			bEnding = TRUE;
			waveInReset(hWaveIn);
			return TRUE;

		case IDC_PLAY_BEG:
			// Open waveform audio for output

			waveform.wFormatTag = WAVE_FORMAT_PCM;
			waveform.nChannels = 1; //1
			waveform.nSamplesPerSec = 22050;
			waveform.nAvgBytesPerSec = 2 * 22050 * 1;
			waveform.nBlockAlign = 1 * 2;
			waveform.wBitsPerSample = 2 * 8;
			waveform.cbSize = 0;

			if (waveOutOpen(&hWaveOut, WAVE_MAPPER, &waveform,
				(DWORD)hwnd, 0, CALLBACK_WINDOW))
			{
				MessageBeep(MB_ICONEXCLAMATION);
				MessageBox(hwnd, szOpenError, szAppName,
					MB_ICONEXCLAMATION | MB_OK);
			}
			return TRUE;

		case IDC_PLAY_PAUSE:
			// Pause or restart 
			if (!bPaused)
			{
				waveOutPause(hWaveOut);
				SetDlgItemText(hwnd, IDC_PLAY_PAUSE, TEXT("Resume"));
				bPaused = TRUE;
			}
			else
			{
				waveOutRestart(hWaveOut);
				SetDlgItemText(hwnd, IDC_PLAY_PAUSE, TEXT("Pause"));
				bPaused = FALSE;
			}
			return TRUE;

		case IDC_PLAY_END:
			// Reset output for close preparation

			bEnding = TRUE;
			waveOutReset(hWaveOut);
			return TRUE;

		case IDC_PLAY_REV:
			// Reverse save buffer and play

			bReverse = TRUE;
			ReverseMemory(pSaveBuffer, dwDataLength);

			SendMessage(hwnd, WM_COMMAND, IDC_PLAY_BEG, 0);
			return TRUE;

		case IDC_PLAY_REP:
			// Set infinite repetitions and play

			dwRepetitions = -1;
			SendMessage(hwnd, WM_COMMAND, IDC_PLAY_BEG, 0);
			return TRUE;

		case IDC_PLAY_SPEED:
			// Open waveform audio for fast output

			waveform.wFormatTag = WAVE_FORMAT_PCM;
			waveform.nChannels = 1;
			waveform.nSamplesPerSec = 22050;
			waveform.nAvgBytesPerSec = 22050;
			waveform.nBlockAlign = 1;
			waveform.wBitsPerSample = 8;
			waveform.cbSize = 0;

			if (waveOutOpen(&hWaveOut, 0, &waveform, (DWORD)hwnd, 0,
				CALLBACK_WINDOW))
			{
				MessageBeep(MB_ICONEXCLAMATION);
				MessageBox(hwnd, szOpenError, szAppName,
					MB_ICONEXCLAMATION | MB_OK);
			}
			return TRUE;
		case IDC_VOLUME_DOWN:
			return TRUE;
		case IDC_VOLUME_UP:

			return TRUE;
		case IDC_VOLUME_MUTE:

			return TRUE;
		}
		break;

	case MM_WIM_OPEN:
		// Shrink down the save buffer

		pSaveBuffer = (PBYTE)realloc(pSaveBuffer, 1);


	
		// Add the buffers

		waveInAddBuffer(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));
		waveInAddBuffer(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));

		// Begin sampling

		bRecording = TRUE;
		bEnding = FALSE;
		dwDataLength = 0;
		waveInStart(hWaveIn);
		return TRUE;

	case MM_WIM_DATA:

		// Reallocate save buffer memory

		pNewBuffer = (PBYTE)realloc(pSaveBuffer, dwDataLength +
			((PWAVEHDR)lParam)->dwBytesRecorded);

		if (pNewBuffer == NULL)
		{
			waveInClose(hWaveIn);
			MessageBeep(MB_ICONEXCLAMATION);
			MessageBox(hwnd, szMemError, szAppName,
				MB_ICONEXCLAMATION | MB_OK);
			return TRUE;
		}

		pSaveBuffer = pNewBuffer;
		CopyMemory(pSaveBuffer + dwDataLength, ((PWAVEHDR)lParam)->lpData,
			((PWAVEHDR)lParam)->dwBytesRecorded);

		dwDataLength += ((PWAVEHDR)lParam)->dwBytesRecorded;

		if (bEnding)
		{
			waveInClose(hWaveIn);
			return TRUE;
		}

		// Send out a new buffer

		waveInAddBuffer(hWaveIn, (PWAVEHDR)lParam, sizeof(WAVEHDR));
		return TRUE;

	case MM_WIM_CLOSE:
		// Free the buffer memory

		waveInUnprepareHeader(hWaveIn, pWaveHdr1, sizeof(WAVEHDR));
		waveInUnprepareHeader(hWaveIn, pWaveHdr2, sizeof(WAVEHDR));

		free(pBuffer1);
		free(pBuffer2);


		bRecording = FALSE;

		if (bTerminating)
			SendMessage(hwnd, WM_SYSCOMMAND, SC_CLOSE, 0L);

		return TRUE;

	case MM_WOM_OPEN:
		// Set up header

		pWaveHdr1->lpData = (LPSTR)pSaveBuffer;
		pWaveHdr1->dwBufferLength = dwDataLength;
		pWaveHdr1->dwBytesRecorded = 0;
		pWaveHdr1->dwUser = 0;
		pWaveHdr1->dwFlags = WHDR_BEGINLOOP | WHDR_ENDLOOP;
		pWaveHdr1->dwLoops = dwRepetitions;
		pWaveHdr1->lpNext = NULL;
		pWaveHdr1->reserved = 0;

		// Prepare and write

		waveOutPrepareHeader(hWaveOut, pWaveHdr1, sizeof(WAVEHDR));
		waveOutWrite(hWaveOut, pWaveHdr1, sizeof(WAVEHDR));

		bEnding = FALSE;
		bPlaying = TRUE;
		return TRUE;

	case MM_WOM_DONE:
		waveOutUnprepareHeader(hWaveOut, pWaveHdr1, sizeof(WAVEHDR));
		waveOutClose(hWaveOut);
		return TRUE;

	case MM_WOM_CLOSE:
		
		bPaused = FALSE;
		dwRepetitions = 1;
		bPlaying = FALSE;

		if (bReverse)
		{
			ReverseMemory(pSaveBuffer, dwDataLength);
			bReverse = FALSE;
		}

		if (bTerminating)
			SendMessage(hwnd, WM_SYSCOMMAND, SC_CLOSE, 0L);

		return TRUE;

	case WM_SYSCOMMAND:
		switch (LOWORD(wParam))
		{
		case SC_CLOSE:
			if (bRecording)
			{
				bTerminating = TRUE;
				bEnding = TRUE;
				waveInReset(hWaveIn);
				return TRUE;
			}

			if (bPlaying)
			{
				bTerminating = TRUE;
				bEnding = TRUE;
				waveOutReset(hWaveOut);
				return TRUE;
			}

			free(pWaveHdr1);
			free(pWaveHdr2);
			free(pSaveBuffer);
			EndDialog(hwnd, 0);
			return TRUE;
		}
		break;

	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;
	}
	return DefWindowProc(hwnd, message, wParam, lParam);
}