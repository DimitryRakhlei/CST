#include "d3dUtility.h"
#include "pSystem.h"
#include "Game.h"

//
// Globals
//

IDirect3DDevice9* Device = 0;

D3DXVECTOR3 view;

const int Width = 640;
const int Height = 480;

IDirect3DVertexBuffer9* VB = 0;

psys::PSystem* Sno = 0;

int selected = 0;


struct Mirror {
	IDirect3DTexture9* Tex;
	D3DMATERIAL9 Mtrl;

	Mirror(IDirect3DTexture9* t, D3DMATERIAL9 m) {
		Tex = t;
		Mtrl = m;
	}
};




struct Object {
	ID3DXMesh* mesh;
	D3DXVECTOR3 position;
	D3DMATERIAL9 material;

	Object(ID3DXMesh* m, D3DXVECTOR3 v, D3DMATERIAL9 mater) {
		mesh = m;
		position = v;
		material = mater;
	}
};

std::vector<MeshStruct> Meshes;

std::vector<Mirror> Mirrors{
	{0 ,d3d::WHITE_MTRL},
	{0,d3d::WHITE_MTRL},
	{0,d3d::WHITE_MTRL},
	{0,d3d::WHITE_MTRL},
	{0,d3d::WHITE_MTRL},
	{0,d3d::WHITE_MTRL}};


D3DXMATRIX World;
d3d::BoundingSphere BSphere;
std::vector<d3d::BoundingSphere> BSpheres;

void RenderScene();
void RenderMirror();

//
// Classes and Structures
//
struct Vertex {
	Vertex() {
	}

	Vertex(float x, float y, float z,
	       float nx, float ny, float nz,
	       float u, float v) {
		_x = x;
		_y = y;
		_z = z;
		_nx = nx;
		_ny = ny;
		_nz = nz;
		_u = u;
		_v = v;
	}

	float _x, _y, _z;
	float _nx, _ny, _nz;
	float _u, _v;

	static const DWORD FVF;
};

const DWORD Vertex::FVF = D3DFVF_XYZ | D3DFVF_NORMAL | D3DFVF_TEX1;


//global

int meshcount = 0;

bool LoadMesh(char * filename, int pos) {
	//temporary meshes
	ID3DXMesh* Mesh = nullptr;
	std::vector<D3DMATERIAL9> Mtrls(0);
	std::vector<IDirect3DTexture9*> Textures(0);

	// loading mesh
	HRESULT hr = 0;

	//
	// Load the XFile data.
	//

	ID3DXBuffer* adjBuffer = 0;
	ID3DXBuffer* mtrlBuffer = 0;
	DWORD numMtrls = 0;

	hr = D3DXLoadMeshFromX(
		filename,
		D3DXMESH_MANAGED,
		Device,
		&adjBuffer,
		&mtrlBuffer,
		0,
		&numMtrls,
		&Mesh);

	if (FAILED(hr)) {
		::MessageBox(0, "D3DXLoadMeshFromX() - FAILED", 0, 0);
		return false;
	}

	//
	// Extract the materials, and load textures.
	//

	if (mtrlBuffer != 0 && numMtrls != 0) {
		D3DXMATERIAL* mtrls = (D3DXMATERIAL*)mtrlBuffer->GetBufferPointer();

		for (int i = 0; i < numMtrls; i++) {
			// the MatD3D property doesn't have an ambient value set
			// when its loaded, so set it now:
			mtrls[i].MatD3D.Ambient = mtrls[i].MatD3D.Diffuse;

			// save the ith material
			Mtrls.push_back(mtrls[i].MatD3D);
		

			// check if the ith material has an associative texture
			if (mtrls[i].pTextureFilename != 0) {
				// yes, load the texture for the ith subset
				IDirect3DTexture9* tex = 0;
				D3DXCreateTextureFromFile(
					Device,
					mtrls[i].pTextureFilename,
					&tex);

				// save the loaded texture
				Textures.push_back(tex);
			}
			else {
				// no texture for the ith subset
				Textures.push_back(0);
			}
		}
	}
	d3d::Release<ID3DXBuffer*>(mtrlBuffer); // done w/ buffer

	//
	// Optimize the mesh.
	//

	hr = Mesh->OptimizeInplace(
		D3DXMESHOPT_ATTRSORT |
		D3DXMESHOPT_COMPACT |
		D3DXMESHOPT_VERTEXCACHE,
		(DWORD*)adjBuffer->GetBufferPointer(),
		0, 0, 0);

	d3d::Release<ID3DXBuffer*>(adjBuffer); // done w/ buffer

	if (FAILED(hr)) {
		::MessageBox(0, "OptimizeInplace() - FAILED", 0, 0);
		return false;
	}

	
		float ret[3];
		switch ( pos ) {
			case 0:
				ret[0] = 0.0f;
				ret[1] = 0.5f;
				ret[2] = -7.5f;
				break;
			case 1:
				ret[0] = 7.5f;
				ret[1] = 0.5f;
				ret[2] = 2.5f;
				break;
			case 2:
				ret[0] = 0.0f;
				ret[1] = 0.5f;
				ret[2] = 7.5f;
				break;
		}
	
	MeshStruct retstruct(Mesh, Mtrls, Textures, {ret[0],ret[1],ret[2]});
	Meshes.push_back(retstruct);
}


