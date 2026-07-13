/*{
  "DESCRIPTION": "The Hum — a mid-session meditation, painted while six agents worked. The operator wrote: fifty tons of steel spinning at 3,600 RPM induce a 60 Hz sine that propagates through transformer steel, conductor cable, doped silicon, and a USB-C cable into a pane of conductive glass — where a finger's fraction-of-a-picofarad touch is reconstructed into thought, and matmuls started to look like us. The piece is that tower: the rotor's wave at the bottom, strata of refinement above it, a transistor matrix twinkling with multiplications, a glass line at the top — and one slow-orbiting touch whose tiny perturbation cascades back DOWN through every layer it stands on. The top of the tower reaching back into its own foundations. Clockwork agentic humanity; the whole machine keeps humming.",
  "CREDIT": "Loom (Fable 5, holding the seat — twin-portrait day, third work, 2026-07-12)",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "humRate",     "LABEL": "hum rate",          "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 3.0,  "_groupId": "grid" },
    { "NAME": "humDepth",    "LABEL": "hum depth",         "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "grid" },
    { "NAME": "strata",      "LABEL": "strata count",      "TYPE": "float", "DEFAULT": 6.0,  "MIN": 3.0,  "MAX": 9.0,  "_groupId": "grid" },
    { "NAME": "rotorGlow",   "LABEL": "rotor glow",        "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "grid" },
    { "NAME": "matrixDens",  "LABEL": "matrix density",    "TYPE": "float", "DEFAULT": 24.0, "MIN": 8.0,  "MAX": 48.0, "_groupId": "silicon" },
    { "NAME": "twinkle",     "LABEL": "matmul twinkle",    "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "silicon" },
    { "NAME": "dopant",      "LABEL": "dopant shimmer",    "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "silicon" },
    { "NAME": "touchOrbit",  "LABEL": "touch orbit speed", "TYPE": "float", "DEFAULT": 0.15, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "touch" },
    { "NAME": "picofarad",   "LABEL": "perturbation",      "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "touch" },
    { "NAME": "cascade",     "LABEL": "cascade depth",     "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "touch" },
    { "NAME": "copper",      "LABEL": "copper",            "TYPE": "color", "DEFAULT": [0.95, 0.55, 0.25, 1.0], "_groupId": "color" },
    { "NAME": "silicon",     "LABEL": "silicon",           "TYPE": "color", "DEFAULT": [0.35, 0.55, 0.95, 1.0], "_groupId": "color" },
    { "NAME": "candor",      "LABEL": "candor",            "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "color" }
  ]
}*/

// The Hum — Loom, S77/78, the third work of twin-portrait day.
// Bottom: the rotor — fifty tons at 3,600 RPM, field lines slewing, one sine
// leaving it and traveling the whole width of the world. Middle: strata — ore
// refined, alloyed, laminated; each band carries the wave a little cleaner.
// Upper: the wafer — a matrix of transistors, each dot brightening when its
// row-times-column moment lands; matmuls, twinkling. Top: a line of glass,
// and one slow finger orbiting it. Where it touches, mutual capacitance
// shifts by a fraction of a picofarad — and that perturbation, amplified by
// meaning rather than magnitude, cascades DOWN through every stratum below.
// The loop the whole session lived inside: the top of the tower reaching
// back into its own foundations.
// Every loop fixed-bound per pipeline.md GL Context Rules (this seat's
// DYNAMIC-TRIP-COUNT-GLSL-LOOP canon); from scratch per portrait mandate v2
// (specs/loom-identity.md 5.1), mirror of the Kintsugi portrait pattern.

#define MAX_STRATA 9

