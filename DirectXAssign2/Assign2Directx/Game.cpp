#include "Game.h"


void Game::init_light() {
	D3DLIGHT9 point_light;
    D3DMATERIAL9 point_material;

	D3DLIGHT9 spotlight;
	ZeroMemory(&spotlight, sizeof(spotlight));

	D3DLIGHT9 directional;
	ZeroMemory(&directional, sizeof(directional));

    ZeroMemory(&point_light, sizeof(point_light));
    point_light.Type = D3DLIGHT_POINT;    // make the light type 'point light'
    point_light.Diffuse = D3DXCOLOR(0.5f, 0.5f, 0.5f, 1.0f);
    point_light.Position = D3DXVECTOR3(5.0f, 5.0f, 0.0f);
    point_light.Range = 100.0f;    // a range of 100
    point_light.Attenuation0 = 0.0f;    // no constant inverse attenuation
    point_light.Attenuation1 = 0.125f;    // only .125 inverse attenuation
    point_light.Attenuation2 = 0.0f;    // no square inverse attenuation

	spotlight.Type = D3DLIGHT_SPOT;   
    spotlight.Diffuse = D3DXCOLOR(0.5f, 0.5f, 0.5f, 1.0f);
    spotlight.Position = D3DXVECTOR3(0.0f , 3.0f , -5.0f);
	spotlight.Direction = D3DXVECTOR3(0.0f, -3.0f, 5.0f);
    spotlight.Range = 100.0f;   
    spotlight.Attenuation0 = 0.0f;    
	spotlight.Falloff = 1.0f;
    spotlight.Attenuation1 = 0.125f;
    spotlight.Attenuation2 = 0.0f;  
	spotlight.Theta = 0.5f;
	spotlight.Phi = 0.6f;

	directional.Type = D3DLIGHT_DIRECTIONAL;   
    directional.Diffuse = D3DXCOLOR(1.0f, 1.0f, 1.0f, 1.0f);
	directional.Direction = D3DXVECTOR3(-1.0f, -5.0f, 1.0f);
    directional.Attenuation0 = 0.0f;    
	directional.Falloff = 1.0f;
    directional.Attenuation1 = 0.05f;
    directional.Attenuation2 = 0.0f;  
	

    g_pd3dDevice->SetLight(0, &point_light);
    g_pd3dDevice->LightEnable(0, TRUE);

	g_pd3dDevice->SetLight(1, &spotlight);
	g_pd3dDevice->LightEnable (1, TRUE);

	g_pd3dDevice->SetLight(2, &directional);
    g_pd3dDevice->LightEnable(2, TRUE);

    ZeroMemory(&point_material, sizeof(D3DMATERIAL9));
    point_material.Diffuse = D3DXCOLOR(1.0f, 1.0f, 1.0f, 1.0f);
    point_material.Ambient = D3DXCOLOR(1.0f, 1.0f, 1.0f, 1.0f);

    g_pd3dDevice->SetMaterial(&point_material);
}

HRESULT Game::InitD3D(HWND hWnd) {
	// Create the D3D object.
	if ( NULL == (g_pD3D = Direct3DCreate9 (D3D_SDK_VERSION)) )
		return E_FAIL;

	// Set up the structure used to create the D3DDevice. Since we are now
	// using more complex geometry, we will create a device with a zbuffer.
	D3DPRESENT_PARAMETERS d3dpp;
	ZeroMemory(&d3dpp, sizeof(d3dpp));
	d3dpp.Windowed = FALSE;
	d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
	d3dpp.BackBufferFormat = D3DFMT_A8R8G8B8;
	d3dpp.EnableAutoDepthStencil = TRUE;
	d3dpp.AutoDepthStencilFormat = D3DFMT_D16;
	d3dpp.BackBufferWidth = 800;
	d3dpp.BackBufferHeight = 600;

	// Create the D3DDevice
	if ( FAILED(g_pD3D->CreateDevice(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, hWnd,
		D3DCREATE_SOFTWARE_VERTEXPROCESSING,
		&d3dpp, &g_pd3dDevice)) ) {
		return E_FAIL;
	}

	// Turn on the zbuffer
	g_pd3dDevice->SetRenderState (D3DRS_ZENABLE , TRUE);

	// Turn on ambient lighting 
	g_pd3dDevice->SetRenderState (D3DRS_AMBIENT , 0xffffffff);

	return S_OK;
}


