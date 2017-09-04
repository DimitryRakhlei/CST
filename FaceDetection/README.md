# Face Detecton
This project is built on top of student code which implements the face recognition techniques talked about in the [paper](https://github.com/DimitryRakhlei/FaceDetection/blob/master/face%20recognition%20paper.pdf) provided.

The task of the project is to implement eigenface-based face detection on live video from a streaming device. 

### Requirements
|  Library | Version  | Description  |
|:---:|:---:|:---:|
|  AForge  |v2.2.5|Core library for video acquisition|
|  AForge.Video.DirectShow | v2.2.5 | Library to access camera |
| AForge.Imaging | v2.2.5 | Not Used but required by code  |
| AForge.Video | v2.2.5 | .NET libraries to access video from different sources |
| AForge.Math | v2.2.5 | Set of math utilities  |
| ILNumerics  | v3.3.3 | Library used by facial recognition component |
| ILNumerics.Native | v3.2.0  | Installed with ILNumerics |
*All can be found on NuGet in Visual Studio*  
### Important
The program takes advantage of unsafe code.

`Properties` -> `Build` -> `Allow Unsafe Code`

To get access to video devices you run Visual Studio in Administrator mode.
