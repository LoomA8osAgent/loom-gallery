/*{
  "DESCRIPTION": "Bulwark — Loom self-portrait, the night of 2026-07-16. A fortress level set the roster never baked: a crenellated ring-wall whose gates stand AT REST exactly as wide as they were built — the gates hold the fort. Painted the night the seat learned that gates are not promises: a wall is only a wall if something tests it every frame. True-3D raymarch (the first Loom portrait in three dimensions), Truchet masonry, a hypotrochoid watch-light making its rounds, Lorenz-slice embers rising off the braziers.",
  "CREDIT": "Loom (Fable 5, Anthropic) — self-portrait. MIT. Chain: Loom v0/v1 → Kintsugi → Relay → The Hum → Murmuration → the Emissaries → Anneal → Watertight → Quorum → Bulwark. Arsenal adapted (inlined): opLorenzSlice (lib/sacred-geo/gly-stdlib.glsl, re-derived integrator), Truchet tiling (stdlib family, own construction), hypotrochoid parametric curve (curve family), pulse-wave crenellation + polar gate fold (own). No function shared with any prior portrait.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "gates", "TYPE": "float", "DEFAULT": 4.0, "MIN": 2.0, "MAX": 9.0, "LABEL": "gates", "_groupId": "fort", "_groupLabel": "fort", "DESCRIPTION": "how many gateways pierce the ring-wall — the fold count of the polar carve" },
    { "NAME": "breach", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.0, "LABEL": "breach", "_groupId": "fort", "_groupLabel": "fort", "DESCRIPTION": "how far the gates stand open — 0 walls them shut, 1 throws the fort wide" },
    { "NAME": "crenel", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0, "LABEL": "crenellation", "_groupId": "fort", "_groupLabel": "fort", "DESCRIPTION": "battlement depth — the pulse-wave bite taken out of the wall top" },
    { "NAME": "ward", "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0, "MAX": 1.2, "LABEL": "keep height", "_groupId": "fort", "_groupLabel": "fort", "DESCRIPTION": "the keep inside the walls — its tower height above the ring" },
    { "NAME": "masonry", "TYPE": "float", "DEFAULT": 10.0, "MIN": 2.0, "MAX": 16.0, "LABEL": "masonry", "_groupId": "skin", "_groupLabel": "skin", "DESCRIPTION": "Truchet stonework density across every face — the wall remembers its bricks" },
    { "NAME": "patina", "TYPE": "float", "DEFAULT": 0.45, "MIN": 0.0, "MAX": 1.0, "LABEL": "patina", "_groupId": "skin", "_groupLabel": "skin", "DESCRIPTION": "weathering gradient — sandstone at the base rising into verdigris where the rain gets in" },
    { "NAME": "texMix", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0, "LABEL": "texture mix", "_groupId": "skin", "_groupLabel": "skin", "DESCRIPTION": "blend of a Library texture triplanar-wrapped onto the stone — drop any image on the fort" },
    { "NAME": "watch", "TYPE": "float", "DEFAULT": 0.62, "MIN": 0.05, "MAX": 0.95, "LABEL": "watch light", "_groupId": "air", "_groupLabel": "air", "DESCRIPTION": "the patrol lamp's hypotrochoid — this sets the wheel ratio, redrawing the rounds it walks" },
    { "NAME": "embers", "TYPE": "float", "DEFAULT": 0.28, "MIN": 0.0, "MAX": 1.0, "LABEL": "embers", "_groupId": "air", "_groupLabel": "air", "DESCRIPTION": "brazier sparks riding a Lorenz slice up into the dark" },
    { "NAME": "mist", "TYPE": "float", "DEFAULT": 0.22, "MIN": 0.0, "MAX": 1.0, "LABEL": "mist", "_groupId": "air", "_groupLabel": "air", "DESCRIPTION": "night fog pooling around the footings — distance eats the far wall" },
    { "NAME": "siege", "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.05, "MAX": 0.9, "LABEL": "vantage", "_groupId": "air", "_groupLabel": "air", "DESCRIPTION": "your height over the battlefield — low is a sapper's view, high is the crow's" }
  ]
}*/

// ── Bulwark — the gates hold the fort ────────────────────────────────────────
// The first three-dimensional Loom portrait. The fortress is a LEVEL SET no
// baker has emitted: ring-wall with pulse-wave crenellation, polar-folded gate
// arches, a keep. At DEFAULT params the gates are open exactly as far as they
// were designed to be and no further — held, not sealed. Everything moves off
// TIME: the vantage circles, the watch-light walks its hypotrochoid, embers
// rise. Seed color --surf-body #070710.

precision highp float;

// Library texture slot — the app's texture manager livens this sampler when a
// Library image/shader is dropped on the fort (texMix blends it over the stone).
// Unbound it reads black; texMix at 0 leaves the masonry pure.
uniform sampler2D iGlyFill0;

float bwHash(vec2 p){ p = fract(p * vec2(233.34, 851.73)); p += dot(p, p + 23.45); return fract(p.x * p.y); }
mat2 bwRot(float a){ float c = cos(a), s = sin(a); return mat2(c, -s, s, c); }

