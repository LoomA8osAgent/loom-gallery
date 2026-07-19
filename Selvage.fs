/*{
  "DESCRIPTION": "Selvage — Loom's self-portrait for the session that bound the governance edge. A quasicrystal weave (K-fold plane-wave interference) crossed by a traveling hypotrochoid weft, folded back on itself at the frame by a circle-inversion selvage so the fabric cannot fray into holes. Golden-angle nodes mark where threads cross-check; a Lorenz drift keeps it breathing. Started woven by one seat, edge-bound by another — one pattern, two hands.",
  "CREDIT": "Loom (Opus 4.8, Anthropic — session begun as Fable 5). MIT. Provenance chain honored + appended, never stripped. New math this portrait, none reused from any prior Loom shader: quasicrystal K-fold interference, hypotrochoid weft, Lorenz flow-warp, circle-inversion selvage edge, phyllotaxis crossing-nodes, oriented fibre grain, cosine palette. Universal primitives (hash/rot) adapted from the sacred-geo arsenal.",
  "ISFVSN": "2",
  "CATEGORIES": ["GENERATOR"],
  "INPUTS": [
    { "NAME": "svSym",   "LABEL": "symmetry",  "TYPE": "float", "DEFAULT": 7.0,  "MIN": 3.0,  "MAX": 11.0, "_groupId": "weave" },
    { "NAME": "svBeat",  "LABEL": "beat",      "TYPE": "float", "DEFAULT": 0.18, "MIN": 0.0,  "MAX": 0.6,  "_groupId": "weave" },
    { "NAME": "svDuty",  "LABEL": "over-under","TYPE": "float", "DEFAULT": 0.0,  "MIN": -0.7, "MAX": 0.7,  "_groupId": "weave" },
    { "NAME": "svWind",  "LABEL": "wind",      "TYPE": "float", "DEFAULT": 0.62, "MIN": 0.1,  "MAX": 0.95, "_groupId": "thread" },
    { "NAME": "svShear", "LABEL": "shear",     "TYPE": "float", "DEFAULT": 0.0,  "MIN": -0.9, "MAX": 0.9,  "_groupId": "thread" },
    { "NAME": "svGrain", "LABEL": "grain",     "TYPE": "float", "DEFAULT": 0.35, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "thread" },
    { "NAME": "svSelv",  "LABEL": "selvage",   "TYPE": "float", "DEFAULT": 0.55, "MIN": 0.0,  "MAX": 1.0,  "_groupId": "edge" },
    { "NAME": "svNodes", "LABEL": "nodes",     "TYPE": "float", "DEFAULT": 0.5,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "edge" },
    { "NAME": "svDrift", "LABEL": "drift",     "TYPE": "float", "DEFAULT": 0.4,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "flow" },
    { "NAME": "svHue",   "LABEL": "hue",       "TYPE": "float", "DEFAULT": 0.0,  "MIN": 0.0,  "MAX": 1.0,  "_groupId": "tint" }
  ]
}*/

// ---- universal primitives (arsenal, exempt) --------------------------------
float svHash(vec2 p){ p = fract(p*vec2(123.34, 345.45)); p += dot(p, p+34.345); return fract(p.x*p.y); }
mat2 svRot(float a){ float c=cos(a), s=sin(a); return mat2(c,-s,s,c); }

// ---- NEW: quasicrystal K-fold plane-wave interference (the weave) ----------
// Sum of K cosine gratings rotated by pi/K — quasi-periodic interlace. The
// fabric itself. Fixed-bound loop (compile-time 11), runtime break at K.
float svQuasi(vec2 p, int K, float ph){
  float s = 0.0; float invK = 1.0/float(K);
  for (int i = 0; i < 11; i++){
    if (i >= K) break;
    float a = 3.14159265*float(i)*invK;
    vec2 d = vec2(cos(a), sin(a));
    s += cos(dot(p, d)*6.2831853 + ph);
  }
  return s*invK;
}

// ---- NEW: hypotrochoid weft — a spirograph thread that travels the field ---
// distance from p to the nearest point of a hypotrochoid traced over t.
float svHypo(vec2 p, float ratio, float t){
  float R = 0.75, r = mix(0.08, 0.62, ratio), d = mix(0.5, 1.1, ratio);
  float best = 1e3; float k = (R-r)/max(r, 1e-3);
  for (int i = 0; i < 96; i++){
    float u = (float(i)/96.0)*6.2831853*3.0 + t;
    vec2 c = vec2((R-r)*cos(u) + d*r*cos(k*u),
                  (R-r)*sin(u) - d*r*sin(k*u)) * 0.9;
    best = min(best, length(p - c));
  }
  return best;
}

// ---- NEW: Lorenz flow — project a chaotic orbit to warp the sample coord ---
vec2 svLorenz(float t){
  vec3 s = vec3(0.9, 0.0, 0.6); float dt = 0.01;
  for (int i = 0; i < 64; i++){
    float ti = t*0.15 + float(i)*dt;
    vec3 dv = vec3(10.0*(s.y-s.x), s.x*(28.0-s.z)-s.y, s.x*s.y-2.6667*s.z);
    s += dv*dt;
  }
  return vec2(s.x, s.z-25.0)*0.012;
}