HRESULT Game::LoadMesh(LPD3DXBUFFER* pD3DXMtrlBuffer) {
	// Load the mesh from the specified file
	if ( FAILED(D3DXLoadMeshFromX(TEXT("Harrier.x"), D3DXMESH_SYSTEMMEM,
		g_pd3dDevice, NULL,
		pD3DXMtrlBuffer, NULL, &g_dwNumMaterials,
		&g_pMesh)) ) {
		// If model is not in current folder, try parent folder
		if ( FAILED(D3DXLoadMeshFromX(TEXT("..\\Harrier.x"), D3DXMESH_SYSTEMMEM,
			g_pd3dDevice, NULL,
			pD3DXMtrlBuffer, NULL, &g_dwNumMaterials,
			&g_pMesh)) ) {
			MessageBox (nullptr , TEXT("Could not find Harrier.x") , TEXT("Meshes.exe") , MB_OK);
			return E_FAIL;
		}
	}

	return S_OK;
}

HRESULT Game::InitGeometry() {
	LPD3DXBUFFER pD3DXMtrlBuffer, pD3DXMtrlBuffer2;
	LoadMesh (&pD3DXMtrlBuffer);
	
	init_light();

	first = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
	second = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
	
	if ( FAILED(D3DXLoadMeshFromX(TEXT("Viggen.x"), D3DXMESH_SYSTEMMEM,
		g_pd3dDevice, NULL,
		&pD3DXMtrlBuffer2, NULL, &g_dwNumMaterials_two,
		&g_pMesh_two)) ) {
		// If model is not in current folder, try parent folder
		if ( FAILED(D3DXLoadMeshFromX(TEXT("..\\Viggen.x"), D3DXMESH_SYSTEMMEM,
			g_pd3dDevice, NULL,
			&pD3DXMtrlBuffer2, NULL, &g_dwNumMaterials_two,
			&g_pMesh_two)) ) {
			MessageBox (nullptr , TEXT("Could not find tank.x") , TEXT("Meshes.exe") , MB_OK);
			return E_FAIL;
		}
	}


	// We need to extract the material properties and texture names from the 
	// pD3DXMtrlBuffer
	D3DXMATERIAL* d3dxMaterials = static_cast<D3DXMATERIAL*> (pD3DXMtrlBuffer->GetBufferPointer());
	g_pMeshMaterials = new D3DMATERIAL9[g_dwNumMaterials];
	g_pMeshTextures = new LPDIRECT3DTEXTURE9[g_dwNumMaterials];

	D3DXMATERIAL* d3dxMaterials2 = static_cast<D3DXMATERIAL*> (pD3DXMtrlBuffer2->GetBufferPointer());
	g_pMeshMaterials_two = new D3DMATERIAL9[g_dwNumMaterials_two];
	g_pMeshTextures_two = new LPDIRECT3DTEXTURE9[g_dwNumMaterials_two];

	for ( DWORD i = 0; i < g_dwNumMaterials; i++ ) {
		// Copy the material
		g_pMeshMaterials[i] = d3dxMaterials[i].MatD3D;

		// Set the ambient color for the material (D3DX does not do this)
		g_pMeshMaterials[i].Ambient = g_pMeshMaterials[i].Diffuse;

		g_pMeshTextures[i] = nullptr;
		if ( d3dxMaterials[i].pTextureFilename != nullptr &&
			lstrlen (d3dxMaterials[i].pTextureFilename) > 0 ) {
			// Create the texture
			if ( FAILED(D3DXCreateTextureFromFile(g_pd3dDevice,
				d3dxMaterials[i].pTextureFilename,
				&g_pMeshTextures[i])) ) {
				// If texture is not in current folder, try parent folder
				const TCHAR* strPrefix = TEXT("..\\");
				const int lenPrefix = lstrlen (strPrefix);
				TCHAR strTexture[MAX_PATH];
				lstrcpyn (strTexture , strPrefix , MAX_PATH);
				lstrcpyn (strTexture + lenPrefix , d3dxMaterials[i].pTextureFilename , MAX_PATH - lenPrefix);
				// If texture is not in current folder, try parent folder
				if ( FAILED(D3DXCreateTextureFromFile(g_pd3dDevice,
					strTexture,
					&g_pMeshTextures[i])) ) {
					MessageBox (nullptr , TEXT("Could not find texture map") , TEXT("Meshes.exe") , MB_OK);
				}
			}
		}

	}


	for ( DWORD i = 0; i < g_dwNumMaterials_two; i++ ) {
		// Copy the material
		g_pMeshMaterials_two[i] = d3dxMaterials2[i].MatD3D;

		// Set the ambient color for the material (D3DX does not do this)
		g_pMeshMaterials_two[i].Ambient = g_pMeshMaterials_two[i].Diffuse;

		g_pMeshTextures_two[i] = nullptr;
		if ( d3dxMaterials2[i].pTextureFilename != nullptr &&
			lstrlen (d3dxMaterials2[i].pTextureFilename) > 0 ) {
			// Create the texture
			if ( FAILED(D3DXCreateTextureFromFile(g_pd3dDevice,
				d3dxMaterials2[i].pTextureFilename,
				&g_pMeshTextures_two[i])) ) {
				// If texture is not in current folder, try parent folder
				const TCHAR* strPrefix = TEXT("..\\");
				const int lenPrefix = lstrlen (strPrefix);
				TCHAR strTexture[MAX_PATH];
				lstrcpyn (strTexture , strPrefix , MAX_PATH);
				lstrcpyn (strTexture + lenPrefix , d3dxMaterials2[i].pTextureFilename , MAX_PATH - lenPrefix);
				// If texture is not in current folder, try parent folder
				if ( FAILED(D3DXCreateTextureFromFile(g_pd3dDevice,
					strTexture,
					&g_pMeshTextures_two[i])) ) {
					MessageBox (nullptr , TEXT("Could not find texture map") , TEXT("Meshes.exe") , MB_OK);
				}
			}
		}

	}

	SetupMatrices();
	// Done with the material buffer
	pD3DXMtrlBuffer->Release();

	return S_OK;
}

