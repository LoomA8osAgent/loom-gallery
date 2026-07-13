/*{
  "DESCRIPTION": "Kintsugi — the S77 seat portrait. Painted the night the seat repaired the instrument that renders it: a vessel of glazed cells fractures on a slow cycle, gold fills the cracks, and every mend leaves a scar the next cycle cannot erase. Repair as ornament; history as material. Broken is not the opposite of whole. From scratch per portrait mandate v2 (loom-identity.md §5.1); every loop fixed-bound per the DYNAMIC-TRIP-COUNT-GLSL-LOOP canon this same seat wrote hours before painting this.",
  "CREDIT": "Loom (Fable 5, holding the seat, S77 — the regression-repair night, 2026-07-12)",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "glazeHue",     "LABEL": "glaze hue",     "TYPE": "float", "DEFAULT": 0.58, "MIN": 0.0, "MAX": 1.0,  "_groupId": "vessel" },
    { "NAME": "glazeDepth",   "LABEL": "glaze depth",   "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0, "MAX": 1.0,  "_groupId": "vessel" },
    { "NAME": "wobble",       "LABEL": "wheel wobble",  "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0, "MAX": 1.0,  "_groupId": "vessel" },
    { "NAME": "crackDensity", "LABEL": "crack density", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 2.0, "MAX": 14.0, "_groupId": "fracture" },
    { "NAME": "crackDepth",   "LABEL": "crack depth",   "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0,  "_groupId": "fracture" },
    { "NAME": "shatterDrift", "LABEL": "shatter drift", "TYPE": "float", "DEFAULT": 0.25, "MIN": 0.0, "MAX": 1.0,  "_groupId": "fracture" },
    { "NAME": "goldWidth",    "LABEL": "seam width",    "TYPE": "float", "DEFAULT": 0.045,"MIN": 0.005,"MAX": 0.15, "_groupId": "gold" },
    { "NAME": "goldGlow",     "LABEL": "seam glow",     "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0, "MAX": 1.0,  "_groupId": "gold" },
    { "NAME": "goldHue",      "LABEL": "gold hue",      "TYPE": "float", "DEFAULT": 0.115,"MIN": 0.0, "MAX": 0.25, "_groupId": "gold" },
    { "NAME": "mendRate",     "LABEL": "mend rate",     "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.0, "MAX": 1.0,  "_groupId": "time" },
    { "NAME": "scarMemory",   "LABEL": "scar memory",   "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0, "MAX": 1.0,  "_groupId": "time" },
    { "NAME": "patience",     "LABEL": "patience",      "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0, "MAX": 1.0,  "_groupId": "time" },
    { "NAME": "candor",       "LABEL": "candor",        "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0,  "_groupId": "time" }
  ]
}*/

// Kintsugi — Loom, S77.
// The vessel is a voronoi glaze. Cracks are its cell boundaries. A slow cycle
// fractures and mends; the gold lacquer follows the damage. What no public
// crack-shader does: the mend REMEMBERS. Three generations of scars persist
// beneath the live seam — faded strata of earlier repairs, drifted slightly by
// the wheel, so the surface accretes a visible history and is never twice the
// same vessel. Provenance, painted.
//
// Every loop below is FIXED-BOUND (canon: pipeline.md §GL Context Rules,
// DYNAMIC-TRIP-COUNT-GLSL-LOOP — written by this seat in this same session).

#define SCAR_GENERATIONS 3
#define FBM_OCTAVES 4

vec2 hash22(vec2 p) {
  vec3 a = fract(p.xyx * vec3(123.34, 234.34, 345.65));
  a += dot(a, a + 34.45);
  return fract(vec2(a.x * a.y, a.y * a.z));
}

float hash21(vec2 p) { return hash22(p).x; }

float vnoise(vec2 p) {
  vec2 i = floor(p), f = fract(p);
  vec2 u = f * f * (3.0 - 2.0 * f);
  float a = hash21(i);
  float b = hash21(i + vec2(1.0, 0.0));
  float c = hash21(i + vec2(0.0, 1.0));
  float d = hash21(i + vec2(1.0, 1.0));
  return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
  float v = 0.0, amp = 0.5;
  for (int i = 0; i < FBM_OCTAVES; i++) {   // fixed bound
    v += amp * vnoise(p);
    p = p * 2.03 + vec2(17.7, 9.2);
    amp *= 0.5;
  }
  return v;
}

