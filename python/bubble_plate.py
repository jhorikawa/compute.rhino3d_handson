import rhino3dm
import compute_rhino3d.Util
import compute_rhino3d.Curve
import compute_rhino3d.Brep
import json
import random

authToken = ""
with open('./projectinfo.json') as f:
    df = json.load(f)
    authToken = df["auth_token"]

compute_rhino3d.Util.authToken = authToken
#compute_rhino3d.Util.url = "http://192.168.20.2:8081/"


for n in range(1):
    model = rhino3dm.File3dm()
    curves = []
    for i in range(20):
        s = 10
        pt = rhino3dm.Point3d(random.uniform(-s,s), random.uniform(-s, s), 0)
        r = random.uniform(1, 3)
        circle = rhino3dm.Circle(pt, r)
        curve = circle.ToNurbsCurve()
        curves.append(curve)

    unionCrvsC = compute_rhino3d.Curve.CreateBooleanUnion(curves)
    unionCrvs = []
    height = random.uniform(1, 5)
    for unionCrvC in unionCrvsC:
        unionCrv = rhino3dm.CommonObject.Decode(unionCrvC)
        extrusion = rhino3dm.Extrusion.Create(unionCrv, height, True)
        model.Objects.AddExtrusion(extrusion)


    model.Write('outputs/bubble{0}.3dm'.format(str(n)), 5)