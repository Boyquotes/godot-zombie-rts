using Godot;
using ZombieRTS.Scripts;

namespace ZombieRTS.Interface.Scripts
{
    public class TopLeft : Control
    {
        private Button _button;
        private Camera _camera;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNode<Button>("Buttons/UnitButton").Connect(
                "button_down", this, nameof(SpawnAndDragUnit));
            _camera = GetNode<Camera>("Dolly/Camera");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
        }

        private void SpawnAndDragUnit()
        {
            var unit = new Unit();
            AddChild(unit);
            
            /*
             * TODO: Spawn unit on cursor, project up from terrain (make some mesh pole or something)
             * TODO: Set terrain generated to have env mask (1), navigation mesh, and collision mesh
             * TODO: On mouse release, drop unit - give unit gravity
             */
            
            
            var unitTransform = unit.Transform;
            unitTransform.origin = new Vector3(GetGlobalMousePosition().x, GetGlobalMousePosition().y, 0);
        }
    }
}