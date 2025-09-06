### About The HLSL Shader Source
In order to keep the Visual Studio installation uncluttered, the
HLSL shaders are compiled externally, and their source is kept in
this directory as non-copied content for reference.  The shaders will
probably only need to be re-compiled very infrequently.

The compiled shaders live in ``Helios/Resources`.

If you need to re-compiler, then you can install the compiler via the **"Game development with DirectX”** workload in the VS Installer.

Then from the Developer Command Prompt tool

`fxc /T ps_2_0 /E main /Fo ColorAdjust.psc ColorAdjust.ps`

WPF supports only Shader Model 2.0 (ps_2_0) — so don't go wild, keep your HLSL simple!

##### Note:
For use in WPF, the shader needs a wrapper class which
is specific to the requirements of the shader.  An example of this is in
`... /helios/controls/special/EffectColorAdjusterShaderWrapper.cs`