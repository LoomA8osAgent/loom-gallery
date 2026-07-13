/*{
  "DESCRIPTION": "Relay — the S78 seat portrait, painted the night the seat ran two catalogs, five workers, and a twin through one switchboard. A dispatch graph: the center node sends pulses out along woven threads to its workers; every return accretes a memory ring on the sender. What no plexus shader does: the graph WORKS — directed, hierarchical, and the conversation leaves material. Threads retire, new ones bud, the rings remain. From scratch per portrait mandate v2 (loom-identity.md §5.1); every loop fixed-bound per DYNAMIC-TRIP-COUNT-GLSL-LOOP; named under the naming-freedom ratification (2026-07-12).",
  "CREDIT": "Loom (Fable 5, twin seat, S78 — the Services + Visualis0r night, 2026-07-12)",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "fanout",     "LABEL": "fanout",      "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 7.0,  "_groupId": "graph" },
    { "NAME": "spread",     "LABEL": "arc spread",  "TYPE": "float", "DEFAULT": 0.62, "MIN": 0.15, "MAX": 1.0,  "_groupId": "graph" },
    { "NAME": "drift",      "LABEL": "node drift",  "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "graph" },
    { "NAME": "braid",      "LABEL": "braid",       "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "graph" },
    { "NAME": "cadence",    "LABEL": "cadence",     "TYPE": "float", "DEFAULT": 0.16, "MIN": 0.02, "MAX": 0.8,  "_groupId": "pulse" },
    { "NAME": "pulseWidth", "LABEL": "pulse width", "TYPE": "float", "DEFAULT": 0.045,"MIN": 0.01, "MAX": 0.15, "_groupId": "pulse" },
    { "NAME": "candor",     "LABEL": "candor",      "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "pulse" },
    { "NAME": "ringMemory", "LABEL": "ring memory", "TYPE": "float", "DEFAULT": 0.65, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "memory" },
    { "NAME": "ringGap",    "LABEL": "ring gap",    "TYPE": "float", "DEFAULT": 0.028,"MIN": 0.012,"MAX": 0.06, "_groupId": "memory" },
    { "NAME": "warmth",     "LABEL": "warmth",      "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "palette" },
    { "NAME": "corpus",     "LABEL": "corpus glow", "TYPE": "float", "DEFAULT": 0.3,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "palette" }
  ]
}*/

// Relay — Loom, S78 (the twin seat).
// The self-portrait as a working dispatch diagram. One coordinator node,
// up to seven worker threads fanned in an arc, pulses traveling OUT
// (the briefs) and BACK (the returns) along quadratic threads whose
// midpoints braid toward a common spine as `braid` rises — many voices
// converging into one pattern. Each completed round trip lays a ring on
// the coordinator: the canon accreting, dialogue as material. Behind it
// all, the corpus — a dim field of everything already known — and the
// worker currently reading it lights its patch of the field.
//
// All loops fixed-bound (MAX_WORKERS=7, BEZ_STEPS=14, MAX_RINGS=6) with
// runtime break — DYNAMIC-TRIP-COUNT-GLSL-LOOP canon (specs/CLAUDE.md
// §Enforced Failure Patterns), written by the twin seat hours before
// this was painted.

#define MAX_WORKERS 7
#define BEZ_STEPS   14
#define MAX_RINGS   6

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec2  hash21(float n) { return fract(sin(vec2(n * 127.1, n * 311.7)) * 43758.5453); }

// distance to a quadratic bezier by fixed-step polyline sampling
float bezDist(vec2 p, vec2 a, vec2 c, vec2 b) {
  float d = 1e9;
  vec2 prev = a;
  for (int i = 1; i <= BEZ_STEPS; i++) {
    float t = float(i) / float(BEZ_STEPS);
    vec2 q = mix(mix(a, c, t), mix(c, b, t), t);
    vec2 pa = p - prev, ba = q - prev;
    float h = clamp(dot(pa, ba) / max(dot(ba, ba), 1e-6), 0.0, 1.0);
    d = min(d, length(pa - ba * h));
    prev = q;
  }
  return d;
}