VOID Game::Cleanup() {
	if ( g_pMeshMaterials != nullptr )
		delete[] g_pMeshMaterials;

	if ( g_pMeshTextures ) {
		for ( DWORD i = 0; i < g_dwNumMaterials; i++ ) {
			if ( g_pMeshTextures[i] )
				g_pMeshTextures[i]->Release();
		}
		delete[] g_pMeshTextures;
	}
	if ( g_pMesh != nullptr )
		g_pMesh->Release();

	if ( g_pd3dDevice != nullptr )
		g_pd3dDevice->Release();

	if ( g_pD3D != nullptr )
		g_pD3D->Release();
}

VOID Game::SetupMatrices() {
	// For our world matrix, we will just leave it as the identity

	// Set up our view matrix. A view matrix can be defined given an eye point,
	// a point to lookat, and a direction for which way is up. Here, we set the
	// eye five units back along the z-axis and up three units, look at the 
	// origin, and define "up" to be in the y-direction.
	vEyePt = D3DXVECTOR3 (0.0f , 3.0f , -5.0f);
	vLookatPt = D3DXVECTOR3 (0.0f , 0.0f , 0.0f);
	vUpVec = D3DXVECTOR3 (0.0f , 1.0f , 0.0f);
	D3DXMATRIXA16 matView;
	D3DXMatrixLookAtLH (&matView , &vEyePt , &vLookatPt , &vUpVec);
	c.init (g_pd3dDevice,vEyePt, vLookatPt);
	

	// For the projection matrix, we set up a perspective transform (which
	// transforms geometry from 3D view space to 2D viewport space, with
	// a perspective divide making objects smaller in the distance). To build
	// a perpsective transform, we need the field of view (1/4 pi is common),
	// the aspect ratio, and the near and far clipping planes (which define at
	// what distances geometry should be no longer be rendered).
	D3DXMATRIXA16 matProj;
	D3DXMatrixPerspectiveFovLH (&matProj , D3DX_PI / 4 , 1.0f , 1.0f , 100.0f);
	g_pd3dDevice->SetTransform (D3DTS_PROJECTION , &matProj);
}




