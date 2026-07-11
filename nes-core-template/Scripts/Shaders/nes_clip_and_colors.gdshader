shader_type canvas_item;

// Switched to UV boundaries (0.0 to 1.0) instead of screen pixels
instance uniform float clip_uv_top = -1.0;
instance uniform float clip_uv_bottom = -1.0;

// The exact colors currently on your sprite sheet
uniform vec4 color_1_original : source_color;
uniform vec4 color_1_replace : source_color;

uniform vec4 color_2_original : source_color;
uniform vec4 color_2_replace : source_color;

uniform vec4 color_3_original : source_color;
uniform vec4 color_3_replace : source_color;

uniform vec4 color_4_original : source_color;
uniform vec4 color_4_replace : source_color;

// A tiny tolerance handles slight blending/compression artifacts
uniform float tolerance : hint_range(0.0, 1.0) = 0.01;

void fragment() {
    // UV.y goes from 0.0 (top of sprite) to 1.0 (bottom of sprite)
    if (UV.y >= clip_uv_top && UV.y < clip_uv_bottom) {
        discard;
    }
	
	vec4 current_color = texture(TEXTURE, UV);

    // Swap Color 1 (Now applies the new Alpha value as well)
    if (distance(current_color.rgb, color_1_original.rgb) < tolerance && current_color.a > 0.0) {
        COLOR = color_1_replace;
    }
    // Swap Color 2 (Now applies the new Alpha value as well)
    else if (distance(current_color.rgb, color_2_original.rgb) < tolerance && current_color.a > 0.0) {
        COLOR = color_2_replace;
    }
	// Swap Color 3 (Now applies the new Alpha value as well)
    else if (distance(current_color.rgb, color_3_original.rgb) < tolerance && current_color.a > 0.0) {
        COLOR = color_3_replace;
    }
	// Swap Color 4 (Now applies the new Alpha value as well)
    else if (distance(current_color.rgb, color_4_original.rgb) < tolerance && current_color.a > 0.0) {
        COLOR = color_4_replace;
    }
    // Keep everything else the same
    else {
        COLOR = current_color;
    }
}