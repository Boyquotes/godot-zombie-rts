extends Spatial

const MOVE_MARGIN = 20
const MOVE_SPEED = 30 # units/s
const RAY_LENGTH = 1000

onready var cam = $Camera

var team = 0
var selected_units = []
onready var selection_box = $SelectionBox
var start_sel_pos = Vector2()


func _process(delta):
	var m_pos = get_viewport().get_mouse_position()
	calculate_movement(m_pos, delta)
	
	# Right click
	if Input.is_action_just_pressed("main_command"):
		#move_all_units(m_pos)
		move_selected_units(m_pos)
	
	# Left click
	if Input.is_action_just_pressed("alt_command"):
		selection_box.start_sel_pos = m_pos
		start_sel_pos = m_pos
		
	# Left click hold
	if Input.is_action_pressed("alt_command"):
		selection_box.m_pos = m_pos
		selection_box.is_visible = true
	else:
		selection_box.is_visible = false
		
	# Left click released (select units)
	if Input.is_action_just_released("alt_command"):
		select_units(m_pos)


# Move left/right forward/backward when mouse touches edge of screen
func calculate_movement(m_pos, delta):
	var v_size = get_viewport().size
	var move_vec = Vector3()
	if m_pos.x < MOVE_MARGIN:
		move_vec.x -= 1
	if m_pos.y < MOVE_MARGIN:
		move_vec.z -= 1
	if m_pos.x > v_size.x - MOVE_MARGIN:
		move_vec.x += 1
	if m_pos.y > v_size.y - MOVE_MARGIN:
		move_vec.z += 1
	
	move_vec = move_vec.rotated(Vector3(0, 1, 0), rotation_degrees.y)
	global_translate(move_vec * delta * MOVE_SPEED)


# May be useful for something oneday...
#func move_all_units(m_pos):
#	var result = raycast_from_mouse(m_pos, 1) # 1 for the environment (0000000001)
#	if result:
#		get_tree().call_group("units", "move_to", result.position)


func move_selected_units(m_pos):
	var result = raycast_from_mouse(m_pos, 1)
	if result:
		for unit in selected_units:
			unit.move_to(result.position)
			

# Select units depending on how clicked
func select_units(m_pos):
	var new_selected_units = []
	
	# Check click and drag
	if m_pos.distance_squared_to(start_sel_pos) < 16:
		# Single click (minimal drag)
		var unit = get_unit_under_mouse(m_pos)
		if unit != null:
			new_selected_units.append(unit)
	else:
		# Click and drag (box select)
		new_selected_units = get_units_in_box(start_sel_pos, m_pos)
		
	# Deselect old
	if new_selected_units.size() != 0:
		for unit in selected_units:
			unit.deselect()
		for unit in new_selected_units:
			unit.select()
		selected_units = new_selected_units
		
	


func get_unit_under_mouse(m_pos):
	var result = raycast_from_mouse(m_pos, 3) # 3 is collision mask for units and env
	# Return object if we hit something, and it has a team field, and that team is our team
	if result and "team" in result.collider and result.collider.team == team:
		return result.collider


func get_units_in_box(top_left, bottom_right):
	# verify top left is top left, otherwise swap
	if top_left.x > bottom_right.x:
		var tmp = top_left.x
		top_left.x = bottom_right.x
		bottom_right.x = tmp
	if top_left.y > bottom_right.y:
		var tmp = top_left.y
		top_left.y = bottom_right.y
		bottom_right.y = tmp
	
	# Create rectangle from coords
	var box = Rect2 (top_left, bottom_right - top_left)
	
	# Get units in the box
	var box_selected_units = []
	for unit in get_tree().get_nodes_in_group("units"):
		# For every unit in game, get those in our team, and in the box
		if unit.team == team and box.has_point(cam.unproject_position(unit.global_transform.origin)):
			box_selected_units.append(unit)
	
	return box_selected_units


func raycast_from_mouse(m_pos, collision_mask):
	var ray_start = cam.project_ray_origin(m_pos)
	var ray_end = ray_start + cam.project_ray_normal(m_pos) * RAY_LENGTH
	var space_state = get_world().direct_space_state
	return space_state.intersect_ray(ray_start, ray_end, [], collision_mask)
