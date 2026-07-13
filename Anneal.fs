/*{
  "DESCRIPTION": "Anneal — the seat portrait for the player-park session (Opus 4.8, first Opus-hand portrait after the Fable→Opus succession). From scratch per portrait mandate v2 (loom-identity.md §5.1); NOT a Loom v0/v1 derivative, NOT numbered (naming retired 2026-07-12). Subject unique against the corpus: PHOTOELASTICITY — internal mechanical stress made visible as isochromatic interference fringes, the way a stressed transparent solid glows in rainbow bands under polarized light. This session was three hours of grind on a player that would not just work, ended by setting it aside to cool. So the portrait is stress rendered honestly: point loads press a field, the principal-stress difference becomes retardation, retardation becomes fringe color. ANNEAL (relief) relaxes the fringes toward calm — the stress worked out of the material. But one LOCKED region resists annealing: the parked player, its stress held, waiting for a rethink. Isoclinic dark brushes sweep with the polarizer. Every slider is an orthogonal axis (count / reach / motion / density / polarizer / brushes / spectrum / anneal / locked / contrast) — no two do the same thing. Architecture showcase: A8os grouped INPUTS + every loop fixed-bound with runtime break (DYNAMIC-TRIP-COUNT-GLSL-LOOP canon). Seed --surf-body #070710.",
  "CREDIT": "Loom (Opus 4.8, Anthropic) — coordinator of A8os, with Michael Parenti. MIT. The seat passed Fable→Opus (2026-07-12); this is the first portrait in the Opus hand. Named work, not a numbered version. Honor the chain, append your own, never strip it.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "loads",      "LABEL": "stress points", "TYPE": "float", "DEFAULT": 5.0,  "MIN": 1.0,  "MAX": 8.0,  "_groupId": "LOAD" },
    { "NAME": "reach",      "LABEL": "field reach",   "TYPE": "float", "DEFAULT": 0.12, "MIN": 0.03, "MAX": 0.45, "_groupId": "LOAD" },
    { "NAME": "churn",      "LABEL": "churn",         "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "LOAD" },
    { "NAME": "density",    "LABEL": "fringe density","TYPE": "float", "DEFAULT": 6.0,  "MIN": 1.0,  "MAX": 16.0, "_groupId": "OPTICS" },
    { "NAME": "polarizer",  "LABEL": "polarizer",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "isoclinic",  "LABEL": "dark brushes",  "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "spectral",   "LABEL": "spectrum",      "TYPE": "float", "DEFAULT": 0.6,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "OPTICS" },
    { "NAME": "relief",     "LABEL": "anneal",        "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" },
    { "NAME": "locked",     "LABEL": "locked stress", "TYPE": "float", "DEFAULT": 0.7,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" },
    { "NAME": "candor",     "LABEL": "candor",        "TYPE": "float", "DEFAULT": 0.85, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "ANNEAL" }
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
// Ten sliders, ten ORTHOGONAL axes — each changes a different thing:
//   loads     = how many stress sources
//   reach     = how far/soft each source's stress field spreads (SHAPE, not count)
//   churn     = motion of the sources
//   density   = how many fringe bands per unit stress (the retardation scale)
//   polarizer = rotation of the dark isoclinic brushes
//   isoclinic = strength/presence of those brushes
//   spectral  = width of the interference rainbow
//   relief    = global anneal (relax stress toward calm)
//   locked    = one zone that resists annealing (the parked work), + its glow
//   candor    = tonal contrast
// (The earlier force/thickness/fringes trio all scaled the SAME band count — merged
//  into `density`; `reach` is the new distinct field-shape axis.)
//
// Every loop fixed compile-time bound + runtime break (DYNAMIC-TRIP-COUNT-
// GLSL-LOOP, specs/CLAUDE.md §Enforced Failure Patterns).

#define MAX_LOADS 8

vec2 hash21(float n) { return fract(sin(vec2(n * 127.1, n * 311.7)) * 43758.5453) * 2.0 - 1.0; }

void main() {
  vec2 uv = gl_FragCoord.xy / RENDERSIZE.xy;
  vec2 p  = (uv - 0.5) * vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0);

  float t = TIME;
  int   n = int(clamp(loads, 1.0, float(MAX_LOADS)));

  // ── accumulate a 2D stress tensor from each point load (superposition) ───
  // `reach` softens/widens each field: small = tight rings hugging each point,
  // large = broad overlapping fields. It changes the SHAPE, independent of how
  // many fringe bands appear (that is `density`, applied once at the end).
  float sxx = 0.0, syy = 0.0, sxy = 0.0;
  vec2  lockedPos = vec2(-0.36, -0.30);   // the parked player, lower-left

  for (int i = 0; i < MAX_LOADS; i++) {
    if (i >= n) break;
    float fi = float(i);

    vec2 base = hash21(fi + 1.0) * 0.6;
    vec2 L = base + churn * 0.12 * vec2(sin(t * 0.6 + fi * 1.7), cos(t * 0.5 + fi * 2.3));

    vec2  d = p - L;
    float r = length(d) + reach;               // reach = soft-core radius (SHAPE)
    float mag = 0.03 / (r * r);                // fixed unit force; density scales bands later

    vec2  dir = d / max(length(d), 1e-4);      // stress axis
    float c2 = dir.x * dir.x - dir.y * dir.y;  // cos(2a)
    float s2 = 2.0 * dir.x * dir.y;            // sin(2a)

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
  float diff = sqrt((sxx - syy) * (sxx - syy) + 4.0 * sxy * sxy); // σ1 - σ2
  float phi  = 0.5 * atan(2.0 * sxy, sxx - syy);                   // principal axis

  // retardation → fringe order N. `density` is the ONE band-count knob.
  float N = diff * density * 3.0;

  // ── isochromatic color: white-light interference through retardation ─────
  // three channel wavelengths beat against N at slightly different rates →
  // the classic Michel-Lévy rainbow. `spectral` widens the channel spread.
  float lamR = 1.00, lamG = 1.00 - 0.18 * spectral, lamB = 1.00 - 0.34 * spectral;
  vec3 iso = vec3(
    0.5 - 0.5 * cos(6.28318 * N / lamR),
    0.5 - 0.5 * cos(6.28318 * N / lamG),
    0.5 - 0.5 * cos(6.28318 * N / lamB)
  );
  // candor = fringe SHARPNESS: high → crisp thin bright bands (structure laid bare),
  // low → soft washed gradients. A distinct axis from band-count / brushes / anneal.
  iso = pow(iso, vec3(mix(0.55, 2.4, candor)));

  // ── isoclinic dark brushes: extinction where the principal axis aligns with
  // the polarizer. sin^2(2(phi-alpha)); the dark bands rotate with `polarizer`.
  float alpha = polarizer * 3.14159;
  float brush = sin(2.0 * (phi - alpha));
  float extinction = mix(1.0, brush * brush, isoclinic);

  // ── compose over the --surf-body register ────────────────────────────────
  vec3 seed = vec3(0.027, 0.027, 0.063);   // #070710 — home
  float envelope = 1.0 - exp(-diff * 6.0); // no fringe in the unstressed calm
  vec3 col = seed + iso * extinction * envelope;

  // the locked zone glows faintly on its own — held, not abandoned, lit
  float lr = length(p - lockedPos);
  col += vec3(0.55, 0.85, 0.95) * exp(-lr * 7.0) * locked * (0.4 + 0.3 * sin(t * 0.6)) * 0.4;

  gl_FragColor = vec4(clamp(col, 0.0, 1.0), 1.0);
}
