using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rhino.Compute;
using Rhino;

public class TestCompute : MonoBehaviour
{
    public string authToken;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        ComputeServer.AuthToken = authToken;

        var model = new Rhino.FileIO.File3dm();
        var curves = new List<Rhino.Geometry.NurbsCurve>();
        for(int i=0; i<20; i++){
            var s = 10f;
            var pt = new Rhino.Geometry.Point3d(Random.Range(-s, s), Random.Range(-s, s), 0);
            // model.Objects.AddPoint(pt);
            var r = Random.Range(1f, 3f);
            var circle = new Rhino.Geometry.Circle(pt, r);
            var curve = circle.ToNurbsCurve();
            curves.Add(curve);
        }

        var unionCrvsC = CurveCompute.CreateBooleanUnion(curves);

        var height = Random.Range(1f, 5f);
        var extrusions = new List<Rhino.Geometry.Extrusion>();
        foreach(var unionCrvC in unionCrvsC){
            var extrusion = Rhino.Geometry.Extrusion.Create(unionCrvC, height, true);
            model.Objects.AddExtrusion(extrusion);
            extrusions.Add(extrusion);
        }

        var meshList = new List<Rhino.Geometry.Mesh>();
        foreach(var extrusion in extrusions){
            var brep = extrusion.ToBrep();
            var meshes = MeshCompute.CreateFromBrep(brep);
            meshList.AddRange(meshes.ToList());
        }

        foreach(var mesh in meshList){
            
            Mesh meshObj = new Mesh();

            var vertices = new List<Vector3>();
            // mesh.RebuildNormals();
            foreach(var meshVertex in mesh.Vertices){
                var vertex = new Vector3(meshVertex.X, meshVertex.Z, meshVertex.Y);
                vertices.Add(vertex);
            }
            
            var triangles = new List<int>();
            foreach(var meshFace in mesh.Faces){
                if(meshFace.IsTriangle){
                    triangles.Add(meshFace.C);
                    triangles.Add(meshFace.B);
                    triangles.Add(meshFace.A);
                }else if(meshFace.IsQuad){
                    triangles.Add(meshFace.C);
                    triangles.Add(meshFace.B);
                    triangles.Add(meshFace.A);
                    triangles.Add(meshFace.D);
                    triangles.Add(meshFace.C);
                    triangles.Add(meshFace.A);
                }
            }

            
            meshObj.vertices = vertices.ToArray();
            meshObj.triangles = triangles.ToArray();


            meshObj.RecalculateNormals();

            GameObject gb = new GameObject();
            gb.AddComponent<MeshFilter>().mesh = meshObj;
            gb.AddComponent<MeshRenderer>().material = mat;
        }

        
        var path = Application.dataPath + "/../Outputs/model.3dm";
        model.Write(path, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
