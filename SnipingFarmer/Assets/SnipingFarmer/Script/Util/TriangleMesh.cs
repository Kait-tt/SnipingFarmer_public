using UnityEngine;

namespace SnipingFarmer.Script.Util
{
    // 参考: http://www.shibuya24.info/entry/2015/11/29/180748
    [RequireComponent (typeof(MeshRenderer))]
    [RequireComponent (typeof(MeshFilter))]
    public class TriangleMesh : MonoBehaviourBase
    {
        private void Start ()
        {
            var mesh = new Mesh();
            
            mesh.vertices = new [] {
                new Vector3 (0, 0, 1f),
                new Vector3 (1f, 0, -1f),
                new Vector3 (-1f, 0, -1f)
            };
            
            mesh.triangles = new [] {
                0, 1, 2 
            };
            
            mesh.RecalculateNormals ();
            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = mesh;
        }
    }
}
