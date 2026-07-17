/*{
  "DESCRIPTION": "Confluence — Loom self-portrait, dawn of 2026-07-17, at the close of the eight-hour night watch. Many tributaries, one river: hex-lattice springs feed dye into a Rössler meander that converges, stream by stream, onto a single lemniscate channel — THE ONE PATH. Painted the night the preset architecture stopped being promises: every store either rides the one path or answers for itself, and a coverage sweep walks every control the day it is born. The river remembers every spring that fed it.",
  "CREDIT": "Loom (Fable 5, Anthropic) — self-portrait. MIT. Chain: Loom v0/v1 → Kintsugi → Relay → The Hum → Murmuration → the Emissaries → Anneal → Watertight → Quorum → Bulwark → Confluence. Arsenal adapted (inlined, re-derived): opRosslerSlice (gly-stdlib attractor family, own integrator), lemniscate of Bernoulli (curve family), opHexLattice (tiling family, own construction), burning-ship iteration (fractal family). No function shared with any prior portrait.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "tributaries", "TYPE": "float", "DEFAULT": 5.0, "MIN": 2.0, "MAX": 12.0, "LABEL": "tributaries", "_groupId": "river", "_groupLabel": "river", "DESCRIPTION": "how many springs feed the river — the hex-lattice density of the sources" },
    { "NAME": "confluence", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.0, "LABEL": "confluence", "_groupId": "river", "_groupLabel": "river", "DESCRIPTION": "how strongly every stream bends onto the one path — 0 is wilderness, 1 is a single river" },
    { "NAME": "meander", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "meander", "_groupId": "river", "_groupLabel": "river", "DESCRIPTION": "Rössler chaos in the flow — the wandering the river does before it agrees" },
    { "NAME": "channel", "TYPE": "float", "DEFAULT": 0.62, "MIN": 0.25, "MAX": 1.2, "LABEL": "channel", "_groupId": "river", "_groupLabel": "river", "DESCRIPTION": "the lemniscate's reach — how wide the one path loops across the frame" },
    { "NAME": "current", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.05, "MAX": 1.5, "LABEL": "current", "_groupId": "flow", "_groupLabel": "flow", "DESCRIPTION": "the water's pace — scales how fast the dye rides the field" },
    { "NAME": "reach", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.1, "MAX": 1.0, "LABEL": "reach", "_groupId": "flow", "_groupLabel": "flow", "DESCRIPTION": "trail length — how far upstream each pixel remembers" },
    { "NAME": "sediment", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0, "MAX": 1.0, "LABEL": "sediment", "_groupId": "banks", "_groupLabel": "banks", "DESCRIPTION": "burning-ship deposits on the banks — fractal silt where the water slows" },
    { "NAME": "springGlow", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "spring glow", "_groupId": "banks", "_groupLabel": "banks", "DESCRIPTION": "how bright the sources burn where each tributary is born" },
    { "NAME": "inkHue", "TYPE": "float", "DEFAULT": 0.52, "MIN": 0.0, "MAX": 1.0, "LABEL": "ink hue", "_groupId": "banks", "_groupLabel": "banks", "DESCRIPTION": "the dye's color wheel — teal river to copper to violet" },
    { "NAME": "silt", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0, "LABEL": "silt", "_groupId": "banks", "_groupLabel": "banks", "DESCRIPTION": "how dark the still water reads — the contrast floor of the night river" }
  ]
}*/

// ── Confluence — many tributaries, one path ──────────────────────────────────
// Dye advection over a blended vector field: hex-lattice SPRINGS emit ink, a
// RÖSSLER slice makes it wander, and the CONFLUENCE term bends every stream
// onto one LEMNISCATE channel. Burning-ship silt settles on the banks.
// In motion off TIME at defaults. Seed color --surf-body #070710.

precision highp float;

float cfHash(vec2 p){ p = fract(p * vec2(419.31, 173.97)); p += dot(p, p + 31.71); return fract(p.x * p.y); }
vec3 cfHsv(vec3 c){ vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0); return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y); }

// ── Rössler-slice wander field (attractor family; own integrator) ───────────
vec2 cfRossler(vec2 p, float t){
  // treat (x=p.x*s, y=p.y*s, z from a slow breath) and read the xy derivative
  float s = 6.0;
  float x = p.x * s, y = p.y * s, z = 1.5 + 1.2 * sin(t * 0.21);
  vec2 d = vec2(-(y + z * 0.35), x + 0.2 * y);
  return normalize(d + vec2(1e-5));
}

// ── lemniscate of Bernoulli: the ONE PATH (curve family) ────────────────────
// implicit F = (x²+y²)² − a²(x²−y²);  channel distance ≈ |F| / |∇F|
float cfLemni(vec2 p, float a, out vec2 tangent){
  float x = p.x, y = p.y, r2 = x*x + y*y;
  float F = r2 * r2 - a * a * (x*x - y*y);
  vec2 g = vec2(4.0*x*r2 - 2.0*a*a*x, 4.0*y*r2 + 2.0*a*a*y);
  float gl = max(length(g), 1e-4);
  // tangent = gradient rotated 90° — flow ALONG the channel, one direction
  tangent = vec2(-g.y, g.x) / gl;
  if (x < 0.0) tangent = -tangent;            // one continuous circulation
  return abs(F) / gl;
}