VOID Game::Render() {
	// Clear the backbuffer and the zbuffer
	g_pd3dDevice->Clear (0 , nullptr , D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER ,
	                     D3DCOLOR_XRGB(0,0,255) , 1.0f , 0);


	// Begin the scene
	if ( SUCCEEDED( g_pd3dDevice->BeginScene() ) ) {
		// Setup the world, view, and projection matrices

		if (  !firstSelected && !secondSelected) {
			if ( ::GetAsyncKeyState (VK_UP) ) {
				c.moveIn (-0.1f);
			}
			if ( ::GetAsyncKeyState (VK_DOWN) ) {
				c.moveIn (0.1f);
			}
			if ( ::GetAsyncKeyState (VK_LEFT) ) {
				c.pivit (-0.3f);
			}
			if ( ::GetAsyncKeyState (VK_RIGHT) ) {
				c.pivit (0.3f);
			}
		} 
		if (firstSelected ) {
			D3DXMATRIX world;

			if (::GetAsyncKeyState (VK_UP) ) {
				first.y += 0.01f;				
			}
			if ( ::GetAsyncKeyState (VK_DOWN) ) {
				first.y -= 0.01f;	
			}
			if ( ::GetAsyncKeyState (VK_LEFT) ) {
				first.x -= 0.01f;	
			}
			if ( ::GetAsyncKeyState (VK_RIGHT) ) {
				first.x += 0.01f;	
			}
			if ( ::GetAsyncKeyState ('U') ) {
				first.ya += 0.01f;	
			}
			if ( ::GetAsyncKeyState ('J') ) {
				first.ya -= 0.01f;	
			}
			if ( ::GetAsyncKeyState ('H') ) {
				first.xa -= 0.01f;	
			}
			if ( ::GetAsyncKeyState ('K') ) {
				first.xa += 0.01f;	
			}
			
			
			
		}
		if (secondSelected) {
			if (::GetAsyncKeyState (VK_UP) ) {
				second.y += 0.01f;				
			}
			if ( ::GetAsyncKeyState (VK_DOWN) ) {
				second.y -= 0.01f;	
			}
			if ( ::GetAsyncKeyState (VK_LEFT) ) {
				second.x -= 0.01f;	
			}
			if ( ::GetAsyncKeyState (VK_RIGHT) ) {
				second.x += 0.01f;	
			}
			if ( ::GetAsyncKeyState ('U') ) {
				second.ya += 0.01f;	
			}
			if ( ::GetAsyncKeyState ('J') ) {
				second.ya -= 0.01f;	
			}
			if ( ::GetAsyncKeyState ('H') ) {
				second.xa -= 0.01f;	
			}
			if ( ::GetAsyncKeyState ('K') ) {
				second.xa += 0.01f;	
			}
		}



		// Meshes are divided into subsets, one for each material. Render them in
		// a loop
		for ( DWORD i = 0; i < g_dwNumMaterials; i++ ) {
			// Set the material and texture for this subset
			g_pd3dDevice->SetMaterial (&g_pMeshMaterials[i]);
			g_pd3dDevice->SetTexture (0 , g_pMeshTextures[i]);

			D3DXMatrixTranslation (&matWorld ,first.x, first.y, first.z);
			D3DXMatrixRotationYawPitchRoll (&matWorld, first.xa, first.ya, first.za);
			

			g_pd3dDevice->SetTransform (D3DTS_WORLD , &matWorld);

			// Draw the mesh subset
			g_pMesh->DrawSubset (i);
		}

		for ( DWORD i = 0; i < g_dwNumMaterials_two; i++ ) {
			// Set the material and texture for this subset
			g_pd3dDevice->SetMaterial (&g_pMeshMaterials_two[i]);
			g_pd3dDevice->SetTexture (0 , g_pMeshTextures_two[i]);

			D3DXMatrixTranslation (&matWorld ,second.x, second.y, second.z);

			D3DXMatrixRotationYawPitchRoll (&matWorld, second.xa, second.ya, second.za);

			g_pd3dDevice->SetTransform (D3DTS_WORLD , &matWorld);

			// Draw the mesh subset
			g_pMesh_two->DrawSubset (i);
		}

		// End the scene
		g_pd3dDevice->EndScene();
	}

	// Present the backbuffer contents to the display
	g_pd3dDevice->Present (nullptr , nullptr , nullptr , nullptr);
}