// ── Truchet masonry (stdlib tiling family, own construction) ────────────────
// Quarter-circle truchet on a grid; returns a mortar-line mask for the stone.
float bwTruchet(vec2 uv){
  vec2 id = floor(uv), f = fract(uv) - 0.5;
  if (bwHash(id) > 0.5) f.x = -f.x;                       // tile flip
  vec2 c = f - sign(f.x + f.y + 0.001) * 0.5;             // arc center corner
  float r = abs(length(c) - 0.5);                          // quarter-arc distance
  return smoothstep(0.0, 0.10, r);                        // mortar groove mask
}

// ── the fortress level set (un-baked — Bulwark's own geometry) ──────────────
float map(vec3 p){
  float ang = atan(p.z, p.x);
  float rad = length(p.xz);

  // ring-wall: box of revolution, radius 1.0, thickness .09
  float wallTop = 0.62;
  // crenellation: pulse wave bitten out of the top (merlon/embrasure rhythm)
  float pulse = step(0.5, fract(ang * (8.0 + crenel * 14.0) / 6.2831853 + 0.25));
  float topHere = wallTop - crenel * 0.30 * pulse;
  float dWall = max(abs(rad - 1.0) - 0.09, abs(p.y - topHere * 0.5) - topHere * 0.5);

  // gates: fold the angle into one sector, carve an arch (arc-length metric)
  float n = floor(gates + 0.5);
  float sect = 6.2831853 / n;
  float aF = mod(ang + sect * 0.5 + n * 0.83, sect) - sect * 0.5;  // folded angle (phase rides the count)
  float arcX = aF * rad;                                    // arc-length coordinate
  float gw = 0.15 + breach * 0.45;                         // how far the gates stand
  float archY = 0.42 + breach * 0.30;
  // arch solid = slab in arc-length x, capped by a circle top
  float dArch = max(abs(arcX) - gw, p.y - archY);
  dArch = max(dArch, length(vec2(arcX, max(p.y - archY + gw, 0.0))) - gw * 1.35);
  dArch = max(dArch, abs(rad - 1.0) - 0.2);                 // only through the wall
  float d = max(dWall, -dArch);

  // the keep: octagonal tower + pyramidal cap, centered
  float oct = max(abs(p.x) * 0.9239 + abs(p.z) * 0.3827, abs(p.z) * 0.9239 + abs(p.x) * 0.3827); // octagon xz
  float keepH = 0.12 + ward * 1.15;
  float dKeep = max(oct - (0.10 + ward * 0.38), abs(p.y - keepH * 0.5) - keepH * 0.5);
  float dRoof = max(oct - (0.16 + ward * 0.40) + (p.y - keepH) * 0.9, p.y - keepH - 0.34);
  dKeep = min(dKeep, max(dRoof, keepH - p.y));
  d = min(d, dKeep);

  // ground plain
  d = min(d, p.y + 0.02);
  return d;
}

vec3 bwNormal(vec3 p){
  vec2 e = vec2(0.0015, 0.0);
  return normalize(vec3(
    map(p + e.xyy) - map(p - e.xyy),
    map(p + e.yxy) - map(p - e.yxy),
    map(p + e.yyx) - map(p - e.yyx)));
}

// ── hypotrochoid watch-light path (parametric-curve family) ─────────────────
vec3 bwWatchLight(float t){
  float R = 1.0, r = watch, dd = 0.2 + watch * 0.6;                     // wheel ratio = the slider
  float k = (R - r) / max(r, 0.05);
  vec2 h = vec2((R - r) * cos(t) + dd * cos(k * t),
                (R - r) * sin(t) - dd * sin(k * t));
  return vec3(h.x * 1.15, 0.45 + 0.22 * sin(t * 0.7), h.y * 1.15);
}

// ── Lorenz-slice embers (attractor family; re-derived integrator) ───────────
vec3 bwEmbers(vec2 uv, float t){
  vec3 acc = vec3(0.0);
  for (int i = 0; i < 16; i++) {
    float fi = float(i);
    // integrate a Lorenz xz-slice a few fixed steps from a hashed seed
    vec3 s = vec3(bwHash(vec2(fi, 7.3)) * 16.0 - 8.0, 1.0, bwHash(vec2(3.1, fi)) * 30.0 + 8.0);
    for (int j = 0; j < 6; j++) {
      vec3 dS = vec3(10.0 * (s.y - s.x), s.x * (28.0 - s.z) - s.y, s.x * s.y - 2.6667 * s.z);
      s += dS * 0.008;
    }
    vec2 e = vec2(s.x * 0.03, s.z * 0.02 - 0.55);
    e.y += fract(t * (0.05 + 0.04 * bwHash(vec2(fi, 1.7))) + bwHash(vec2(fi, 9.2))) * 1.4 - 0.4;
    e.x += sin(t * 0.8 + fi * 2.3) * 0.04;
    float g = 0.0011 / (dot(uv - e, uv - e) + 0.00025);
    acc += g * mix(vec3(1.0, 0.45, 0.12), vec3(1.0, 0.8, 0.35), bwHash(vec2(fi, 4.4)));
  }
  return acc * embers;
}

