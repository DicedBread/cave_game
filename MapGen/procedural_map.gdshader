shader_type canvas_item;

uniform vec2 screenDimen = vec2(0, 0);

uniform float value;

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec2 c = SCREEN_PIXEL_SIZE;
}

void light(){
	vec2 screenCenter = vec2(screenDimen.x/2.0, screenDimen.y/2.0);
	float val = distance(vec2(FRAGCOORD.x, FRAGCOORD.y), screenCenter);
	float stren = (screenDimen.x - val) / screenDimen.x;

	LIGHT = vec4(LIGHT_COLOR.rgb * COLOR.rgb * stren, stren);
}

// vec3 rgbToHsv(vec3 col){
// 	float max = max(col.r, max(col.g, col.b));
// 	float min = min(col.r, min(col.g, col.b));
	
// 	float diff = max - min;
// 	float h = -1.0;
// 	float s = -1.0;
	
// 	if(max == min){
// 		h = 0.0;
// 	}else{
		
// 	}
	
	
// 	return vec3()
// }

float distance(vec2 a, vec2 b){
	float x = pow(a.x - b.x, 2);
	float y = pow(a.y - b.y, 2);
	float val = sqrt(x + y);
	return val;
}
