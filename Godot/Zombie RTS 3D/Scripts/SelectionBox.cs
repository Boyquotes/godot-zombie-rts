using Godot;

namespace ZombieRTS.Scripts
{
    public class SelectionBox : Control
    {
        public Vector2 MousePosition = new Vector2();
        public Vector2 StartSelectionPosition = new Vector2();
        private static readonly Color SelectionBoxColour = new Color(0, 1, 0);
        private const int SelectionBoxLineWidth = 3;

        /**
         * Called when the node enters the scene tree for the first time.
         */
        public override void _Ready()
        {
        }


        /**
         * Called every frame. 'delta' is the elapsed time since the previous frame.
         */
        public override void _Process(float delta)
        {
            Update(); // Draw changes
        }


        /**
         * Drawn on update
         */
        public override void _Draw()
        {
            // If visible and mouse moved, draw four lines to make selection box
            if (!Visible || StartSelectionPosition == MousePosition) return; // Do nothing

            // Top line
            DrawLine(
                StartSelectionPosition,
                new Vector2(MousePosition.x, StartSelectionPosition.y),
                SelectionBoxColour, SelectionBoxLineWidth
            );
            // Left line
            DrawLine(
                StartSelectionPosition,
                new Vector2(StartSelectionPosition.x, MousePosition.y),
                SelectionBoxColour, SelectionBoxLineWidth
            );
            // Right line
            DrawLine(
                MousePosition,
                new Vector2(MousePosition.x, StartSelectionPosition.y),
                SelectionBoxColour, SelectionBoxLineWidth
            );
            // Bottom line
            DrawLine(
                MousePosition,
                new Vector2(StartSelectionPosition.x, MousePosition.y),
                SelectionBoxColour, SelectionBoxLineWidth
            );
        }
    }
}