//
// Framework Functions
//


d3d::Ray CalcPickingRay(int x, int y) {
	float px = 0.0f;
	float py = 0.0f;

	D3DVIEWPORT9 vp;
	Device->GetViewport(&vp);

	D3DXMATRIX proj;
	Device->GetTransform(D3DTS_PROJECTION, &proj);

	px = (((2.0f*x) / vp.Width) - 1.0f) / proj(0, 0);
	py = (((-2.0f*y) / vp.Height) + 1.0f) / proj(1, 1);

	d3d::Ray ray;
	ray._origin = D3DXVECTOR3(0.0f, 0.0f, 0.0f);
	ray._direction = D3DXVECTOR3(px, py, 1.0f);

	return ray;
}

void TransformRay(d3d::Ray* ray, D3DXMATRIX* T) {
	// transform the ray's origin, w = 1.
	D3DXVec3TransformCoord(
		&ray->_origin,
		&ray->_origin,
		T);

	// transform the ray's direction, w = 0.
	D3DXVec3TransformNormal(
		&ray->_direction,
		&ray->_direction,
		T);

	// normalize the direction
	D3DXVec3Normalize(&ray->_direction, &ray->_direction);
}

bool RaySphereIntTest(d3d::Ray* ray, d3d::BoundingSphere* sphere) {
	D3DXVECTOR3 v = ray->_origin - sphere->_center;

	float b = 2.0f * D3DXVec3Dot(&ray->_direction, &v);
	float c = D3DXVec3Dot(&v, &v) - (sphere->_radius * sphere->_radius);

	// find the discriminant
	float discriminant = (b * b) - (4.0f * c);

	// test for imaginary number
	if (discriminant < 0.0f)
		return false;

	discriminant = sqrtf(discriminant);

	float s0 = (-b + discriminant) / 2.0f;
	float s1 = (-b - discriminant) / 2.0f;

	// if a solution is >= 0, then we intersected the sphere
	if (s0 >= 0.0f || s1 >= 0.0f)
		return true;

	return false;
}