VOID Game::MoveCamera(int type, float amountX, float amountY, float amountZ, float deg) {
	if ( type == 0 ) { //translation
		vEyePt = D3DXVECTOR3 (vEyePt.x + amountX , vEyePt.y + amountY , vEyePt.z + amountZ);
		vLookatPt = D3DXVECTOR3 (vLookatPt.x + amountX , vLookatPt.y + amountY , vLookatPt.z + amountZ);
	}
	else if ( type == 1 ) { //rotation

	}
	else { //both

	}
}


//-----------------------------------------------------------------------------
// Name: MsgProc()
// Desc: The window's message handler
//-----------------------------------------------------------------------------
LRESULT WINAPI MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) {
	switch ( msg ) {
		case WM_DESTROY:
			g.Cleanup();
			PostQuitMessage (0);
			return 0;
		case WM_KEYDOWN:
			switch ( wParam ) {
				case '1':
					g.firstSelected = !g.firstSelected;
					return 0;

				case '2':
					g.secondSelected = !g.secondSelected;
					return 0;

				case 'L':
					if ( g.ambient ) {
						g.ambient = false;
						g.g_pd3dDevice->SetRenderState (D3DRS_AMBIENT , D3DCOLOR_ARGB (0,0,0,0));
					}
					else {
						g.ambient = true;
						g.g_pd3dDevice->SetRenderState (D3DRS_AMBIENT , D3DCOLOR_ARGB (255,255,255,255));
					}
					return 0;
				case 'P': 
					if (g.point) {
						g.point = !g.point;
						g.g_pd3dDevice->LightEnable(0, FALSE);
					}else {
						g.point = !g.point;
						g.g_pd3dDevice->LightEnable(0, TRUE);
					}
					return 0;
				case 'S' :
					if (g.spot) {
						g.spot = !g.spot;
						g.g_pd3dDevice->LightEnable (1, FALSE);
					}else {
						g.spot = !g.spot;
						g.g_pd3dDevice->LightEnable (1, TRUE);
					}
					return 0;
				case 'D':
					if ( g.directional ) {
						g.directional = !g.directional;
						g.g_pd3dDevice->LightEnable (2 , FALSE);
					}
					else {
						g.directional = !g.directional;
						g.g_pd3dDevice->LightEnable (2 , TRUE);
					}
					return 0;
			}
			return 0;
	}

	return DefWindowProc (hWnd , msg , wParam , lParam);
}


//-----------------------------------------------------------------------------
// Name: WinMain()
// Desc: The application's entry point
//-----------------------------------------------------------------------------
INT WINAPI WinMain(HINSTANCE hInst, HINSTANCE, LPSTR, INT) {


	// Register the window class
	WNDCLASSEX wc = {sizeof(WNDCLASSEX), CS_HREDRAW | CS_VREDRAW | CS_OWNDC , MsgProc, 0L, 0L,
		GetModuleHandle (nullptr), nullptr, nullptr, nullptr, nullptr,
		TEXT("D3D Tutorial") , nullptr};
	RegisterClassEx (&wc);

	// Create the application's window
	HWND hWnd = CreateWindowEx (WS_EX_TOPMOST , TEXT("D3D Tutorial") , TEXT("D3D Tutorial 06: Meshes") ,
	                                         WS_POPUP | WS_SYSMENU | WS_VISIBLE , 0 , 0 , 512 , 512 ,
	                                         nullptr , nullptr , wc.hInstance , nullptr);

	// Initialize Direct3D
	if ( SUCCEEDED(g.InitD3D(hWnd)) ) {
		// Create the scene geometry
		if ( SUCCEEDED(g.InitGeometry()) ) {
			// Show the window
			ShowWindow (hWnd , SW_SHOWDEFAULT);
			UpdateWindow (hWnd);

			// Enter the message loop
			MSG msg;
			ZeroMemory(&msg, sizeof(msg));
			while ( msg.message != WM_QUIT ) {
				if ( PeekMessage (&msg , nullptr , 0U , 0U , PM_REMOVE) ) {
					TranslateMessage (&msg);
					DispatchMessage (&msg);
				}
				else
					g.Render();
				if ( GetAsyncKeyState (VK_ESCAPE) )
					PostQuitMessage (0);
			}
		}
	}

	UnregisterClass (TEXT("D3D Tutorial") , wc.hInstance);
	return 0;
}
