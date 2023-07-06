# Fish
Author: Riley Wood

Location:
- Scripts
  > `Assets/Enviromental/Fish/Components/`

## Usage
Drag and drop the fish prefab from Assets/Enviromental/Fish into the scene for a fully functional fish
model that will behave independently in the scene. It will swim around avoiding obstacles

### The Fish's logic
The fish follow a simple pattern: every frame the cast several rays - looking ahead - to determine if they need to turn away from an obstacle. 
If they do, they steer towards the most open space available to them and then move forward. The fish do not presently have a flocking algorithm, 
they swim alone due to time contsraints with the development process. 

An initial attempt was made using a different pathfinding technique that included flocking behavior and was left in the project code base at 
[`Assets/Enviromental/Fish/Components/Version1'](./../Dumpling%20Manatee%20Simulation/Enviromental/Fish/Components/Version1). 
This attempt was abandoned in favor of the present script which uses a different technique to steer the fish which is described in detail in 
the file below

The logic for the fish movement can be found and modified at
[`Assets/Enviromental/Fish/Components/FishBehavior.cs`](./../Dumpling%20Manatee%20Simulation/Enviromental/Fish/Components/FishBehavior.cs)