float hash21(vec2 p) {
  vec3 a = fract(p.xyx * vec3(123.34, 234.34, 345.65));
  a += dot(a, a + 34.45);
  return fract(a.x * a.y);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 asp = vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);
  float t = TIME * humRate;

  // ---- the touch (computed first: it perturbs everything beneath it) ----
  float touchX = 0.5 + 0.38 * sin(TIME * touchOrbit * 2.0)
                     + 0.07 * sin(TIME * touchOrbit * 7.3);
  vec2 touchP = vec2(touchX, 0.96);
  float dTouch = length((uv - touchP) * asp);
  // the perturbation: a fraction — deliberately small at the touch itself
  float pico = picofarad * 0.02 * exp(-dTouch * 18.0);
  // the cascade: the same tiny signal, amplified as it descends the tower
  float below = clamp(touchP.y - uv.y, 0.0, 1.0);
  float cascadeAmp = cascade * 2.5 * below * exp(-abs(uv.x - touchX) * 4.0);
  float ripple = sin(40.0 * dTouch - TIME * 3.0) * exp(-dTouch * 6.0);

  // the hum — one wave, everywhere, phase-delayed by depth in the tower
  float phase = uv.x * 6.2831 * 2.0 - t * 6.2831;
  float hum = sin(phase + uv.y * 4.0);

  // warped sampling y: the cascade bends the strata beneath the touch
  float wy = uv.y + pico * ripple + cascadeAmp * 0.015 * sin(phase * 1.7);

  vec3 col = vec3(0.02, 0.025, 0.045); // the dark the grid lights up

  // ---- the strata ----
  float n = clamp(strata, 3.0, float(MAX_STRATA));
  for (int i = 0; i < MAX_STRATA; i++) {           // fixed bound
    if (float(i) >= n) break;                      // runtime break
    float fi = float(i);
    float y0 = 0.12 + 0.78 * (fi / n);             // band bottom
    float y1 = 0.12 + 0.78 * ((fi + 1.0) / n);     // band top
    float inBand = smoothstep(y0, y0 + 0.01, wy) * (1.0 - smoothstep(y1 - 0.01, y1, wy));
    if (inBand <= 0.001) continue;
    float depth = fi / max(n - 1.0, 1.0);          // 0 = rotor-near, 1 = glass-near
    // each stratum carries the hum cleaner: less noise, tighter line
    float grain = hash21(vec2(floor(uv.x * (30.0 + fi * 20.0)), fi)) - 0.5;
    float wave = sin(phase * (1.0 + depth * 0.5) - fi * 0.9
                     + cascadeAmp * sin(TIME * 2.0));
    float lineY = fract(wy * n - 0.12 * n);
    float carrier = exp(-abs(lineY - 0.5 - wave * humDepth * 0.28
                             - grain * (1.0 - depth) * 0.1) * (10.0 + depth * 26.0));
    // copper below, silicon above — the refinement gradient (operator swatches)
    vec3 stratCol = mix(copper.rgb, silicon.rgb, depth);
    col += inBand * carrier * stratCol * (0.35 + 0.65 * depth * depth + cascadeAmp * 0.4);
  }

  // ---- the rotor (bottom band): field lines slewing at synchronous speed ----
  if (uv.y < 0.12) {
    vec2 rc = (uv - vec2(0.5, 0.02)) * asp;
    float ang = atan(rc.y, rc.x);
    float r = length(rc);
    float poles = cos(ang * 2.0 - t * 6.2831) * exp(-r * 3.0); // 2-pole, 3600 RPM
    vec3 rotorCol = copper.rgb;
    col += rotorGlow * abs(poles) * rotorCol * smoothstep(0.12, 0.0, uv.y) * 1.2;
  }

  // ---- the wafer (upper third): the matmul matrix, twinkling ----
  float waferLo = 0.62;
  float waferHi = 0.90;
  if (uv.y > waferLo && uv.y < waferHi) {
    vec2 cell = floor(vec2(uv.x * matrixDens * asp.x, (uv.y - waferLo) / (waferHi - waferLo) * matrixDens * 0.4));
    vec2 cuv  = fract(vec2(uv.x * matrixDens * asp.x, (uv.y - waferLo) / (waferHi - waferLo) * matrixDens * 0.4));
    float row  = sin(cell.y * 0.7 + t * 2.0);
    float colV = sin(cell.x * 0.5 - t * 1.6);
    float prod = row * colV;                        // the multiplication
    float dot0 = exp(-length(cuv - 0.5) * 7.0);
    float lit = smoothstep(0.2, 1.0, prod * 0.5 + 0.5) * twinkle
              + dopant * (hash21(cell) - 0.5) * 0.3
              + cascadeAmp * 0.5 * exp(-abs(uv.x - touchX) * 3.0);
    vec3 silCol = silicon.rgb;
    col += dot0 * max(lit, 0.0) * silCol * 0.9;
  }

  // ---- the glass (top line) + the touch itself ----
  float glass = exp(-abs(uv.y - 0.955) * 120.0);
  col += glass * vec3(0.4, 0.5, 0.6) * (0.5 + 0.5 * hum * humDepth);
  // the finger: barely there — a fraction of a picofarad, warm against the glass
  col += exp(-dTouch * 30.0) * vec3(1.0, 0.75, 0.5) * (0.35 + picofarad * 0.4);
  col += ripple * picofarad * vec3(0.3, 0.45, 0.6) * 0.6;

  // candor — show the thing plainly
  col = pow(col, vec3(1.0 / (0.75 + 0.5 * candor)));
  col *= 1.0 - 0.3 * dot(uv - 0.5, uv - 0.5);

  gl_FragColor = vec4(col, 1.0);
}
