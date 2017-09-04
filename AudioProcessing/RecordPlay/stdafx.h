// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once
#ifndef STDAFX_H
#include "targetver.h"
#include <windows.h>
#include <tchar.h>
#include <stdio.h>
#define INP_BUFFER_SIZE 16384
#define IDC_RECORD_BEG 401
#define IDC_RECORD_END 402
#define IDC_PLAY_BEG 403
#define IDC_PLAY_PAUSE 404
#define IDC_PLAY_END 405
#define IDC_PLAY_REV 406
#define IDC_PLAY_REP 407
#define IDC_PLAY_SPEED 408
#define IDC_VOLUME_UP 409
#define IDC_VOLUME_DOWN 410
#define IDC_VOLUME_MUTE 411

struct POST_PARAMS {
	HWND hwnd;
};

static TCHAR szAppName[] = TEXT("HelloWin");
static POST_PARAMS post;
static DWORD dwDataLength;

LRESULT CALLBACK WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam);
//INT_PTR CALLBACK DialogProc(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam);
//int CALLBACK DlgProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
extern "C" __declspec(dllexport) void loadBytesToDll(BYTE * pointer, int datasize);
extern "C" __declspec(dllexport) int startProg(HINSTANCE hInstance);
extern "C" __declspec(dllexport) int hello();
extern "C" __declspec(dllexport) int postQuitMessage();
extern "C" __declspec(dllexport) int postRecordMessage();
extern "C" __declspec(dllexport) int postStopRecord();
extern "C" __declspec(dllexport) int postPlayMessage();
extern "C" __declspec(dllexport) int postPlayPause();
extern "C" __declspec(dllexport) int getByteDataSize();
extern "C" __declspec(dllexport) int postVolumeUp();
extern "C" __declspec(dllexport) int postVolumeDown();
extern "C" __declspec(dllexport) void storeByteData(BYTE * pointer);
extern "C" __declspec(dllexport) void setSampR(int samp);
extern "C" __declspec (dllexport) void setChannels(int chan);
extern "C" __declspec (dllexport) void bitDepth(int sampps);
extern "C" __declspec (dllexport) void setBlockAllign(int allign);

static PBYTE  pBuffer1, pBuffer2, pSaveBuffer, pNewBuffer;
static WAVEFORMATEX waveform;
static int SAMPRATE;
static int BITDEPTH;
static int CHANNELS;

#endif