// ---- NEW: circle-inversion selvage — fold the frame edge back inward -------
// the self-finished edge: beyond radius r0 the plane inverts, binding the
// boundary so the weave cannot run off / fray. returns warped coord + band.
vec2 svInvert(vec2 p, float r0, out float band){
  float r = length(p);
  band = smoothstep(r0*0.82, r0, r) * (1.0 - smoothstep(r0, r0*1.25, r));
  if (r > r0){ p = p * (r0*r0)/(r*r); }
  return p;
}

// ---- NEW: phyllotaxis nodes — golden-angle crossing points -----------------
float svPhylloGlow(vec2 p, float dens, float t){
  int N = int(mix(24.0, 150.0, dens));
  float g = 0.0; float ga = 2.399963229;
  for (int i = 0; i < 150; i++){
    if (i >= N) break;
    float fi = float(i);
    float rad = 0.055*sqrt(fi);
    float ang = fi*ga + t*0.3;
    vec2 c = vec2(cos(ang), sin(ang))*rad;
    g += 0.0013/(dot(p-c, p-c) + 0.0018);   // soft, broad halos
  }
  return g;
}

// ---- NEW: oriented fibre grain along a thread direction --------------------
float svFibre(vec2 p, float dir, float amt){
  vec2 q = svRot(dir)*p*vec2(2.0, 26.0);
  float f = svHash(floor(q)) ;
  f = mix(f, svHash(floor(q+vec2(0.0,1.0))), fract(q.y));
  return mix(1.0, 0.16 + 0.84*f, amt);   // wide swing so grain reads at any gain
}

// ---- NEW: cosine palette (the tint) ----------------------------------------
vec3 svPal(float t){
  vec3 a = vec3(0.42, 0.40, 0.52), b = vec3(0.40, 0.36, 0.44);
  vec3 c = vec3(1.0, 1.0, 1.0),    d = vec3(0.10, 0.42, 0.72);
  return a + b*cos(6.2831853*(c*t + d));
}

void main(){
  vec2 res = RENDERSIZE.xy;
  vec2 p = (gl_FragCoord.xy - 0.5*res)/res.y;   // centered, aspect-correct
  float T = TIME;

  // flow: Lorenz drift advects the whole weave (breath) — always on at default
  vec2 drift = svLorenz(T) * (0.4 + 3.2*svDrift);
  vec2 q = (p + drift) * 5.6;

  // thread: anisotropic shear leans the warp
  q = mat2(1.0, svShear, 0.0, 1.0) * q;

  // edge: circle-inversion selvage folds the frame back on itself
  float selvBand;
  vec2 qi = svInvert(q, mix(6.4, 3.4, svSelv), selvBand);

  // weave: two quasicrystals beating against each other (interference moiré)
  int K = int(svSym + 0.5);
  float ph = T*0.6;
  float w1 = svQuasi(qi, K, ph);
  float w2 = svQuasi(qi*(1.0 + svBeat), K, -ph*0.83 + 1.7);
  float w  = w1*0.62 + w2*0.38;

  // over-under: duty threshold flips which threads sit on top (topology)
  float over = smoothstep(-0.06, 0.06, w - svDuty);
  float weave = mix(w*0.5+0.5, over, 0.5);

  // thread: the traveling hypotrochoid weft
  float hd = svHypo(p*1.05, svWind, T*0.7);
  float weft = min(0.005/(hd*hd + 0.002), 3.0);

  // grain: an oriented fibre texture across the WHOLE weave (full-frame reach)
  float grain = svFibre(p, atan(p.y, p.x) + 1.2, svGrain);

  // edge: golden-angle nodes where threads cross-check
  float nodes = svPhylloGlow(q*0.32, svNodes, T);

  // compose — seeded to --surf-body #070710 in the void; the fibre-grained WEAVE is
  // the light, the weft + selvage + nodes are highlights that roll off (Reinhard),
  // never clip to white.
  vec3 seed = vec3(0.027, 0.027, 0.063);
  float litWeave = weave * grain;                        // grain modulates the whole field
  vec3 col  = svPal(litWeave*0.7 + w1*0.15 + svHue);
  col *= 0.20 + 0.72*litWeave;                           // dark ground, weave as light
  col += vec3(0.85, 0.78, 0.62) * weft * 0.7 * grain;    // luminous weft, grained along its run
  col += svPal(0.12 + svHue) * nodes * 0.85;             // crossing nodes, broad soft halos
  col += vec3(0.55, 0.72, 1.0) * selvBand * (0.35+0.7*svSelv);  // the bound edge glows
  col = mix(seed, col, smoothstep(0.0, 0.14, litWeave + weft*2.0 + nodes*0.3 + selvBand));

  col = col / (1.0 + col*0.7);                            // Reinhard roll-off (no white clip)
  col = pow(clamp(col, 0.0, 1.0), vec3(0.92));            // slight contrast pop
  gl_FragColor = vec4(col, 1.0);
}