bool Game::Setup() {

	LoadMesh("harrier.x", 0);
	LoadMesh("car.x", 1);

	//
	// Create Snow System.
	//

	d3d::BoundingBox boundingBox;
	boundingBox._min = D3DXVECTOR3(-10.0f, -10.0f, -10.0f);
	boundingBox._max = D3DXVECTOR3( 10.0f,  10.0f,  10.0f);
	Sno = new psys::Snow(&boundingBox, 5000);
	Sno->init(Device, "snowflake.dds");

	/*//
	// Create the teapot.
	//

	D3DXCreateTeapot(Device, &RenderObjects[0].mesh, 0);
	D3DXCreateSphere(Device, 1, 10, 10, &RenderObjects[1].mesh, 0);
	D3DXCreateSphere(Device, 1, 10, 10, &RenderObjects[2].mesh, 0);*/


	//
	// Compute the bounding sphere.
	//
	for (int i = 0; i < Meshes.size(); i++) {
		BYTE* byte = 0;
		d3d::BoundingSphere tempSphere;
		Meshes[i].mesh->LockVertexBuffer(0, reinterpret_cast<void**>(&byte));

		D3DXComputeBoundingSphere(
			reinterpret_cast<D3DXVECTOR3*>(byte),
			Meshes[i].mesh->GetNumVertices(),
			D3DXGetFVFVertexSize(Meshes[i].mesh->GetFVF()),
			&tempSphere._center,
			&tempSphere._radius);

		BSpheres.push_back(tempSphere);
		Meshes[i].mesh->UnlockVertexBuffer();
	}

	
	Device->CreateVertexBuffer(
		54 * sizeof(Vertex),
		0, // usage
		Vertex::FVF,
		D3DPOOL_MANAGED,
		&VB,
		0);

	Vertex* v = 0;
	VB->Lock(0, 0, reinterpret_cast<void**>(&v), 0);


	// mirror1 front 
	v[0] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[1] = Vertex(-2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[2] = Vertex(2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[3] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[4] = Vertex(2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[5] = Vertex(2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	// mirror2 right
	v[6] = Vertex(2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[7] = Vertex(2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[8] = Vertex(2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[9] = Vertex(2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[10] = Vertex(2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[11] = Vertex(2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	// mirror3 //left
	v[12] = Vertex(-2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[13] = Vertex(-2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[14] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[15] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[16] = Vertex(-2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[17] = Vertex(-2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	// mirror4 back
	v[18] = Vertex(-2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[19] = Vertex(2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[20] = Vertex(2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[21] = Vertex(-2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[22] = Vertex(2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[23] = Vertex(-2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	// mirror5 top 
	v[24] = Vertex(-2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[25] = Vertex(-2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[26] = Vertex(2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[27] = Vertex(2.5f, 2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[28] = Vertex(-2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[29] = Vertex(2.5f, 2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	// mirror6 bottom
	v[30] = Vertex(2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[31] = Vertex(2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f);
	v[32] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);

	v[33] = Vertex(-2.5f, -2.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f);
	v[34] = Vertex(2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f);
	v[35] = Vertex(-2.5f, -2.5f, 5.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f);

	VB->Unlock();

	//
	// Load Textures, set filters.
	//


	for (int i = 0; i < Mirrors.size(); i++) {
		IDirect3DTexture9* tex;
		D3DXCreateTextureFromFile(Device, "ice.bmp", &tex);
		Mirrors[i].Tex = tex;
	}


	Device->SetSamplerState(0, D3DSAMP_MAGFILTER, D3DTEXF_LINEAR);
	Device->SetSamplerState(0, D3DSAMP_MINFILTER, D3DTEXF_LINEAR);
	Device->SetSamplerState(0, D3DSAMP_MIPFILTER, D3DTEXF_LINEAR);

	//
	// Lights.
	//

	D3DXVECTOR3 lightDir(0.707f, -0.707f, 0.707f);
	D3DXCOLOR color(1.0f, 1.0f, 1.0f, 1.0f);
	D3DLIGHT9 light = d3d::InitDirectionalLight(&lightDir, &color);

	Device->SetLight(0, &light);
	Device->LightEnable(0, true);

	Device->SetRenderState(D3DRS_NORMALIZENORMALS, true);
	Device->SetRenderState(D3DRS_SPECULARENABLE, true);

	//
	// Set Camera.
	//

	D3DXVECTOR3 pos(-10.0f, 3.0f, -15.0f);
	D3DXVECTOR3 target(0.0, 0.0f, 0.0f);
	D3DXVECTOR3 up(0.0f, 1.0f, 0.0f);

	view = target - pos;

	D3DXMATRIX V;
	D3DXMatrixLookAtLH(&V, &pos, &target, &up);

	Device->SetTransform(D3DTS_VIEW, &V);

	//
	// Set projection matrix.
	//
	D3DXMATRIX proj;
	D3DXMatrixPerspectiveFovLH(
		&proj,
		D3DX_PI / 4.0f, // 45 - degree
		(float)Width / (float)Height,
		1.0f,
		1000.0f);
	Device->SetTransform(D3DTS_PROJECTION, &proj);

	return true;
}

void Game::Cleanup() {
	d3d::Release<IDirect3DVertexBuffer9*>(VB);
	for (auto m : Mirrors)
		d3d::Release<IDirect3DTexture9*>(m.Tex);
	for (auto it : Meshes) {
		d3d::Release<ID3DXMesh*>(it.mesh);
	}
	d3d::Delete<psys::PSystem*>( Sno );
}

bool Game::RenderMesh(MeshStruct mesh) {
	D3DXMATRIX world;
	D3DXMatrixTranslation (&world , mesh.pos.x , mesh.pos.y , mesh.pos.z);
	Device->SetTransform (D3DTS_WORLD , &world);
	for ( int i = 0; i < mesh.mtrls.size(); i++ ) {
		Device->SetMaterial (&mesh.mtrls[i]);
		Device->SetTexture (0 , mesh.tex[i]);
		mesh.mesh->DrawSubset (i);
	}
	return true;
}

bool Game::Display(float timeDelta) {
	if (Device) {
		//
		// Update the scene:
		//

		static float radius = 20.0f;

		if (::GetAsyncKeyState(VK_LEFT) & 0x8000f)
			Meshes[selected].pos.x -= 3.0f * timeDelta;

		if (::GetAsyncKeyState(VK_RIGHT) & 0x8000f)
			Meshes[selected].pos.x += 3.0f * timeDelta;

		if (::GetAsyncKeyState('W') & 0x8000f)
			radius -= 2.0f * timeDelta;

		if (::GetAsyncKeyState('S') & 0x8000f)
			radius += 2.0f * timeDelta;


		static float angle = (3.0f * D3DX_PI) / 2.0f;

		if (::GetAsyncKeyState('A') & 0x8000f)
			angle -= 0.5f * timeDelta;

		if (::GetAsyncKeyState('D') & 0x8000f)
			angle += 0.5f * timeDelta;

		D3DXVECTOR3 position(cosf(angle) * radius, 3.0f, sinf(angle) * radius);
		D3DXVECTOR3 target(0.0f, 0.0f, 0.0f);
		D3DXVECTOR3 up(0.0f, 1.0f, 0.0f);
		D3DXMATRIX V;
		view = target - position;
		D3DXMatrixLookAtLH(&V, &position, &target, &up);
		Device->SetTransform(D3DTS_VIEW, &V);

		Sno->update(timeDelta);

		//
		// Draw the scene:
		//
		Device->Clear(0, 0,
		              D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL,
		              0xff000000, 1.0f, 0L);

		Device->BeginScene();

		RenderScene();
		for (auto mesh : Meshes) {
			RenderMesh(mesh);
		}

		RenderMirror();

		D3DXMATRIX I;
		D3DXMatrixIdentity(&I);
		Device->SetTransform(D3DTS_WORLD, &I);

		// order important, render snow last.
		Device->SetTransform(D3DTS_WORLD, &I);
		Sno->render();

		Device->EndScene();
		Device->Present(0, 0, 0, 0);
	}
	return true;
}

void Game::RenderScene() {
	
	for (auto mesh : Meshes) {
		// TODO : add rendering
	}

	D3DXMATRIX I;
	D3DXMatrixIdentity(&I);
	Device->SetTransform(D3DTS_WORLD, &I);

	Device->SetStreamSource(0, VB, 0, sizeof(Vertex));
	Device->SetFVF(Vertex::FVF);


	// draw the mirror
	for (auto mirror : Mirrors) {
		Device->SetMaterial(&mirror.Mtrl);
		Device->SetTexture(0, mirror.Tex);
	}
	Device->DrawPrimitive(D3DPT_TRIANGLELIST, 0, 12); //original 2
}


D3DXPLANE Planemaker(int i);

void Game::RenderMirror() {
	//
	// Draw Mirror quad to stencil buffer ONLY.  In this way
	// only the stencil bits that correspond to the mirror will
	// be on.  Therefore, the reflected teapot can only be rendered
	// where the stencil bits are turned on, and thus on the mirror 
	// only.
	//

	for (auto i = 0; i < 5; i++) {

		auto pl = Planemaker(i);
		if (D3DXPlaneDotNormal(&pl, &view) < 0) continue;

		//clear stencil buffer
		Device->Clear(0, 0,  D3DCLEAR_STENCIL, NULL, 1.0f, 0);


		Device->SetRenderState(D3DRS_STENCILENABLE, true);
		Device->SetRenderState(D3DRS_STENCILFUNC, D3DCMP_ALWAYS);
		Device->SetRenderState(D3DRS_STENCILREF, 0x1);
		Device->SetRenderState(D3DRS_STENCILMASK, 0xffffffff);
		Device->SetRenderState(D3DRS_STENCILWRITEMASK, 0xffffffff);
		Device->SetRenderState(D3DRS_STENCILZFAIL, D3DSTENCILOP_KEEP);
		Device->SetRenderState(D3DRS_STENCILFAIL, D3DSTENCILOP_KEEP);
		Device->SetRenderState(D3DRS_STENCILPASS, D3DSTENCILOP_REPLACE);

	

		// disable writes to the depth and back buffers
		Device->SetRenderState(D3DRS_ZWRITEENABLE, false);
		Device->SetRenderState(D3DRS_ALPHABLENDENABLE, true);
		Device->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_ZERO);
		Device->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE);


		// draw the mirror to the stencil buffer
		Device->SetStreamSource(0, VB, 0, sizeof(Vertex));
		Device->SetFVF(Vertex::FVF);
		Device->SetMaterial(&Mirrors[i].Mtrl);
		Device->SetTexture(0, Mirrors[i].Tex);
		D3DXMATRIX I;
		D3DXMatrixIdentity(&I);
		Device->SetTransform(D3DTS_WORLD, &I);
		Device->DrawPrimitive(D3DPT_TRIANGLELIST, i * 6, 2);

		// re-enable depth writes
		Device->SetRenderState(D3DRS_ZWRITEENABLE, true);


		// only draw reflected teapot to the pixels where the mirror
		// was drawn to.
		Device->SetRenderState(D3DRS_STENCILFUNC, D3DCMP_EQUAL);
		Device->SetRenderState(D3DRS_STENCILPASS, D3DSTENCILOP_KEEP);


		// position reflection
		D3DXMATRIX W, T, R;
		D3DXPLANE plane = Planemaker(i);

		D3DXMatrixReflect(&R, &plane);


		// clear depth buffer and blend the reflected teapot with the mirror
		Device->Clear(0, 0, D3DCLEAR_ZBUFFER, 0, 1.0f, 0);
		Device->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_DESTCOLOR);
		Device->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ZERO);

		// Turn on clipping plane to prevent spillage between mirror-faces
		Device->SetClipPlane(0, plane);
		Device->SetRenderState(D3DRS_CLIPPLANEENABLE, D3DCLIPPLANE0);

		for (auto obj : Meshes) {

			D3DXMatrixTranslation(&T,
			                      obj.pos.x,
			                      obj.pos.y,
			                      obj.pos.z);

			W = T * R;
			


			Device->SetTransform (D3DTS_WORLD , &W);
			for ( int i = 0; i < obj.mtrls.size(); i++ ) {
				Device->SetMaterial (&obj.mtrls[i]);
				Device->SetTexture (0 , obj.tex[i]);
				obj.mesh->DrawSubset (i);
			}


			

			Device->SetRenderState(D3DRS_CULLMODE, D3DCULL_CW);
			obj.mesh->DrawSubset(0);

			
		}

		Device->SetRenderState (D3DRS_CLIPPLANEENABLE, false);

		// Restore render states.
		Device->SetRenderState(D3DRS_ALPHABLENDENABLE, false);
		Device->SetRenderState(D3DRS_STENCILENABLE, false);
		Device->SetRenderState(D3DRS_CULLMODE, D3DCULL_CCW);

	}
}


// front right left back top bottom
D3DXPLANE Planemaker(int i) {
	D3DXPLANE retplane;
	switch (i) {
	case 0:
		retplane = D3DXPLANE(0.0f, 0.0f, 1.0f, 2.5f); //frong
		break;
	case 1:
		retplane = D3DXPLANE(-1.0f, 0.0f, 0.0f, 2.5f); //right
		break;
	case 2:
		retplane = D3DXPLANE(1.0f, 0.0f, 0.0f, 2.5f); // left
		break;
	case 3:
		retplane = D3DXPLANE(0.0f, 0.0f, -1.0f, 2.5f); // back
		break;
	case 4:
		retplane = D3DXPLANE(0.0f, -1.0f, 0.0f, 0.0f); //top
		break;
	case 5:
		retplane = D3DXPLANE(0.0f, 1.0f, 0.0f, 5.0f); //bottom
		break;
	}
	return retplane;
}

//
// WndProc
//
LRESULT CALLBACK d3d::WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam) {
	switch (msg) {
	case WM_DESTROY:
		::PostQuitMessage(0);
		break;

	case WM_KEYDOWN:
		if (wParam == VK_ESCAPE)
			::DestroyWindow(hwnd);
		break;

	case WM_LBUTTONDOWN:

		// compute the ray in view space given the clicked screen point
		d3d::Ray ray = CalcPickingRay(LOWORD(lParam), HIWORD(lParam));

		// transform the ray to world space
		D3DXMATRIX view;
		Device->GetTransform(D3DTS_VIEW, &view);

		D3DXMATRIX viewInverse;
		D3DXMatrixInverse(&viewInverse, 0, &view);

		TransformRay(&ray, &viewInverse);


		for ( int i = 0; i < BSpheres.size(); i++ ) {
			auto sphere = BSpheres[i];
			// test for a hit
			if ( RaySphereIntTest (&ray , &sphere) ) {
				::MessageBox (0 , "Hit!" , "HIT" , 0);
				selected = i;
			}
		}
		
	}
	return ::DefWindowProc(hwnd, msg, wParam, lParam);
}

//
// WinMain
//
int WINAPI WinMain(HINSTANCE hinstance,
                   HINSTANCE prevInstance,
                   PSTR cmdLine,
                   int showCmd) {
	if (!d3d::InitD3D(hinstance,
	                  Width, Height, true, D3DDEVTYPE_HAL, &Device)) {
		::MessageBox(0, "InitD3D() - FAILED", 0, 0);
		return 0;
	}

	Game g;

	if (!g.Setup()) {
		::MessageBox(0, "Setup() - FAILED", 0, 0);
		return 0;
	}

	d3d::EnterMsgLoop(g);

	g.Cleanup();

	Device->Release();

	return 0;
}