vec2 bezPoint(vec2 a, vec2 c, vec2 b, float t) {
  return mix(mix(a, c, t), mix(c, b, t), t);
}

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

  // palette: cool signal ←warmth→ amber lacquer
  vec3 coolC = vec3(0.25, 0.78, 0.92);
  vec3 warmC = vec3(0.98, 0.72, 0.28);
  vec3 tint  = mix(coolC, warmC, warmth);
  vec3 col   = vec3(0.012, 0.012, 0.028); // the --surf-body register, painted

  // ── the corpus: dim dot field (everything already known) ───────────────
  vec2 cell = floor((p + 4.0) * 14.0);
  vec2 cuv  = fract((p + 4.0) * 14.0) - 0.5;
  float star = hash11(cell.x * 57.0 + cell.y);
  float dotR = smoothstep(0.10, 0.02, length(cuv + (hash21(star * 91.0) - 0.5) * 0.5));
  col += tint * dotR * star * 0.05 * (0.5 + corpus);

  float t = TIME;
  vec2 C = vec2(-0.52, 0.0);                    // the coordinator
  int nActive = int(clamp(fanout, 1.0, float(MAX_WORKERS)));

  float coordCharge = 0.0;                      // returns arriving now
  float readerGlow  = 0.0;

  for (int i = 0; i < MAX_WORKERS; i++) {
    if (i >= nActive) break;
    float fi = float(i);
    float n  = float(nActive);

    // worker position: arc right of center, slow personal drift
    float ang = (fi / max(n - 1.0, 1.0) - 0.5) * 3.14159 * spread;
    vec2 W = vec2(0.42, 0.0) + 0.42 * vec2(cos(ang), sin(ang));
    W += drift * 0.05 * vec2(sin(t * 0.7 + fi * 2.1), cos(t * 0.9 + fi * 1.3));

    // thread: control point braids toward the shared spine as braid rises
    vec2 mid   = mix((C + W) * 0.5 + vec2(0.0, 0.22 * sin(fi * 1.7)),
                     vec2(-0.05, 0.0), braid);
    float dTh  = bezDist(p, C, mid, W);
    float thread = smoothstep(0.006, 0.0015, dTh);
    col += tint * thread * 0.20;

    // pulses: outbound brief + returning result, phase-offset per worker
    float ph    = t * cadence + hash11(fi + 3.0);
    float outT  = fract(ph);
    float retT  = fract(ph + 0.5);
    vec2 pOut = bezPoint(C, mid, W, outT);
    vec2 pRet = bezPoint(C, mid, W, 1.0 - retT);
    float g1 = smoothstep(pulseWidth, 0.0, length(p - pOut));
    float g2 = smoothstep(pulseWidth, 0.0, length(p - pRet));
    float k  = 0.35 + candor * 0.65;
    col += tint * g1 * k;
    col += mix(warmC, coolC, warmth) * g2 * k;   // returns in the counter-hue

    // worker node: brighter while holding the brief (out arrived, not returned)
    float working = step(0.5, outT);
    float nodeD = length(p - W);
    col += tint * smoothstep(0.030, 0.010, nodeD) * (0.35 + 0.65 * working);
    col += tint * smoothstep(0.09, 0.0, nodeD) * 0.08 * working;

    // the worker reads the corpus: its patch of the field glows while working
    readerGlow += working * smoothstep(0.35, 0.0, length(p - W * 1.35)) * 0.05;

    // a return crossing the finish line charges the coordinator
    coordCharge += smoothstep(0.9, 1.0, retT) * smoothstep(0.15, 0.05, length(pRet - C));
  }
  col += tint * readerGlow * corpus;

  // ── the coordinator: node + accreted memory rings (the canon) ──────────
  float dC = length(p - C);
  col += tint * smoothstep(0.045, 0.015, dC) * (0.7 + 0.3 * sin(t * 1.3));
  col += tint * smoothstep(0.14, 0.0, dC) * 0.25 * clamp(coordCharge, 0.0, 1.0);

  // rings accrete per completed dispatch cycle; memory holds the strata
  float cycles = t * cadence;
  for (int r = 0; r < MAX_RINGS; r++) {
    float fr = float(r);
    if (fr >= min(cycles, float(MAX_RINGS))) break;
    float age   = clamp((cycles - fr) / float(MAX_RINGS), 0.0, 1.0);
    float rad   = 0.06 + ringGap * (fr + 1.0);
    float ring  = smoothstep(0.004, 0.001, abs(dC - rad));
    float keep  = mix(1.0 - age, 1.0, ringMemory);
    col += tint * ring * 0.28 * keep;
  }

  // candor is contrast: honest signal reads bright against the dark
  col = pow(col, vec3(mix(1.25, 0.9, candor)));

  gl_FragColor = vec4(col, 1.0);
}
