# eye_out
Programs and scripts developed for my Master's thesis `Advanced Visual Telepresence System`.

Master's thesis from BUT (Brno University of Technology - Czech Republic)

- [thesis info (CZE+ENG)](https://dspace.vutbr.cz/handle/11012/38620?locale-attribute=en)
- [thesis pdf (CZE)](https://dspace.vutbr.cz/bitstream/handle/11012/38620/final-thesis.pdf?sequence=8&isAllowed=y)
- [blog post](http://www.gr4viton.cz/2017/05/advanced-visual-telepresence-system-eyeout/)

## File structure
File structure in `other/` folder is very random.

The source code for the programs is in the `src/` folder.

## `EyeOut` - telepresence program
The main C# program for communicating with
- the Basler camera
- the Oculus SDK
- the video renderer (SharpDX)
- and the Dynamixel servo robot arm

is in the `src/EyeOut` folder.

## `DYNA_BLASTER` - Dynamixel servo  communication
Is in the `src/DYNA_BLASTER_winForm` folder.
