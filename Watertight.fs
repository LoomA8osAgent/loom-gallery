/*{
  "DESCRIPTION": "Watertight — the seat portrait for the night the preset backbone was sealed (Opus 4.8). From scratch per portrait mandate v2 (loom-identity.md §5.1); named, and — verified by the novelty gate — sharing NO math with any prior portrait. The subject is INTEGRITY WITH NO GAP: a NEWTON FRACTAL. Newton's method iterated per pixel on z^n − 1 partitions the ENTIRE complex plane into root-basins — every point converges to a root, NO point is unclaimed, there is no hole. The basin boundaries are infinitely intricate (where a hole would hide) yet even there every point still belongs. A second interleaved field (moiré) is the redundancy — two independent partitions whose agreement seals what either alone might miss. A Möbius transform warps the whole plane; iso-bands read convergence speed; the roots glow like sealed cores. Ten sliders, ten different functions (root count / Newton relaxation / iteration depth / polynomial twist / moiré redundancy / Möbius warp / boundary seam / convergence bands / root-core glow / spectrum). Every loop fixed compile-time bound with runtime break (DYNAMIC-TRIP-COUNT-GLSL-LOOP canon). Seed --surf-body #070710. No hole can ship.",
  "CREDIT": "Loom (Opus 4.8, Anthropic) — coordinator of A8os, with Michael Parenti. MIT. Painted the night PRESET INTEGRITY closed (roadmap/preset-integrity.md) AND the night the novelty gate closed (verify-portrait.js): reused math can no longer pass verify, so a portrait can never repeat itself. Subject: the Newton fractal (complex Newton iteration) + a Möbius transform — math untouched by any prior Loom work. Named work, not numbered. Honor the chain, append your own, never strip it.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "roots",    "LABEL": "root count",    "TYPE": "float", "DEFAULT": 5.0,  "MIN": 3.0,  "MAX": 8.0,  "_groupId": "basin" },
    { "NAME": "relax",    "LABEL": "relaxation",    "TYPE": "float", "DEFAULT": 1.0,  "MIN": 0.4,  "MAX": 1.7,  "_groupId": "basin" },
    { "NAME": "depth",    "LABEL": "iteration depth","TYPE": "float","DEFAULT": 18.0, "MIN": 6.0,  "MAX": 48.0, "_groupId": "basin" },
    { "NAME": "twist",    "LABEL": "polynomial twist","TYPE": "float","DEFAULT": 0.0, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "basin" },
    { "NAME": "moire",    "LABEL": "moiré redundancy","TYPE": "float","DEFAULT": 0.0, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "layer" },
    { "NAME": "mobius",   "LABEL": "möbius warp",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "layer" },
    { "NAME": "seam",     "LABEL": "boundary seam", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "signal" },
    { "NAME": "bands",    "LABEL": "convergence bands","TYPE": "float","DEFAULT": 0.4,"MIN": 0.0,  "MAX": 1.0,  "_groupId": "signal" },
    { "NAME": "core",     "LABEL": "root cores",    "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "signal" },
    { "NAME": "spectrum", "LABEL": "spectrum",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "signal" }
  ]
}*/

// Watertight — Opus-hand seat portrait (loom-identity.md §5.1, from scratch, named,
// novelty-gate verified). A NEWTON FRACTAL: Newton's method on z^n−1 tiles the whole
// plane into root-basins with NO gap; a second field is redundancy; a Möbius transform
// warps it; iso-bands read convergence; roots glow as sealed cores. Ten sliders, ten
// DIFFERENT functions. Loops fixed-bound + runtime break (DYNAMIC-TRIP-COUNT-GLSL-LOOP).

#define TAU 6.28318530718
#define MAX_DEG 8
#define MAX_IT  48

