# Grass System
The sea grass system uses skinned meshes and shapekeys to make grass sway back and forth

## Key information
- Author 
	> Models, scripts,etc. made by Riley Wood 

- Project Location
	> The sea grass system can be found in Assets>enviromental>SeaGrass.
	> The sea grass system also interacts with the edible sea grass in Assets>Gameplay>EdibleSeaGrass
- How to use the asset
  > Several prefabs have been created for the different types of grass. These have all required properties set up in advance
  > To add grass to the scene navigate to Assets>enviromental>SeaGrass then select a sub directory (ManateeGrass or ShoalGrass) and drag the prefabs from the folder into the scene.
  > The scene MUST HAVE a single object with the GrassWave script attached to it in order for the grass to animate
  > DO NOT rotate the grass prefabs. They are set up to animate along specific axies. To change the rotation look at

- How the asset works
  > The sea grass system uses skinned meshes and shapekeys to make grass sway back and forth.
  > This technique was chosen over several other available methods for the following reasons:
	1) Shader-based grass movement (usign a displacement shader to bend individual grass blades) was discarded since the unity shadergraph is only accessible through URP which at this popint was not functioning on the oculus headset. Perhaps someone with more developed technical skills could implement a version of this using the built-in render pipeline
	2) Animated grass movement (usign bones to create a swaying motion which would allow for better individual bending of grass blades) was discardedas testing found it to be about half as effecient as shape-key based grass movement even with a minimal rig and we needed a large quantity of seagrass 
  >
  >
  >	The grass meshes were set up as follows:
	1) A texture is painted using photoshop (or equivilent software)
		a) the alpha channel should make everything other than the individual blade of grass transparent
	2) A mesh is created using Blender (or equivilent software).
		a) Each blade of grass was made by having a plane follow a curve (curve modifier in Blender)
		b) The blade is duplicated and rotated 90 degrees such that the top-down veiw of the grass forms a cross. 
		c) Each of these blades is then duplicated and its normals are flipped since the built-in RP lacks facilities for double-sided rendering
		d) The blade is then duplicated and curved/rotated into new positions to form a clump of grass
		e) The Shape keys are created using a lattice to deform the model
			1) A lattice modifier is assigned to a collection of grass blades
			2) The lattice is asigned a defaul shape key, then positioned to bend the grass blades along each axis, with each being given its own shape key in the lattice
			3) Each lattice shape key is saved as a shape key to the grass mesh (modifiers>lattice>save as shape key). The shape keys should come in the following order (if they are not setup in the corrct order they will animate incorrectly): 
				0: X deformation
				1: -x deformation
				2: Y deformation
				3: -Y deformation
				4: stretch upwards on the Z axis
		f) The mesh is then exported as an FBX and imported to unity where it should appear as a skinned mesh. Dragging it into the scene and selecting it should show a "blend shapes" tab in the skinned mesh renderer which can be tested by hand
	3) A material is created using the standard template with the texture created above dragged into albedo and emission.
		a) The Rendering mode should be set to cutout and adjusted so that only the grass blade itself appears when rendered
		b) if you encounter issues with this, chech the texture import and make sure that the "Alpha is transparency" box is checked
	4) The sea grass mesh is given the tag "SeaGrass" and a GrassWave script is attached to a single object in the scene 
		(Only one of these scripts is required in the scene). Additionally the seagrass mesh has a GrassInitScript attached to it
	5) The grass init script must be given a value to determine what the starting direction will be (StartDirection box in the editor). To set this,
		enter a character (x, -x, y, -x) in the box according to how you want the grass to sway. Currently, all grass moves as follows: 
		0 degree Z axis rotation = x
		90 degree Z axis rotation = y
		180 degree Z axis rotation = -x
		270 degree Z axis rotation = -y
  >
  >
  > See the GrassWave script for a description of how the grasses are manipulated
  > See the grass Init script for a description of what the grasses do when the scene starts