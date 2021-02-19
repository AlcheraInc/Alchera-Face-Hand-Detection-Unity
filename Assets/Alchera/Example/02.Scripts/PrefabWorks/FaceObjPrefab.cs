using UnityEngine;

namespace Alchera
{
    public class FaceObjPrefab : MonoBehaviour, IFace
    {
        Mesh mesh;
        Material mat;
        Vector3[] vertices;
        new Transform transform;

        AutoBackgroundQuad quad;

        public Pose HeadPose
        {
            set
            {
                var lerpRatio = 0.5f;
                transform.localPosition = Vector3.Lerp(transform.localPosition, value.position, lerpRatio);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, value.rotation, lerpRatio);
            }
        }
        
        void Start()
        {
            mat = GetComponentInChildren<MeshRenderer>().material;
            mesh = GetComponentInChildren<MeshFilter>().mesh;
            vertices = mesh.vertices;
            transform = GetComponent<Transform>();
            
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i].x = -vertices[i].x;
                vertices[i].y = vertices[i].y;
                vertices[i].z = -vertices[i].z;
            }
            
            var tris = mesh.triangles;
            int num_tris = 0;
            for (int i =0; i < tris.Length / 3; ++i)
            {
                if (tris[i * 3 + 1] >= 1220 || tris[i * 3 + 2] >= 1220 || tris[i * 3] >= 1220)
                    continue;

                int tmp = tris[i * 3 + 1];
                tris[i * 3 + 1] = tris[i * 3 + 2];
                tris[i * 3 + 2] = tmp;
                ++num_tris;

            }
            
            var ntris = new int[num_tris * 3];
            int idx = 0;
            for (int i = 0; i < tris.Length / 3; ++i)
            {
                if (tris[i * 3 + 1] >= 1220 || tris[i * 3 + 2] >= 1220 || tris[i * 3] >= 1220)
                    continue;

                ntris[idx] = tris[i * 3];
                ++idx;
                ntris[idx] = tris[i * 3 + 1];
                ++idx;
                ntris[idx] = tris[i * 3 + 2];
                ++idx;
            }

            mesh.triangles = ntris;
        }

        void Update()
        {
            float ratio;
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                ratio = Input.mousePosition.x / Screen.width;
#else
            if (Input.touchCount > 0)
            {
                ratio = Input.GetTouch(0).position.x / Screen.width;
#endif
                mat.SetFloat("Vector1_5235FC9A", ratio);
            }  
        }

        public unsafe void UseFaceData(ref ImageData image, ref FaceData face)
        {
            if (quad == null)
            {
                quad = FindObjectOfType<AutoBackgroundQuad>();
                return;
            }

            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
            mirrorX = mirrorX * ReadWebcam.instance.mirror3D;

            transform.parent.localScale = new Vector3(mirrorX, mirrorY, -1.0f);

            
            var pose = face.HeadPose;
            pose.position *= 100;
            HeadPose = pose;

            for (int i = 0; i < 1220; ++i)
                vertices[i] = face.Vertices[i];

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    } 
}
