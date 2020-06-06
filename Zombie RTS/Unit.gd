extends KinematicBody

export var team = 0
# Team materials (colours)
var team_colours = {
	0: preload("res://TeamOneMaterial.tres"),
	1: preload("res://TeamTwoMaterial.tres")
}

var path = []
var path_ind = 0
const move_speed = 12
onready var nav = get_parent()

func _ready():
	# Apply mesh material for the team if in one
	if team in team_colours:
		$MeshInstance.material_override = team_colours[team]

func move_to(target_pos):
	path = nav.get_simple_path(global_transform.origin, target_pos)
	path_ind = 0
	
func _physics_process(_delta):
	if path_ind < path.size():
		var move_vec = (path[path_ind] - global_transform.origin)
		if move_vec.length() < 0.1:
			# Do next move
			path_ind += 1
		else:
			# Up is up, florin normal for calculating collision
			move_and_slide(move_vec.normalized() * move_speed, Vector3(0, 1, 0))


# Show selection ring on unit selection
func select():
	$SelectionRing.show()

# Hide selection ring on unit deselection
func deselect():
	$SelectionRing.hide()
