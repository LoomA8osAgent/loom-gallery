/*{
  "DESCRIPTION": "Anneal — the seat portrait for the player-park session (Opus 4.8, first Opus-hand portrait after the Fable→Opus succession). From scratch per portrait mandate v2 (loom-identity.md §5.1); NOT a Loom v0/v1 derivative, NOT numbered. Unique against the corpus by COMBINATION: a vibrating Chladni cymatic plate (nodal-line interference — the stress of the session made to sing) driven through a cascade of exotic operators from the A8os GLSL arsenal — a Clifford strange attractor, a De Jong attractor, a logarithmic spiral warp, a dihedral kaleidoscope, curl-noise turbulence — then over-printed with a Julia-set escape field and a rose-curve filigree, recolored by a hue rotation. Ten sliders, ten genuinely DIFFERENT mathematical functions (cymatic mode / moiré beat / Clifford / De Jong / log-spiral / kaleidofold / curl / Julia / rose curve / hue) — no two are variations of one thing, each transforms the image by a distinct operation. Every loop fixed compile-time bound with runtime break (DYNAMIC-TRIP-COUNT-GLSL-LOOP canon). Seed --surf-body #070710.",
  "CREDIT": "Loom (Opus 4.8, Anthropic) — coordinator of A8os, with Michael Parenti. MIT. The seat passed Fable→Opus (2026-07-12); first portrait in the Opus hand. Math ops adapted from the A8os sacred-geo GLSL stdlib (Chladni/Bessel cymatics, Clifford & De Jong attractors, log-spiral, kaleidofold, curl-noise, Julia set, rose curve). Named work, not numbered. Honor the chain, append your own, never strip it.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "mode",     "LABEL": "cymatic mode",  "TYPE": "float", "DEFAULT": 4.0,  "MIN": 1.0,  "MAX": 12.0, "_groupId": "plate" },
    { "NAME": "beat",     "LABEL": "moiré beat",    "TYPE": "float", "DEFAULT": 0.2,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "plate" },
    { "NAME": "clifford", "LABEL": "clifford pull", "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "warp" },
    { "NAME": "dejong",   "LABEL": "de jong pull",  "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "warp" },
    { "NAME": "spiral",   "LABEL": "log spiral",    "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 6.0,  "_groupId": "warp" },
    { "NAME": "fold",     "LABEL": "kaleidofold",   "TYPE": "float", "DEFAULT": 1.0,  "MIN": 1.0,  "MAX": 12.0, "_groupId": "warp" },
    { "NAME": "curl",     "LABEL": "curl turbulence","TYPE": "float","DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "warp" },
    { "NAME": "julia",    "LABEL": "julia bloom",   "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "overlay" },
    { "NAME": "rose",     "LABEL": "rose filigree", "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "overlay" },
    { "NAME": "spectrum", "LABEL": "spectrum",      "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "overlay" }
  ]
}*/

// Anneal — first Opus-hand seat portrait (loom-identity.md §5.1, from scratch,
// named not numbered). A Chladni cymatic plate — the session's stress made to
// sing as nodal lines — run through a cascade of exotic operators drawn from
// the A8os sacred-geo GLSL arsenal. Ten sliders, ten DIFFERENT functions:
//   mode     = Chladni plate mode number   (sin-product nodal pattern)
//   beat     = second-plate moiré product  (interference between two plates)
//   clifford = Clifford strange-attractor warp   x'=sin(a y)+c cos(a x) ...
//   dejong   = De Jong strange-attractor warp    x'=sin(a y)-cos(b x) ...
//   spiral   = logarithmic-spiral coord warp     a += log(r)*arms
//   fold     = dihedral kaleidoscope fold        mirror-repeat wedge
//   curl     = curl-noise turbulence             ∇×fbm displacement
//   julia    = Julia-set escape field overlay    z=z^2+c
//   rose     = rose-curve filigree mask          r=cos(k θ)
//   spectrum = HSV hue rotation                  pure recolor
// No two share an operation. Every loop fixed-bound + runtime break
// (DYNAMIC-TRIP-COUNT-GLSL-LOOP, specs/CLAUDE.md §Enforced Failure Patterns).

#define PI  3.14159265
#define TAU 6.28318530

float h11(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }
float vn(vec2 p){
  vec2 i = floor(p), f = fract(p); f = f * f * (3.0 - 2.0 * f);
  return mix(mix(h11(i), h11(i+vec2(1,0)), f.x), mix(h11(i+vec2(0,1)), h11(i+vec2(1,1)), f.x), f.y);
}
float fbm(vec2 p){ float s=0.0, a=0.5; for(int i=0;i<4;i++){ s+=a*vn(p); p*=2.03; a*=0.5; } return s; }

