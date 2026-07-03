shader_type canvas_item;

// Reads the combined pixel data of the game rendering underneath this ColorRect
uniform sampler2D screen_texture : hint_screen_texture, filter_nearest;

// The number of vertical scanlines. 
uniform float scanline_count = 216.0; 

// How dark the scanlines appear (0.0 is invisible, 1.0 is pitch black)
uniform float scanline_opacity : hint_range(0.0, 1.0) = 0.25;

void fragment() {
    // 1. Get the original pixel color from the game
    vec4 original_color = texture(screen_texture, SCREEN_UV);

    // 2. Generate a repeating wave based on the vertical screen coordinate (Y)
    // Multiplying by Pi ensures the math cleanly aligns with the top and bottom edges
    float wave = sin(SCREEN_UV.y * scanline_count * 3.14159265);
    
    // 3. Square the wave to keep the values positive and sharpen the line contrast
    wave = wave * wave; 

    // 4. Blend the original color with a darkened version of itself based on the wave
    COLOR = mix(original_color, original_color * wave, scanline_opacity);
}