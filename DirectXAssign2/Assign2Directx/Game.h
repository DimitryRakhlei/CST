#pragma once
#include <d3d9.h>
#include <d3dx9.h>

#include <Windows.h>
#include "camera.h"


class Game {
public :
	//-----------------------------------------------------------------------------
	// Global variables
	//-----------------------------------------------------------------------------
	LPDIRECT3D9             g_pD3D = nullptr; // Used to create the D3DDevice
	LPDIRECT3DDEVICE9       g_pd3dDevice = nullptr; // Our rendering device

	LPD3DXMESH              g_pMesh = nullptr; // Our mesh object in sysmem
	D3DMATERIAL9*           g_pMeshMaterials = nullptr; // Materials for our mesh
	LPDIRECT3DTEXTURE9*     g_pMeshTextures = nullptr; // Textures for our mesh
	DWORD                   g_dwNumMaterials = 0L;   // Number of mesh materials

	LPD3DXMESH              g_pMesh_two = nullptr; // Our mesh object in sysmem
	D3DMATERIAL9*           g_pMeshMaterials_two = nullptr; // Materials for our mesh
	LPDIRECT3DTEXTURE9*     g_pMeshTextures_two = nullptr; // Textures for our mesh
	DWORD                   g_dwNumMaterials_two = 0L;   // Number of mesh materials


	D3DXVECTOR3 vEyePt;
	D3DXVECTOR3 vLookatPt;
	D3DXVECTOR3 vUpVec;
	D3DXMATRIXA16 matWorld;


	//selections
	BOOL firstSelected = FALSE;
	BOOL secondSelected = FALSE;

	BOOL ambient = TRUE;
	BOOL point = TRUE;
	BOOL spot = TRUE;
	BOOL directional = TRUE;

	camera c;


	struct Pos {
		float x;
		float y;
		float z;
		float xa;
		float ya;
		float za;
	};

	
	Pos first;
	Pos second;

	void init_light(void);

	//-----------------------------------------------------------------------------
	// Name: InitD3D()
	// Desc: Initializes Direct3D
	//-----------------------------------------------------------------------------
	HRESULT InitD3D(HWND hWnd);

	//-----------------------------------------------------------------------------
	// Name: InitGeometry()
	// Desc: Load the mesh and build the material and texture arrays
	//-----------------------------------------------------------------------------
	HRESULT InitGeometry();
	HRESULT LoadMesh(LPD3DXBUFFER* pD3DXMtrlBuffer);

	//-----------------------------------------------------------------------------
	// Name: Cleanup()
	// Desc: Releases all previously initialized objects
	//-----------------------------------------------------------------------------
	VOID Cleanup();


	//-----------------------------------------------------------------------------
	// Name: SetupMatrices()
	// Desc: Sets up the world, view, and projection transform matrices.
	//-----------------------------------------------------------------------------
	VOID SetupMatrices();

	//-----------------------------------------------------------------------------
	// Name: Render()
	// Desc: Draws the scene
	//-----------------------------------------------------------------------------
	VOID Render();
	VOID MoveCamera(int type, float amountX, float amountY, float amountZ, float deg);
};

//global instance of the game
Game g;