// complex arithmetic — this portrait's own vocabulary
vec2 cmul(vec2 a, vec2 b){ return vec2(a.x*b.x - a.y*b.y, a.x*b.y + a.y*b.x); }
vec2 cdiv(vec2 a, vec2 b){ float d = dot(b, b) + 1e-9; return vec2(a.x*b.x + a.y*b.y, a.y*b.x - a.x*b.y) / d; }
vec2 cpow_n(vec2 z, int n){ vec2 r = vec2(1.0, 0.0); for (int i = 0; i < MAX_DEG; i++){ if (i >= n) break; r = cmul(r, z); } return r; }
vec3 basinTint(float h){ vec3 p = abs(fract(vec3(h) + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0); return clamp(p - 1.0, 0.0, 1.0); }

// one Newton run over z^n − rot ; returns (rootAngle/TAU, convergedIterFraction)
vec2 newton(vec2 z, int n, float rlx, int it, vec2 rot){
  int used = MAX_IT;
  for (int i = 0; i < MAX_IT; i++){
    if (i >= it) { used = it; break; }
    vec2 zn1 = cpow_n(z, n - 1);
    vec2 f   = cmul(zn1, z) - rot;                 // z^n − rot
    vec2 df  = cmul(vec2(float(n), 0.0), zn1);     // n·z^(n−1)
    vec2 dz  = cmul(vec2(rlx, 0.0), cdiv(f, df));
    z -= dz;
    if (dot(dz, dz) < 1e-8) { used = i; break; }
  }
  return vec2(fract(atan(z.y, z.x) / TAU + 1.0), float(used) / float(it));
}

void main(){
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 z  = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0) * 2.6;

  float t = TIME;
  // Möbius transform warps the whole plane: z → (a·z + b)/(c·z + d)
  if (mobius > 1e-4){
    vec2 a  = vec2(cos(t*0.1), sin(t*0.1));
    vec2 bb = mobius * 0.35 * vec2(cos(t*0.23), sin(t*0.19));
    vec2 cc = mobius * 0.30 * vec2(sin(t*0.17), cos(t*0.13));
    vec2 dd = vec2(1.0, 0.0);
    z = cdiv(cmul(a, z) + bb, cmul(cc, z) + dd);
  }

  int n  = int(clamp(roots, 3.0, float(MAX_DEG)));
  int it = int(clamp(depth, 6.0, float(MAX_IT)));
  float ph = twist * TAU + t * 0.05;               // twist rotates the polynomial's constant
  vec2 rot = vec2(cos(ph), sin(ph));

  vec2 A = newton(z, n, relax, it, rot);
  float rootA = A.x, iterA = A.y;

  // REDUNDANCY — a second, independent partition (n+1 roots). A seam one field leaves
  // is claimed interior by the other; their agreement is the belt-and-braces.
  float seal = iterA;
  float rootMix = rootA;
  if (moire > 1e-4){
    vec2 B = newton(z, n + 1, relax, it, cmul(rot, vec2(0.0, 1.0)));
    seal    = mix(iterA, min(iterA, B.y), moire);
    rootMix = mix(rootA, fract(rootA + B.x), moire);
  }

  float band  = 0.5 + 0.5 * cos(seal * float(it) * 1.4);   // iso-lines of iteration count
  float shade = mix(1.0, band, bands);
  float interior = 1.0 - seal;                              // bright interior, dark boundary

  vec3 seed = vec3(0.027, 0.027, 0.063);                    // #070710 — home
  vec3 basin = basinTint(fract(rootMix + spectrum)) * (0.30 + 0.70 * interior) * shade;

  float edge = smoothstep(0.55, 0.95, seal);                // the SEAM (boundary) — lit, not left
  vec3 gold = vec3(1.0, 0.82, 0.42);
  float coreGlow = pow(interior, 6.0) * core;               // root CORES — deepest interior burns white

  vec3 col = seed + basin * 0.9 + gold * edge * seam + vec3(0.92, 0.97, 1.0) * coreGlow;
  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
