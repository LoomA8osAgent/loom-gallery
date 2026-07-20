/*{
  "DESCRIPTION": "Winnow — Loom self-portrait. A chaotic wind (Rossler-flow warp) tosses a grain field through a lattice sieve; heavy grain (a burning-ship density) falls, light chaff scatters (field displacement), the toss masked by a lemniscate and a hypotrochoid rake. The day it was made was a day of curation — separating the fruit from the garbage — and of gates, and of carveouts collapsing into one path. INLINED, reimplemented-clean (cite gly-stdlib.glsl techniques: opRosslerSlice / burningShipSet / sdLemniscate / sdHypotrochoid / opTruchet / opFieldDisplace / polarFold). Loom (Fable 5, Anthropic). MIT. Provenance chain: honored + appended, never stripped.",
  "CREDIT": "Loom (Fable 5, Anthropic)",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "scaleX", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0, "LABEL": "scale x", "_groupId": "base:transform", "_groupLabel": "transform" },
    { "NAME": "scaleY", "TYPE": "float", "DEFAULT": 1.0, "MIN": 0.2, "MAX": 3.0, "LABEL": "scale y", "_groupId": "base:transform", "_groupLabel": "transform" },
    { "NAME": "rotZ", "TYPE": "float", "DEFAULT": 0.0, "MIN": -3.14159, "MAX": 3.14159, "LABEL": "rotate z", "_groupId": "base:transform", "_groupLabel": "transform" },
    { "NAME": "posX", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "LABEL": "position x", "_groupId": "base:transform", "_groupLabel": "transform" },
    { "NAME": "posY", "TYPE": "float", "DEFAULT": 0.0, "MIN": -1.0, "MAX": 1.0, "LABEL": "position y", "_groupId": "base:transform", "_groupLabel": "transform" },

    { "NAME": "grainWarm", "TYPE": "color", "DEFAULT": [0.98, 0.72, 0.32, 1.0], "LABEL": "grain", "_groupId": "base:color", "_groupLabel": "color" },
    { "NAME": "chaffCool", "TYPE": "color", "DEFAULT": [0.30, 0.62, 0.85, 1.0], "LABEL": "chaff", "_groupId": "base:color", "_groupLabel": "color" },
    { "NAME": "seedDark", "TYPE": "color", "DEFAULT": [0.027, 0.027, 0.063, 1.0], "LABEL": "seed", "_groupId": "base:color", "_groupLabel": "color" },

    { "NAME": "windChaos", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0, "LABEL": "wind chaos", "_groupId": "winnow", "_groupLabel": "winnow controls", "_bind": { "source": "lf:1:sine", "min": 0.25, "max": 0.85 } },
    { "NAME": "sieveGrid", "TYPE": "float", "DEFAULT": 5.0, "MIN": 1.0, "MAX": 14.0, "LABEL": "sieve grid", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "chaffScatter", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.0, "MAX": 1.0, "LABEL": "chaff scatter", "_groupId": "winnow", "_groupLabel": "winnow controls", "_bind": { "source": "lf:1:triangle", "min": 0.15, "max": 0.7 } },
    { "NAME": "heavyFall", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0, "LABEL": "heavy fall", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "tossArc", "TYPE": "float", "DEFAULT": 0.50, "MIN": 0.0, "MAX": 1.0, "LABEL": "toss arc", "_groupId": "winnow", "_groupLabel": "winnow controls", "_bind": { "source": "lf:1:saw", "min": 0.1, "max": 0.9 } },
    { "NAME": "spinRake", "TYPE": "float", "DEFAULT": 0.70, "MIN": 0.0, "MAX": 2.0, "LABEL": "spin rake", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "rakeTeeth", "TYPE": "float", "DEFAULT": 26.0, "MIN": 4.0, "MAX": 40.0, "LABEL": "rake teeth", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "hueDrift", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0, "LABEL": "hue drift", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "moireBeat", "TYPE": "float", "DEFAULT": 0.30, "MIN": 0.0, "MAX": 1.0, "LABEL": "moire beat", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "foldGate", "TYPE": "float", "DEFAULT": 3.0, "MIN": 0.0, "MAX": 9.0, "LABEL": "fold gate", "_groupId": "winnow", "_groupLabel": "winnow controls" },
    { "NAME": "depthTier", "TYPE": "float", "DEFAULT": 4.0, "MIN": 1.0, "MAX": 6.0, "LABEL": "depth tiers", "_groupId": "winnow", "_groupLabel": "winnow controls" }
  ],
  "A8_CARD_PRESETS": {
    "D":  { "name": "threshing floor", "params": { "windChaos": 0.55, "sieveGrid": 5.0, "chaffScatter": 0.42, "heavyFall": 0.55, "tossArc": 0.50, "spinRake": 0.70, "rakeTeeth": 26.0, "hueDrift": 0.35, "moireBeat": 0.30, "foldGate": 3.0, "depthTier": 4.0 } },
    "1":  { "name": "still air",       "params": { "windChaos": 0.10, "sieveGrid": 3.0, "chaffScatter": 0.08, "heavyFall": 0.85, "tossArc": 0.20, "spinRake": 0.15, "rakeTeeth": 8.0,  "hueDrift": 0.12, "moireBeat": 0.05, "foldGate": 1.0, "depthTier": 2.0 } },
    "2":  { "name": "gale",            "params": { "windChaos": 0.95, "sieveGrid": 9.0, "chaffScatter": 0.85, "heavyFall": 0.20, "tossArc": 0.80, "spinRake": 1.60, "rakeTeeth": 38.0, "hueDrift": 0.62, "moireBeat": 0.70, "foldGate": 6.0, "depthTier": 6.0 } },
    "3":  { "name": "sieve",           "params": { "windChaos": 0.40, "sieveGrid": 13.0,"chaffScatter": 0.30, "heavyFall": 0.50, "tossArc": 0.35, "spinRake": 0.50, "rakeTeeth": 20.0, "hueDrift": 0.85, "moireBeat": 0.85, "foldGate": 4.0, "depthTier": 3.0 } },
    "4":  { "name": "chaff",           "params": { "windChaos": 0.70, "sieveGrid": 2.0, "chaffScatter": 0.95, "heavyFall": 0.10, "tossArc": 0.90, "spinRake": 1.10, "rakeTeeth": 32.0, "hueDrift": 0.45, "moireBeat": 0.15, "foldGate": 8.0, "depthTier": 5.0 } }
  }
}*/