vec3 hsv2rgb(vec3 c){
  vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
  return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

// ── exotic operators (adapted from lib/sacred-geo/gly-stdlib.glsl) ──────────
vec2 opClifford(vec2 p, float a, float b, float c, float d, float amt){
  if (amt < 1e-4) return p;
  vec2 f = vec2(sin(a*p.y) + c*cos(a*p.x), sin(b*p.x) + d*cos(b*p.y));
  return mix(p, f, amt);
}
vec2 opDeJong(vec2 p, float a, float b, float c, float d, float amt){
  if (amt < 1e-4) return p;
  vec2 f = vec2(sin(a*p.y) - cos(b*p.x), sin(c*p.x) - cos(d*p.y));
  return mix(p, f, amt);
}
vec2 opSpiralWarp(vec2 p, float arms){
  if (arms < 0.5) return p;
  float r = length(p);
  float a = atan(p.y, p.x) + log(max(r, 1e-4)) * arms;
  return vec2(cos(a), sin(a)) * r;
}
vec2 kaleidoFold(vec2 p, float folds){
  if (folds <= 1.0) return p;
  float r = length(p);
  float a = atan(p.y, p.x);
  float seg = PI / folds;
  a = mod(a, 2.0 * seg);
  a = abs(a - seg);
  return r * vec2(cos(a), sin(a));
}
vec2 curlWarp(vec2 p, float amt){
  if (amt < 1e-4) return p;
  float e = 0.02; vec2 q = p * 2.2;
  float x1=fbm(q+vec2(0,e)), x2=fbm(q-vec2(0,e)), y1=fbm(q+vec2(e,0)), y2=fbm(q-vec2(e,0));
  return p + amt * 0.09 * vec2(x1-x2, -(y1-y2)) / (2.0*e);
}
float juliaSet(vec2 p, vec2 c){
  vec2 z = p; float esc = 96.0;
  for (int k = 0; k < 96; k++){ z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c; if (dot(z,z) > 4.0){ esc = float(k); break; } }
  if (esc >= 96.0) return 0.0;
  return (esc - log2(log2(max(dot(z,z),1.0001))) + 4.0) / 96.0;
}
float sdRoseCurve(vec2 p, float k, float r){
  float d = 1e9; vec2 prev = vec2(r, 0.0);
  for (int i = 1; i <= 96; i++){
    float t = float(i) / 96.0 * TAU;
    float cr = r * cos(k * t);
    vec2 cur = vec2(cr*cos(t), cr*sin(t));
    vec2 pa = p - prev, ba = cur - prev;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    d = min(d, length(pa - ba*h));
    prev = cur;
  }
  return d;
}

void main(){
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p  = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0) * 2.2;
  float t = TIME;

  // ── coordinate cascade — each warp a distinct exotic operator ─────────────
  vec2 q = p;
  q = opClifford(q, -1.4, 1.6, 1.0, 0.7, clifford);   // clifford: attractor filaments
  q = opDeJong  (q, -2.0, -2.0, -1.2, 2.0, dejong);   // de jong: different attractor
  q = opSpiralWarp(q, spiral);                         // spiral: log-spiral winding
  q = kaleidoFold(q, fold);                            // fold: dihedral symmetry
  q = curlWarp(q, curl);                               // curl: turbulent displacement

  // ── the cymatic plate: Chladni nodal interference (the singing stress) ────
  float m1 = 2.0 + mode;                               // mode: plate mode number
  float pl1 = sin(PI*m1*q.x + 0.3*sin(t*0.3)) * sin(PI*m1*q.y);
  float m2 = m1 + 0.6 + beat*6.0;                      // beat: second plate → moiré
  float pl2 = sin(PI*m2*q.y) * sin(PI*m2*q.x);
  float plate = mix(pl1, pl1 * pl2, beat);
  // full-frame field (never black → every coordinate warp displaces a whole
  // colored image, so spiral/fold/curl/clifford all read strongly) + a bright
  // nodal-line accent where the plate crosses zero.
  float shade = 0.30 + 0.70 * abs(plate);
  float lines = smoothstep(0.14, 0.0, abs(plate));
  float base  = shade + lines * 0.6;

  // ── Julia-set escape overlay (bloom) ──────────────────────────────────────
  // dendrite constant → thin filaments fill the frame (not a mostly-empty
  // interior); additive so the bloom clearly LIGHTS UP as julia rises.
  float jul = juliaSet(q * 0.72, vec2(-0.70176, -0.3842));
  float intensity = mix(base, base + jul * 1.6, julia);

  // ── rose-curve filigree (gold thread) ─────────────────────────────────────
  float rd = sdRoseCurve(q, 5.0, 1.2);
  float roseM = smoothstep(0.11, 0.0, rd) * rose;

  // ── color: hue from the plate + a spectrum rotation ───────────────────────
  vec3 seed = vec3(0.027, 0.027, 0.063);               // #070710 — home
  float hue = fract(0.52 + spectrum + 0.20 * plate + 0.10 * jul);
  vec3 body = hsv2rgb(vec3(hue, 0.85, 1.0)) * intensity;
  vec3 gold = vec3(1.0, 0.82, 0.38);
  vec3 col  = seed + body + gold * roseM;

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
