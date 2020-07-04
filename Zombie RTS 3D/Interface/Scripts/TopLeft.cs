using Godot;
using Godot.Collections;
using ZombieRTS.Scripts;
using World = Godot.World;

namespace ZombieRTS.Interface.Scripts
{
    public class TopLeft : Control
    {
        private Camera _camera;
        private const int RayLength = 1000; // Max distance of mouse click

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNode<Button>("Buttons/UnitButton").Connect(
                "button_down", this, nameof(SpawnAndDragUnit));
            _camera = GetNode<Camera>("/root/World/CameraRig/Dolly/Camera");
            
                /*/root/World/Interface/TopBar/TopLeft
                /root/World/CameraRig/Dolly/Camera*/
            GD.Print("Button path = ", GetPath());
        }
        
        
            
        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
        }
        

        /**
         * Create a Unit instance on the mouse position
         */
        private void SpawnAndDragUnit()
        {
            var unit = new Unit();
            
            /*
             * TODO: Spawn unit on cursor, project up from terrain (make some mesh pole or something)
             * TODO: Set terrain generated to have env mask (1), navigation mesh, and collision mesh
             * TODO: On mouse release, drop unit - give unit gravity
             */
            
            // Get object under mask, using 1 collision mask for env
            var mousePosition = GetViewport().GetMousePosition();
            var result = RayCastFromMouse(mousePosition, 1);
            if (result.Count <= 0) return; // Do nothing...

            var unitTransform = unit.Transform;
            unitTransform.origin = (Vector3) result["position"];
            
            GD.Print("Results = ", result.ToString());
            
            AddChild(unit);

        }
        
        
        /**
		 * Get the mouse position on the "environment" mask upon intersection.
		 */
        private Dictionary RayCastFromMouse(Vector2 mousePosition, uint collisionMask)
        {
            var rayFrom = _camera.ProjectRayOrigin(mousePosition);
            var rayTo = rayFrom + _camera.ProjectRayNormal(mousePosition) * RayLength;
            var spaceState = GetNode<Spatial>("World").GetWorld().DirectSpaceState;

            return spaceState.IntersectRay(rayFrom, rayTo, new Array(), collisionMask);
        }
    }
}