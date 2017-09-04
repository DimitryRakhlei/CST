#pragma once

struct MeshStruct {
	ID3DXMesh* mesh;
	std::vector<D3DMATERIAL9> mtrls;
	std::vector<IDirect3DTexture9*> tex;
	D3DXVECTOR3 pos;

	MeshStruct(ID3DXMesh* m, std::vector<D3DMATERIAL9> mtrl, std::vector<IDirect3DTexture9*> t, D3DXVECTOR3 p) {
		mesh = m;
		mtrls = mtrl;
		tex = t;
		pos = p;
	}
};

class Game {
public:
	bool Setup();
	void Cleanup();
	bool RenderMesh(MeshStruct mesh);
	bool Display(float timeDelta);
	void RenderScene();
	void RenderMirror();
};