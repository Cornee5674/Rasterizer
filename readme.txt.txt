Cornee de Ruiter, 1524623

I have implemented all the minimum requirements.
As for the optional requirements, I have added the functionality for multiple light sources.

The sources I used are the following:
https://learnopengl.com/Getting-started/Camera
https://www.reddit.com/r/opengl/comments/s480zp/why_my_camera_is_rotating_around_world_origin/
https://gamedev.stackexchange.com/questions/104851/rotating-the-view-around-the-origin-of-the-world-coordinate-system
http://pyopengl.sourceforge.net/context/tutorials/shader_7.html
https://computergraphics.stackexchange.com/questions/5323/dynamic-array-in-glsl
https://community.khronos.org/t/uniform-array-length/72438/2

All the meshes are created by myself (except the teapots, they were provided), as was probably clear from the quality.

Screenshots are added in the screenshots folder.

The engine has very simple movement controls. WASD move the camera forward, left, backwards and right respectively. To ease navigation, I have also added functionality to go up and down.
This is done with the space bar and left shift. With the arrow keys, you can rotate the camera up and down, and, left and right. Navigation is multiplied with frametime, so slow pc's have the same 
movement speed as fast pc's.

The scenegraph has a very simple structure. I have created a class MeshNode, which stores a list of MeshNodes (children) and a mesh for itself. It also has a function to add a child.
In the scenegraph class, we create a base MeshNode, called world. This does not have a mesh. Every mesh that needs to be added can be done with world.AddChild(MeshNode x). 
All the MeshNodes that are added as children can themselves also have children. To do this, the statement just needs to be changed to childMeshNode.AddChild(MeshNode y).
All the children of a node inherit all of the translations that parents have, up to the world MeshNode. To demonstrate this, I have placed a car in the scene. The car has 4 children, the four wheels.
The car has an animation, and the wheels all move with the car. The wheels however have a separate rotating animation. To show that this structure can go infinitely deep, I have added a small cube
as a child to the left backside wheel. This is also rotating and moving, the rotation comes from the wheel parent, and the movement comes from the car (grand)parent.

I have implemented 3 different fragment shaders. A standard diffuse shader, a glossy-diffuse shader and a texture shader. The texture shader just applies the color from the specified texture with the uv
coordinates. The diffuse and glossy-diffuse shader calculate their color based on the light sources and their respecitve algorithms.

To pass multiple light sources to the shader, the List of vector3's needed to be converted to an array of floats. This is done in the Mesh class. 

