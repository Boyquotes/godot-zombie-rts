using Godot;
using Godot.Collections;

namespace ZombieRTS.Scripts
{
    public class Unit : KinematicBody
    {
        [Export] public int Team;

        private readonly Array _teamColours = new Array
        {
            ResourceLoader.Load("res://TeamOneMaterial.tres"),
            ResourceLoader.Load("res://TeamTwoMaterial.tres")
        };

        private Vector3[] _path = new Vector3[0];
        private int _pathIndex;
        private const int MoveSpeed = 12;
        private Navigation _navigation;


        /**
         * Called when the node enters the scene tree for the first time.
         */
        public override void _Ready()
        {
            _navigation = (Navigation) GetParent();
            if (Team >= 0 && Team < _teamColours.Count)
            {
                GetNode<MeshInstance>("MeshInstance").MaterialOverride =
                    (Material) _teamColours[Team];
            }
        }


        /**
         * Move the unit to the given target position
         */
        public void MoveTo(Vector3 targetPosition)
        {
            GD.Print("MoveTo triggered.");
            _path = _navigation.GetSimplePath(GlobalTransform.origin, targetPosition);
            _pathIndex = 0;
        }


        /**
         * Accurate calculation step for game loop
         */
        public override void _PhysicsProcess(float delta)
        {
            if (_pathIndex >= _path.Length) return; // Do nothing
            var movementVector = _path[_pathIndex] - GlobalTransform.origin;
            if (movementVector.Length() < 0.1)
            {
                _pathIndex++; // Do the next move
            }
            else
            {
                /* Move, and dont merge into collider (slide vector to avoid intersecting)
                ie, hit wall and drag along it. Up is up, florin normal for calculating collision */
                MoveAndSlide(
                    movementVector.Normalized() * MoveSpeed,
                    new Vector3(0, 1, 0)
                );
            }
        }


        /**
         * Show the selection ring upon unit selection
         */
        public void Select()
        {
            GetNode<MeshInstance>("SelectionRing").Show();
        }


        /**
         * Show the selection ring upon unit deselection
         */
        public void Deselect()
        {
            GetNode<MeshInstance>("SelectionRing").Hide();
        }
    }
}