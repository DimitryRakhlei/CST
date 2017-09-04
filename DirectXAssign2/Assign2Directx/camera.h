#ifndef CAMERAH
#define CAMERAH
#include <d3dx9.h>
#include <d3d9.h>


class camera {
public:
	//Camera Administration
	void diagnostics(void);
	void init(IDirect3DDevice9*, D3DXVECTOR3 location, D3DXVECTOR3 lookAt);
	void init(IDirect3DDevice9*, D3DXVECTOR3 location, D3DXVECTOR3 lookAt, D3DXVECTOR3 up);
	void Cleanup(void);
	//Camera Position Translation Controls
	void zoom(float value);
	//Camera Position and LookAt Translation Controls
	void moveIn(float value);
	void pan(float horizontal, float vetical); //vertical is similar to lift
	//Camera Rotation Controls
	void pivit(float theat); //same as aircraft yaw
	void tilt(float theta); //same as aircraft pitch
	void roll(float theta);
	//Orbital Controls
	void geoSynchronousOrbit(float theta);
	void polarOrbit(float theta);
	//Manual Camera Updates
	void definePosition(D3DXVECTOR3 newLocation);
	void definePosition(D3DXVECTOR3 newLocation, D3DXVECTOR3 up);
	void defineLookAt(D3DXVECTOR3 newLocation);
	void defineLookAt(D3DXVECTOR3 newLocation, D3DXVECTOR3 up);
private:
	IDirect3DDevice9* Device;
	D3DXMATRIX View;
	D3DXVECTOR3 cameraPos;
	D3DXVECTOR3 cameraLookAt;
	D3DXVECTOR3 upVector;
	void makeItSo();
	void calculateDefaultUpVector();
};
#endif