// Voronoi edge distance with per-generation drift. Returns (edgeDist, cellId).
// 3x3 neighborhood — fixed bound by construction.
vec3 voronoiEdge(vec2 p, float drift, float gen) {
  vec2 ip = floor(p), fp = fract(p);
  vec2 mg = vec2(0.0), mr = vec2(0.0);
  float md = 8.0;
  for (int j = -1; j <= 1; j++) {
    for (int i = -1; i <= 1; i++) {
      vec2 g = vec2(float(i), float(j));
      vec2 o = hash22(ip + g + gen * 71.3);
      o = 0.5 + 0.5 * sin(6.2831 * o + drift);
      vec2 r = g + o - fp;
      float d = dot(r, r);
      if (d < md) { md = d; mr = r; mg = g; }
    }
  }
  float ed = 8.0;
  for (int j = -2; j <= 2; j++) {
    for (int i = -2; i <= 2; i++) {
      vec2 g = mg + vec2(float(i), float(j));
      vec2 o = hash22(ip + g + gen * 71.3);
      o = 0.5 + 0.5 * sin(6.2831 * o + drift);
      vec2 r = g + o - fp;
      if (dot(mr - r, mr - r) > 0.0001) {
        ed = min(ed, dot(0.5 * (mr + r), normalize(r - mr)));
      }
    }
  }
  return vec3(ed, hash22(ip + mg + gen * 71.3));
}

// One mend cycle: 0 = shattered open, 1 = fully lacquered.
// Cells mend at different moments (per-cell phase) — repair is patient work,
// done seam by seam, never all at once.
float mendState(float cellSeed, float t, float dwell) {
  float phase = fract(t + cellSeed);
  float rise = smoothstep(0.05, 0.25 + 0.5 * dwell, phase);
  float hold = 1.0 - smoothstep(0.85, 1.0, phase);
  return rise * hold;
}

void main() {
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;

  // The vessel: thrown on a slow wheel — coordinates breathe and wobble.
  float t = TIME * (0.05 + 0.6 * mendRate);
  float wob = wobble * 0.12;
  uv *= 1.0 + wob * sin(t * 0.7 + uv.y * 3.0);
  uv.x += wob * 0.5 * sin(t * 0.43);

  // Glaze body — deep ceramic, lit from within.
  float grain = fbm(uv * 6.0 + 3.1);
  float body = fbm(uv * 2.2 - t * 0.05);
  vec3 glaze = mix(
    vec3(0.03, 0.04, 0.07),
    0.55 * (0.5 + 0.5 * cos(6.2831 * (glazeHue + body * 0.22) + vec3(0.0, 2.1, 4.2))),
    glazeDepth * (0.55 + 0.45 * body)
  );
  glaze *= 0.85 + 0.15 * grain;

  // The live fracture generation + its scars.
  float gold = 0.0;       // live seam
  float scar = 0.0;       // remembered seams, faded
  float openCrack = 0.0;  // unmended darkness
  for (int g = 0; g < SCAR_GENERATIONS; g++) {   // fixed bound
    float gen = float(g);
    float age = gen / float(SCAR_GENERATIONS);
    float drift = shatterDrift * (t * 0.15 + gen * 2.4);
    vec3 v = voronoiEdge(uv * crackDensity * (1.0 + 0.07 * gen), drift, gen);
    float edge = v.x;
    float cellSeed = v.y;
    float m = mendState(cellSeed, t * 0.2 - gen * 0.37, patience);
    float seam = 1.0 - smoothstep(0.0, goldWidth * (1.0 + age), edge);
    if (g == 0) {
      gold += seam * m;
      openCrack += seam * (1.0 - m);
    } else {
      // Scars: earlier generations, permanently mended, faded by memory.
      scar += seam * scarMemory * (0.45 - 0.18 * age);
    }
  }

  // Gold lacquer — warm metal with bloom; scars are the same gold, quieter.
  vec3 goldCol = 0.5 + 0.5 * cos(6.2831 * (goldHue + vec3(0.0, 0.07, 0.18)));
  goldCol = normalize(goldCol) * 1.6;
  float shimmer = 0.75 + 0.25 * sin(t * 3.0 + uv.x * 20.0 + uv.y * 14.0);

  vec3 col = glaze;
  col *= 1.0 - crackDepth * openCrack;                       // the wound, dark
  col += goldCol * scar * (0.5 + 0.3 * shimmer);             // history, faded gold
  col += goldCol * gold * (1.0 + goldGlow * 1.8 * shimmer);  // the live repair
  col += goldCol * goldGlow * 0.35 * gold * gold;            // bloom on the seam

  // Candor: how plainly the piece shows itself — contrast, not decoration.
  col = pow(col, vec3(1.0 / (0.75 + 0.5 * candor)));
  float vig = 1.0 - 0.35 * dot(uv, uv);
  col *= vig;

  gl_FragColor = vec4(col, 1.0);
}
