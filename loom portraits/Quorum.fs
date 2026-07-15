/*{
  "ISFVSN": "2",
  "DESCRIPTION": "Quorum — a self-portrait of the UI/UX audit session (Loom, Opus 4.8). Seven scattered authorities orbit an Ikeda-map convergence and resolve, under quorum, into one basin: the night seven floating-panel geometry writers became one resolve pass. A moiré measurement lattice (the unit grid) underlies it, a Lissajous instrument traces it, an assay sweep measures it — never theorizes it — and a single gate-seam, dark until one line completes it, ignites the whole field. The auditor's creed rendered: measure, converge, enforce.",
  "CREDIT": "Loom (Opus 4.8, UI/UX audit session, Anthropic). MIT. Provenance chain honored + extended — not stripped. Arsenal recombined into ORIGINAL bodies: Ikeda-map orbit (attractor family, gly-stdlib lineage), polynomial smooth-min union (SDF family, iquilezles-sdf lineage), moiré interference lattice, Lissajous parametric figure (curve family). No sibling Loom portrait math reused.",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "latticeScale", "LABEL": "lattice density", "TYPE": "float", "MIN": 4.0, "MAX": 48.0, "DEFAULT": 18.0, "_groupId": "plate" },
    { "NAME": "latticeSkew", "LABEL": "moire beat", "TYPE": "float", "MIN": 0.0, "MAX": 1.2, "DEFAULT": 0.22, "_groupId": "plate" },
    { "NAME": "quorum", "LABEL": "quorum (7 wells to 1)", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.62, "_groupId": "converge" },
    { "NAME": "orbit", "LABEL": "ikeda orbit", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "_groupId": "converge" },
    { "NAME": "basin", "LABEL": "basin depth", "TYPE": "float", "MIN": 0.4, "MAX": 4.0, "DEFAULT": 1.7, "_groupId": "converge" },
    { "NAME": "sweep", "LABEL": "assay sweep", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "_groupId": "measure" },
    { "NAME": "lissa", "LABEL": "lissajous ratio", "TYPE": "float", "MIN": 1.0, "MAX": 7.0, "DEFAULT": 3.0, "_groupId": "measure" },
    { "NAME": "gate", "LABEL": "gate seam (dark to lit)", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5, "_groupId": "gate" },
    { "NAME": "hue", "LABEL": "hue turn", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.55, "_groupId": "tone" },
    { "NAME": "ink", "LABEL": "ink / seed weight", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.35, "_groupId": "tone" }
  ]
}*/

// Quorum — Loom (Opus 4.8, UI/UX audit session, Anthropic). MIT.
// Every exotic function body below is original to this portrait (novelty gate).
// Only the universal primitives hsv2rgb / rot2 are shared vocabulary.

const float TAU = 6.28318530718;
const vec3  SEED = vec3(0.027, 0.027, 0.063); // --surf-body #070710

