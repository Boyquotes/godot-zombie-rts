using Godot;

namespace ZombieRTS.Scripts
{
    public class Player : KinematicBody
    {
        [Export] public float MaxSpeed = 150.0f;
        [Export] public float Acceleration = 4.5f;
        [Export] public float Deceleration = 16.0f;
        [Export] public float MaxSlopeAngle = 40.0f;
        [Export] public float MouseSensitivity = 0.05f;

        private Vector3 _vel;
        private Vector3 _dir;

        private Camera _camera;
        private Spatial _rotationHelper;


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _camera = GetNode<Camera>("RotationHelper/Camera");
            _rotationHelper = GetNode<Spatial>("RotationHelper");

            Input.SetMouseMode(Input.MouseMode.Captured);
        }


        public override void _PhysicsProcess(float delta)
        {
            ProcessInput();
            ProcessMovement(delta);
        }


        private void ProcessInput()
        {
            //  -------------------------------------------------------------------
            //  Walking
            _dir = new Vector3();
            var camXform = _camera.GlobalTransform;

            var inputMovementVector = new Vector2();

            if (Input.IsActionPressed("playerForward"))
                inputMovementVector.y += 1;
            if (Input.IsActionPressed("playerBackward"))
                inputMovementVector.y -= 1;
            if (Input.IsActionPressed("playerLeft"))
                inputMovementVector.x -= 1;
            if (Input.IsActionPressed("playerRight"))
                inputMovementVector.x += 1;

            inputMovementVector = inputMovementVector.Normalized();

            // Basis vectors are already normalized.
            _dir += -camXform.basis.z * inputMovementVector.y;
            _dir += camXform.basis.x * inputMovementVector.x;

            //  -------------------------------------------------------------------
            //  Capturing/Freeing the cursor
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                Input.SetMouseMode(Input.GetMouseMode() == Input.MouseMode.Visible
                    ? Input.MouseMode.Captured
                    : Input.MouseMode.Visible);
            }
        }


        private void ProcessMovement(float delta)
        {
            _dir.y = 0;
            _dir = _dir.Normalized();

            var hVel = _vel;
            hVel.y = 0;

            var target = _dir;

            target *= MaxSpeed;

            var acceleration = _dir.Dot(hVel) > 0 ? Acceleration : Deceleration;

            hVel = hVel.LinearInterpolate(target, acceleration * delta);
            _vel.x = hVel.x;
            _vel.z = hVel.z;
            _vel = MoveAndSlide(_vel, new Vector3(0, 1, 0),
                false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
        }


        public override void _Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseMotion mouseEvent) ||
                Input.GetMouseMode() != Input.MouseMode.Captured) return;

            _rotationHelper.RotateX(Mathf.Deg2Rad(-mouseEvent.Relative.y * MouseSensitivity));
            RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

            var cameraRot = _rotationHelper.RotationDegrees;
            cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
            _rotationHelper.RotationDegrees = cameraRot;
        }
    }
}