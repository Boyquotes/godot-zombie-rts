extends Control

var is_visible = false
var m_pos = Vector2()
var start_sel_pos = Vector2()
const SEL_BOX_COLOUR = Color(0, 1, 0)
const SEL_BOX_LINE_WIDTH = 3

# Drawn on update
func _draw():
	# And we've moved the mouse
	if is_visible and start_sel_pos != m_pos:
		# Draw 4 lines
		draw_line(start_sel_pos, Vector2(m_pos.x, start_sel_pos.y), SEL_BOX_COLOUR, SEL_BOX_LINE_WIDTH)
		draw_line(start_sel_pos, Vector2(start_sel_pos.x, m_pos.y), SEL_BOX_COLOUR, SEL_BOX_LINE_WIDTH)
		draw_line(m_pos, Vector2(m_pos.x, start_sel_pos.y), SEL_BOX_COLOUR, SEL_BOX_LINE_WIDTH)
		draw_line(m_pos, Vector2(start_sel_pos.x, m_pos.y), SEL_BOX_COLOUR, SEL_BOX_LINE_WIDTH)
		
func _process(delta):
	# Draw
	update()