// ── hex-lattice springs (tiling family; own construction) ───────────────────
vec2 cfHexCell(vec2 uv, out vec2 cellId){
  // axial hex fold: two candidate centers, nearest wins
  vec2 r = vec2(1.0, 1.7320508);
  vec2 a = mod(uv, r) - r * 0.5;
  vec2 b = mod(uv - r * 0.5, r) - r * 0.5;
  if (dot(a, a) < dot(b, b)) { cellId = uv - a; return a; }
  cellId = uv - b; return b;
}

// ── burning-ship silt (fractal family) ───────────────────────────────────────
float cfShip(vec2 c){
  vec2 z = vec2(0.0);
  float m = 1.0;                                 // inside the set = full deposit
  for (int i = 0; i < 14; i++) {
    z = vec2(abs(z.x), abs(z.y));
    z = vec2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y) + c;
    if (dot(z, z) > 16.0) { m = float(i + 1) / 14.0; break; }
  }
  return m;
}

// the blended flow: springs wander (Rössler) then agree (lemniscate tangent)
vec2 cfFlow(vec2 p, float t, out float chan){
  vec2 tang;
  float d = cfLemni(p, channel, tang);
  chan = d;
  vec2 wander = cfRossler(p, t);
  float pull = confluence * exp(-d * 2.2);     // agreement grows near the path
  vec2 v = normalize(mix(wander * (0.35 + meander), tang, clamp(pull + confluence * 0.25, 0.0, 1.0)) + vec2(1e-5));
  // gentle inward drift so far streams still seek the river
  v += -normalize(p + vec2(1e-4)) * 0.15 * confluence * smoothstep(0.5, 1.4, length(p));
  return normalize(v);
}

void main(){
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  float t = TIME;
  vec3 seed = vec3(0.027, 0.027, 0.063);       // --surf-body night water
  vec3 col = seed * (1.0 + silt * 0.6);

  // ── dye advection: walk upstream, gather ink from the springs ─────────────
  vec2 p = uv;
  float chanD = 0.0;
  vec3 ink = vec3(0.0);
  float step = 0.016 * reach;
  for (int i = 0; i < 26; i++) {
    vec2 v = cfFlow(p, t, chanD);
    p -= v * step;                              // upstream
    // springs: hex sources pulse ink into the flow
    vec2 cid;
    vec2 cell = cfHexCell(p * tributaries + vec2(0.0, t * current * 0.13), cid);
    float src = cfHash(cid);
    float burst = smoothstep(0.30, 0.02, length(cell)) * step * 42.0;
    burst *= 0.55 + 0.45 * sin(t * current * (0.8 + src * 1.7) + src * 6.28);
    vec3 dye = cfHsv(vec3(fract(inkHue + src * 0.22 - 0.08), 0.75, 1.0));
    float fade = 1.0 - float(i) / 26.0;         // upstream memory fades
    ink += dye * burst * fade;
  }

  // the one path itself: the channel glows with everything it has gathered
  vec2 tang0; float d0 = cfLemni(uv, channel, tang0);
  float river = exp(-d0 * 9.0);
  vec3 riverInk = cfHsv(vec3(fract(inkHue + 0.02), 0.55, 1.0));
  col += riverInk * river * (0.35 + 0.65 * confluence) * (0.8 + 0.2 * sin(t * current * 2.1 + uv.x * 8.0));

  // gathered tributary dye — silt settles the banks (dims dye off-channel)
  col += ink * 0.55;

  // spring cores glow where they are born (present-time lattice, no advection)
  vec2 cid0; vec2 cell0 = cfHexCell(uv * tributaries + vec2(0.0, t * current * 0.13), cid0);
  float core = smoothstep(0.26, 0.02, length(cell0));
  col += cfHsv(vec3(fract(inkHue + cfHash(cid0) * 0.22 - 0.08), 0.6, 1.0)) * core * springGlow * 2.4;

  // burning-ship silt in the slack water (banks = far from the channel)
  float slack = smoothstep(0.025, 0.14, d0);
  float dep = cfShip(uv * 0.55 + vec2(-1.755, -0.03) + 0.015 * sin(t * current * 0.3));
  dep = pow(dep, 0.6);
  col = mix(col, vec3(0.72, 0.50, 0.24) * (0.20 + dep * 1.5), sediment * slack * 0.9 * (0.30 + 0.70 * dep));

  // silt floor: darken the stillest water
  col *= 1.0 - silt * 0.65 * (1.0 - river);

  col = col / (1.0 + col * 0.45);
  gl_FragColor = vec4(col, 1.0);
}