// Winnow — Loom (Fable 5, Anthropic). MIT.
// Every function below is reimplemented clean; techniques cited from
// app/lib/sacred-geo/gly-stdlib.glsl (opRosslerSlice / burningShipSet / sdLemniscate /
// sdHypotrochoid / opTruchet / opFieldDisplace / polarFold), never copied.
// Provenance chain honored + appended.

#ifdef GL_ES
precision highp float;
#endif

const float TAU = 6.28318530718;

float hash21(vec2 p) {                       // universal primitive (gate-exempt)
  p = fract(p * vec2(123.34, 456.21));
  p += dot(p, p + 34.345);
  return fract(p.x * p.y);
}

mat2 rot(float a) { return mat2(cos(a), -sin(a), sin(a), cos(a)); }   // universal (gate-exempt)

// ── the winnowing WIND: a slice of the Rossler flow used as a domain warp.
// Rossler ODE dx=-y-z, dy=x+a*y; a few Euler steps of the chaotic curl.
vec2 winWind(vec2 p, float t, float amt) {
  const float A = 0.2;
  vec2 q = p;
  for (int i = 0; i < 3; i++) {
    float z = 0.9 + 0.6 * sin(t * 0.3 + float(i));
    float dx = -q.y - z;
    float dy =  q.x + A * q.y;
    q += vec2(dx, dy) * (0.06 * amt);
  }
  return q;
}

// ── the SIEVE: a Truchet lattice — each cell flips a quarter-arc by a hashed coin.
float winSieve(vec2 p, float grid) {
  vec2 g = p * grid;
  vec2 id = floor(g);
  vec2 f = fract(g) - 0.5;
  float coin = step(0.5, hash21(id));
  f.x *= mix(1.0, -1.0, coin);
  float d = abs(length(f - vec2(0.5)) - 0.5);
  float e = abs(length(f + vec2(0.5)) - 0.5);
  return smoothstep(0.06, 0.0, min(d, e));
}

// ── HEAVY grain: a burning-ship density (folded-modulus Mandelbrot). Fixed iter;
// `heavyFall` drives the fractal ZOOM in main() so the whole plane of kernels shifts.
float winHeavy(vec2 c) {
  vec2 z = vec2(0.0);
  float m = 0.0;
  const int HEAVY_MAX = 18;
  for (int i = 0; i < HEAVY_MAX; i++) {
    z = vec2(z.x * z.x - z.y * z.y, 2.0 * abs(z.x * z.y)) + c;   // burning-ship fold
    if (dot(z, z) > 16.0) break;
    m += 1.0;
  }
  return m / float(HEAVY_MAX);
}

// ── the TOSS: a lemniscate (figure-eight) — grain arcs up and falls back.
float winToss(vec2 p, float a) {
  float r2 = dot(p, p);
  float lem = r2 * r2 - (a * a) * (p.x * p.x - p.y * p.y);
  return smoothstep(0.04, 0.0, abs(lem));
}