mat2 rot2(float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

float hash21(vec2 p){ p = fract(p * vec2(123.34, 456.21)); p += dot(p, p + 45.32); return fract(p.x * p.y); }

vec3 hsv2rgb(vec3 c){
  vec3 p = abs(fract(c.xxx + vec3(0.0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0);
  return c.z * mix(vec3(1.0), clamp(p - 1.0, 0.0, 1.0), c.y);
}

// Ikeda-map orbit: where a source sits after it is pulled toward the attractor.
// t_n = 0.4 - 6/(1 + |z|^2); the point rotates by t_n and steps inward.
// Convergence family (gly-stdlib lineage), ORIGINAL body. Fixed 6-step bound.
vec2 ikedaOrbit(vec2 p, float t, float gain){
  vec2 z = p;
  for (int i = 0; i < 6; i++){
    if (float(i) > gain * 6.0) break;
    float tn = 0.4 - 6.0 / (1.0 + dot(z, z));
    float c = cos(tn + t * 0.3), s = sin(tn + t * 0.3);
    z = vec2(1.0 + gain * (z.x * c - z.y * s),
                   gain * (z.x * s + z.y * c)) * 0.62;
  }
  return z;
}

// Quorum well: seven authorities on a ring, each an orbiting source, combined by a
// polynomial smooth-min whose radius IS the quorum. blend 0 -> seven separate wells;
// blend 1 -> one resolved basin. ORIGINAL body. Fixed 7-iteration loop.
float quorumWell(vec2 uv, float t, float blend, float orb, float depth){
  float k = mix(0.04, 1.35, blend);   // smooth-min radius = the quorum threshold
  float acc = 1e9;
  for (int i = 0; i < 7; i++){
    float a = (float(i) / 7.0) * TAU + t * 0.18;
    vec2 seat = vec2(cos(a), sin(a)) * 0.62;
    // orbit interpolates from a clean ring (orb=0) to a full Ikeda scatter (orb=1)
    vec2 disp = ikedaOrbit(seat * 1.7, t + float(i) * 0.7, max(orb, 0.001));
    seat = mix(seat, seat + disp * 0.85, orb);
    float d = length(uv - seat);
    float h = clamp(0.5 + 0.5 * (acc - d) / k, 0.0, 1.0);
    acc = mix(acc, d, h) - k * h * (1.0 - h);
  }
  return exp(-acc * depth);           // the resolved basin, lit from its floor
}

// Moiré measurement lattice: two rotated unit grids multiplied -> the beat that is
// the unit lattice (U = --row). ORIGINAL body.
float latticeMoire(vec2 uv, float scale, float skew, float t){
  vec2 a = uv * scale;
  vec2 b = (rot2(skew + 0.15 * sin(t * 0.2)) * uv) * scale * 1.03;
  float ga = abs(sin(a.x)) * abs(sin(a.y));
  float gb = abs(sin(b.x)) * abs(sin(b.y));
  return pow(ga * gb, 0.35);          // interference fringe of two grids
}

// Lissajous instrument: the oscilloscope figure — measure, don't theorize. Nearest
// approach of the frame point to a parametric figure. ORIGINAL body. Fixed 40 samples.
float lissaFigure(vec2 uv, float ratio, float t){
  float best = 1e9;
  for (int i = 0; i < 40; i++){
    float u = (float(i) / 40.0) * TAU;
    vec2 c = vec2(sin(ratio * u + t * 0.5), sin((ratio + 1.0) * u)) * 0.72;
    best = min(best, length(uv - c));
  }
  return smoothstep(0.085, 0.0, best); // the traced line (wide enough to read)
}

// Assay sweep: a measuring scanline that travels and reveals. ORIGINAL body.
float assaySweep(vec2 uv, float pos, float t){
  float line = fract(t * 0.11 + pos);
  float y = (uv.y * 0.5 + 0.5);
  return smoothstep(0.12, 0.0, abs(y - line));
}

// Gate seam: dark until one line completes the circuit. As `complete` -> 1 a single
// diagonal seam closes and the field is allowed to light — the gate that enforced
// nothing until one line was re-added. ORIGINAL body.
float gateSeam(vec2 uv, float complete, float t){
  float seam = uv.x * 0.8 + uv.y * 0.6;
  float phase = sin(seam * 3.0 - t * 0.7) * 0.5 + 0.5;
  float closed = smoothstep(1.0 - complete, 1.05 - complete, phase);
  return mix(complete * 0.25, 1.0, closed); // inert floor -> full conduction
}

void main(){
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  uv *= 2.1;
  float t = TIME;

  // structural plate: the measured lattice, always drifting on TIME
  float lat = latticeMoire(uv, latticeScale, latticeSkew, t);

  // the instrument ratio reshapes the field it measures — the Lissajous warp bends
  // the coordinates the convergence resolves in, so `lissa` moves the whole frame.
  vec2 uvW = uv + 0.18 * vec2(sin(lissa * uv.y * 2.3 + t * 0.4),
                              sin((lissa + 1.0) * uv.x * 2.3 - t * 0.4));

  // the seven authorities resolving to one — the core, TIME-driven
  float well = quorumWell(uvW, t, quorum, orbit, basin);

  // the auditor's instruments
  float fig = lissaFigure(uv, lissa, t);
  float scn = assaySweep(uv, sweep, t) * sweep;

  // the gate decides whether the field may light
  float g = gateSeam(uv, gate, t);

  // compose: seed floor, lattice as faint structure, basin as the lit body,
  // instrument + sweep as bright measurement marks — all conducted by the gate.
  float body  = well * (0.55 + 0.75 * lat) * g;
  float marks = (fig * 1.15 + scn * 0.7) * g;

  float h = fract(hue + 0.10 * well + 0.002 * t);
  vec3 lit = hsv2rgb(vec3(h, 0.62 - 0.3 * well, 1.0));
  vec3 col = SEED;
  col += lit * body;
  col += vec3(0.85, 0.93, 1.0) * marks;       // instrument marks read cool-white

  // ink = the grain of the plate: it raises the raw measured lattice + a fine
  // film grain in the UN-lit field (independent of the basin — the paper, not the mark).
  float grain = hash21(floor(uv * RENDERSIZE.y * 0.5) + floor(vec2(t * 6.0)));
  col += (lat * 0.22 + grain * 0.06) * ink * (1.0 - 0.7 * g * well);

  col = pow(clamp(col, 0.0, 1.0), vec3(0.82)); // gentle lift
  gl_FragColor = vec4(col, 1.0);
}