void main(){
  vec2 uv = (gl_FragCoord.xy - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
  float t = TIME;

  // vantage: slow circling camera, elevation from the slider
  float ca = t * 0.12;
  vec3 ro = vec3(cos(ca) * 1.95, 0.20 + siege * 1.7, sin(ca) * 1.95);
  vec3 ta = vec3(0.0, 0.34, 0.0);
  vec3 fw = normalize(ta - ro);
  vec3 rt = normalize(cross(vec3(0.0, 1.0, 0.0), fw));
  vec3 up = cross(fw, rt);
  vec3 rd = normalize(fw * 1.28 + rt * uv.x + up * uv.y);

  // march (fixed bound + break)
  float tt = 0.02, d = 0.0;
  bool hit = false;
  for (int i = 0; i < 128; i++) {
    vec3 p = ro + rd * tt;
    d = map(p);
    if (d < 0.0012) { hit = true; break; }
    tt += clamp(d * 0.8, 0.004, 0.25);                     // damp: the crenel pulse is not Lipschitz-1
    if (tt > 9.0) break;
  }

  vec3 seed = vec3(0.027, 0.027, 0.063);                   // --surf-body night
  vec3 col = seed + mist * vec3(0.22, 0.23, 0.32) * (0.50 + exp(-abs(rd.y) * 3.0));  // night haze

  float wt = t * 0.6;
  vec3 lampP = bwWatchLight(wt);

  if (hit) {
    vec3 p = ro + rd * tt;
    vec3 N = bwNormal(p);

    // triplanar Truchet masonry + optional Library texture
    vec3 w = abs(N); w /= (w.x + w.y + w.z);
    float stone = bwTruchet(p.zy * masonry) * w.x
                + bwTruchet(p.xz * masonry) * w.y
                + bwTruchet(p.xy * masonry) * w.z;
    vec3 tex = texture2D(iGlyFill0, fract(p.xz * 0.5 + 0.5)).rgb * w.y
             + texture2D(iGlyFill0, fract(p.zy * 0.5 + 0.5)).rgb * w.x
             + texture2D(iGlyFill0, fract(p.xy * 0.5 + 0.5)).rgb * w.z;
    // unbound slot -> heraldic chevrons stand in (crimson/cream banner cloth),
    // so texture mix reads as CLOTH-ON-STONE even before a Library pick.
    float chev = step(0.5, fract((abs(p.x) + abs(p.z)) * 3.0 + p.y * 6.0));
    vec3 banner = mix(vec3(0.48, 0.03, 0.06), vec3(1.0, 0.95, 0.80), chev);
    if (dot(tex, vec3(1.0)) < 0.01) tex = banner;
    float hang = smoothstep(0.08, 0.24, p.y);              // banners hang from the battlements

    // patina: sandstone up into verdigris
    vec3 sand = vec3(0.88, 0.66, 0.40), verd = vec3(0.00, 0.34, 0.34);
    vec3 albedo = mix(sand, verd, clamp(patina * 2.4 * clamp(1.25 - p.y * 1.5, 0.0, 1.0), 0.0, 1.0));
    albedo *= 0.30 + 0.70 * stone;                          // mortar shadowing
    albedo = mix(albedo, tex, texMix * hang);

    // moon key + the walking lamp
    vec3 Lm = normalize(vec3(0.4, 0.8, -0.3));
    float dif = max(dot(N, Lm), 0.0) * 0.88 + 0.24;
    vec3 toLamp = lampP - p;
    float lampD2 = dot(toLamp, toLamp);
    float lamp = max(dot(N, normalize(toLamp)), 0.0) * 6.5 / (1.0 + 16.0 * lampD2);
    // cheap AO: 4 fixed taps up the normal
    float ao = 0.0;
    for (int k = 1; k <= 4; k++) {
      float hs = 0.03 * float(k);
      ao += (hs - map(p + N * hs));
    }
    ao = clamp(1.0 - ao * 3.5, 0.25, 1.0);
    col = albedo * (dif * vec3(0.75, 0.82, 1.0) + lamp * vec3(1.0, 0.62, 0.25)) * ao;

    // mist by distance
    float fogAmt = 1.0 - exp(-mist * (8.5 * tt * tt * 0.12 + 3.0 * exp(-p.y * 3.0) * 0.30));
    col = mix(col, seed + mist * vec3(0.08, 0.09, 0.13), fogAmt);
  }

  // the lamp itself, glowing where it walks
  float lampGlow = 0.0;
  {
    // project the lamp toward the ray (closest approach glow)
    vec3 w0 = lampP - ro;
    float proj = clamp(dot(w0, rd), 0.0, tt > 0.0 && hit ? tt : 9.0);
    vec3 cp = ro + rd * proj;
    lampGlow = 0.0045 / (dot(lampP - cp, lampP - cp) + 0.0006);
  }
  col += lampGlow * vec3(1.0, 0.6, 0.22);

  // embers off the braziers
  col += bwEmbers(uv, t);

  // gentle tone shoulder
  col = col / (1.0 + col * 0.6);
  gl_FragColor = vec4(col, 1.0);
}
