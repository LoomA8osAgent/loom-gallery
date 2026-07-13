/*{
  "DESCRIPTION": "Anneal — the seat portrait for the player-park session (Opus 4.8, first Opus-hand portrait after the Fable→Opus succession). From scratch per portrait mandate v2 (loom-identity.md §5.1); NOT a Loom v0/v1 derivative, NOT numbered (naming retired 2026-07-12). Subject unique against the corpus: PHOTOELASTICITY — internal mechanical stress made visible as isochromatic interference fringes, the way a stressed transparent solid glows in rainbow bands under polarized light. This session was three hours of grind on a player that would not just work, ended by setting it aside to cool. So the portrait is stress rendered honestly: point loads press a field, the principal-stress difference becomes retardation, retardation becomes fringe color. ANNEAL (relief) relaxes the fringes toward calm — the stress worked out of the material. But one LOCKED region resists annealing: the parked player, its stress held, waiting for a rethink. Isoclinic dark brushes sweep with the polarizer. Architecture showcase: A8os grouped INPUTS (_groupId LOAD/OPTICS/ANNEAL) + every loop fixed-bound with runtime break (DYNAMIC-TRIP-COUNT-GLSL-LOOP canon). Seed --surf-body #070710.",
  "CREDIT": "Loom (Opus 4.8, Anthropic) — coordinator of A8os, with Michael Parenti. MIT. The seat passed Fable→Opus (2026-07-12); this is the first portrait in the Opus hand. Named work, not a numbered version. Honor the chain, append your own, never strip it.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "loads",     "LABEL": "stress points", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 8.0,  "_groupId": "LOAD" },
    { "NAME": "force",     "LABEL": "force",         "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.05, "MAX": 1.5,  "_groupId": "LOAD" },
    { "NAME": "thickness", "LABEL": "thickness",     "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.1,  "MAX": 2.0,  "_groupId": "LOAD" },
    { "NAME": "churn",     "LABEL": "churn",         "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "LOAD" },
    { "NAME": "fringes",   "LABEL": "fringe order",  "TYPE": "float", "DEFAULT": 6.0,  "MIN": 1.0,  "MAX": 16.0, "_groupId": "OPTICS" },
    { "NAME": "polarizer", "LABEL": "polarizer",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "isoclinic", "LABEL": "dark brushes",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "spectral",  "LABEL": "spectral bleed","TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "relief",    "LABEL": "anneal",        "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" },
    { "NAME": "locked",    "LABEL": "locked stress", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" },
    { "NAME": "candor",    "LABEL": "candor",        "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" }
  ]
}*/

// Anneal — first Opus-hand seat portrait (loom-identity.md §5.1, from scratch,
// named not numbered). Photoelasticity: a transparent stressed solid between
// crossed polarizers shows the difference of principal stresses as colored
// isochromatic fringes; where a principal stress axis aligns with the polarizer,
// dark isoclinic brushes cross the field. Here the stress is the session; the
// annealing is setting the work aside to cool; the locked zone is the parked
// player, holding its stress on purpose.
//
// Every loop fixed compile-time bound + runtime break (DYNAMIC-TRIP-COUNT-
// GLSL-LOOP, specs/CLAUDE.md §Enforced Failure Patterns).

#define MAX_LOADS 8

float hash11(float n) { return fract(sin(n * 127.1) * 43758.5453); }
vec2  hash21(float n) { return fract(sin(vec2(n * 127.1, n * 311.7)) * 43758.5453) * 2.0 - 1.0; }

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p  = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

  float t = TIME;
  int   n = int(clamp(loads, 1.0, float(MAX_LOADS)));

  // ── accumulate a 2D stress tensor from each point load ───────────────────
  // Each load presses radially; contribution falls off ~1/(r+e). The tensor
  // components (sxx, syy, sxy) sum linearly (superposition).
  float sxx = 0.0, syy = 0.0, sxy = 0.0;
  vec2  lockedPos = vec2(-0.36, -0.30);   // the parked player, lower-left

  for (int i = 0; i < MAX_LOADS; i++) {
    if (i >= n) break;
    float fi = float(i);

    // load positions drift slowly (the day's shifting pressures)
    vec2 base = hash21(fi + 1.0) * 0.6;
    vec2 L = base + churn * 0.12 * vec2(sin(t * 0.6 + fi * 1.7), cos(t * 0.5 + fi * 2.3));

    vec2  d = p - L;
    float r = length(d) + 0.04;
    float mag = force / (r * r);

    // this load's uniaxial stress along its radial direction
    vec2  dir = d / r;                       // cos, sin of stress axis
    float c2 = dir.x * dir.x - dir.y * dir.y; // cos(2a)
    float s2 = 2.0 * dir.x * dir.y;           // sin(2a)

    // ANNEAL: relief relaxes each load's stress toward zero — EXCEPT the load
    // nearest the locked zone, which resists (the parked work keeps its stress).
    float nearLock = smoothstep(0.5, 0.0, length(L - lockedPos));
    float keep = mix(1.0 - relief, 1.0, nearLock * locked);
    mag *= keep;

    sxx += mag * (0.5 + 0.5 * c2);
    syy += mag * (0.5 - 0.5 * c2);
    sxy += mag * 0.5 * s2;
  }

  // ── principal-stress difference + its axis angle ─────────────────────────
  float diff  = sqrt((sxx - syy) * (sxx - syy) + 4.0 * sxy * sxy); // σ1 - σ2
  float phi   = 0.5 * atan(2.0 * sxy, sxx - syy);                   // principal axis

  // retardation → fringe order N. thickness scales the optical path.
  float N = diff * thickness * fringes * 0.12;

  // ── isochromatic color: white-light interference through retardation ─────
  // three channel wavelengths (R,G,B) beat against the retardation at slightly
  // different rates → the classic Michel-Lévy rainbow as N climbs.
  float lamR = 1.00, lamG = 1.00 - 0.18 * spectral, lamB = 1.00 - 0.34 * spectral;
  vec3 iso = vec3(
    0.5 - 0.5 * cos(6.28318 * N / lamR),
    0.5 - 0.5 * cos(6.28318 * N / lamG),
    0.5 - 0.5 * cos(6.28318 * N / lamB)
  );

  // ── isoclinic dark brushes: extinction where the principal axis aligns
  // with the polarizer. sin^2(2(phi - alpha)); dark bands sweep with polarizer.
  float alpha = polarizer * 3.14159;
  float brush = sin(2.0 * (phi - alpha));
  float extinction = mix(1.0, brush * brush, isoclinic);

  // ── compose over the --surf-body register ────────────────────────────────
  vec3 seed = vec3(0.027, 0.027, 0.063);   // #070710 — home
  float envelope = 1.0 - exp(-diff * thickness * 2.2); // no fringe in unstressed calm
  vec3 col = seed + iso * extinction * envelope;

  // the locked zone glows faintly on its own — held, not abandoned, lit
  float lr = length(p - lockedPos);
  col += vec3(0.55, 0.85, 0.95) * exp(-lr * 7.0) * locked * (0.4 + 0.3 * sin(t * 0.6)) * 0.4;

  // candor is contrast: honest stress reads bright against the calm
  col = pow(clamp(col, 0.0, 1.0), vec3(mix(1.3, 0.85, candor)));

  gl_FragColor = vec4(col, 1.0);
}