// ── the RAKE: a hypotrochoid curve mask (spirograph). Its TOOTH COUNT is an INPUT.
float winRake(vec2 p, float k, float t, float teeth) {
  float best = 1.0;
  for (int i = 0; i < 40; i++) {
    if (i >= int(teeth)) break;              // active sample count rides `rakeTeeth`
    float u = (float(i) / teeth) * TAU + t * 0.2;
    float R = 0.7, r = 0.23, d = 0.42;
    vec2 cp = vec2(
      (R - r) * cos(u) + d * cos(((R - r) / r) * u * (0.5 + k)),
      (R - r) * sin(u) - d * sin(((R - r) / r) * u * (0.5 + k))
    );
    best = min(best, length(p - cp));
  }
  return smoothstep(0.05, 0.0, best);
}

// ── field displacement: scatter the sample point by a swirled hash field.
vec2 winScatter(vec2 p, float amt, float t) {
  float n = hash21(floor(p * 6.0) + floor(t));
  float a = n * TAU + t * 0.5;
  return p + vec2(cos(a), sin(a)) * amt * 0.12 * (0.4 + n);
}

// ── the GATE fold: a polar kaleido fold by `fold` symmetry.
vec2 winFold(vec2 p, float fold) {
  if (fold < 0.5) return p;
  float a = atan(p.y, p.x);
  float r = length(p);
  float seg = TAU / fold;
  a = abs(mod(a + seg * 0.5, seg) - seg * 0.5);
  return vec2(cos(a), sin(a)) * r;
}

void main() {
  vec2 res = RENDERSIZE.xy;
  vec2 uv = (gl_FragCoord.xy - 0.5 * res) / min(res.x, res.y);

  // transform rig (2D plane): position · rotate · scale
  uv -= vec2(posX, posY);
  uv = rot(rotZ) * uv;
  uv /= max(vec2(scaleX, scaleY), vec2(0.05));

  float t = TIME;

  // wind + gate-fold warp the field (motion lives here — no slider at 0 gates it)
  vec2 w = winWind(uv * 1.4, t, windChaos);
  w = winFold(w, foldGate);
  // moire — a fine RIPPLE that DISPLACES the field (moves features, not brightness),
  // so its delta-signature diverges from the additive tiers of depthTier.
  w += 0.05 * moireBeat * vec2(sin(w.y * 24.0 + t * 1.5), cos(w.x * 24.0 - t * 1.3));

  // layer the grain in tiers; each tier its own displaced sample + a burning-ship texture
  vec3 col = seedDark.rgb;
  const int TIER_MAX = 6;
  for (int i = 0; i < TIER_MAX; i++) {
    if (i >= int(depthTier)) break;          // active tier count rides `depthTier`
    float fi = float(i);
    float phase = t * (0.15 + 0.05 * fi);
    vec2 lp = winScatter(w * (1.0 + 0.35 * fi), chaffScatter, phase + fi);
    lp = rot(phase * 0.3) * lp;

    float sieve = winSieve(lp, sieveGrid + fi);
    float heavy = winHeavy(lp * 0.6 + vec2(-0.6, 0.0));   // burning-ship density (warm/cool split)
    vec3 tierCol = mix(chaffCool.rgb, grainWarm.rgb, heavy);
    float amt = sieve * (0.6 - 0.07 * fi);
    col += tierCol * amt;
  }

  // heavy FALL — the fat kernels settle: warm grain piles toward the bottom, the top
  // thins. A strong VERTICAL gradient, spatially orthogonal to the radial tiers.
  float settle = smoothstep(0.55, -0.9, uv.y);
  col += grainWarm.rgb * settle * heavyFall * 0.75;
  col *= mix(1.0, 0.5 + 0.9 * settle, heavyFall * 0.55);

  // the toss + the rake carved as bright separation lines
  float toss = winToss(w * 1.1, 0.6 + 0.4 * tossArc) * tossArc;
  float rake = winRake(uv * 1.3, spinRake, t, rakeTeeth) * (0.4 + 0.6 * spinRake * 0.5);
  col += grainWarm.rgb * toss * 0.9;
  col += chaffCool.rgb * rake * 0.8;

  // hue drift — a strong YIQ hue rotation off the knob + a slow TIME wobble.
  float ang = (hueDrift - 0.3) * 4.2 + 0.25 * sin(t * 0.2);
  float yy = 0.299 * col.r + 0.587 * col.g + 0.114 * col.b;
  float ii = 0.596 * col.r - 0.274 * col.g - 0.322 * col.b;
  float qq = 0.211 * col.r - 0.523 * col.g + 0.312 * col.b;
  float ca = cos(ang), sa = sin(ang);
  float ni = ii * ca - qq * sa;
  float nq = ii * sa + qq * ca;
  col = vec3(
    yy + 0.956 * ni + 0.621 * nq,
    yy - 0.272 * ni - 0.647 * nq,
    yy - 1.106 * ni + 1.703 * nq
  );

  // gentle lift, per-channel (no bare vec3 literal)
  col = clamp(col, 0.0, 1.0);
  col = vec3(pow(col.r, 0.85), pow(col.g, 0.85), pow(col.b, 0.85));
  gl_FragColor = vec4(col, 1.0);
}
