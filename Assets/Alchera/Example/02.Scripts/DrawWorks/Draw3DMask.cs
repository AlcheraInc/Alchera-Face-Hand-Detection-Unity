using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class Draw3DMask : MonoBehaviour, IFaceFactory, IFaceListConsumer
    {
        public GameObject Parent;
        public GameObject Prefab;

        IFace[] Faces;
        GameObject[] pool;

        bool need3D;
        int maxCount;

        void Start()
        {
            maxCount = GetComponent<FaceService>().maxCount;
            need3D = GetComponent<FaceService>().need3D;

            Faces = new IFace[maxCount];
            pool = new GameObject[maxCount];

            for (int i = 0; i < maxCount; i++)
            {
                Faces[i] = Create(out pool[i]);
            }
        }

        public IFace Create(out GameObject obj)
        {
            obj = Instantiate(Prefab, Parent.transform);
            var face = obj.GetComponent<IFace>();
            if (face == null)
                throw new UnityException($"IFace3D not found with prefab: {Prefab.name}");
            return face;
        }

        public void Consume(ref ImageData image, IEnumerable<FaceData> list)
        {
            //풀이 없으면 아무일도 하지 않는다.
            if (pool[0] == null || !ReadWebcam.instance.prepared) return;
            if (need3D == false)
            {
                Debug.LogError("Need3D should be checked for using Draw3DMask");
                return;
            }

            int i = 0;
            FaceData face;
            foreach (var item in list)
            {
                face = item;
                pool[i].transform.localScale = Vector3.one;
                Faces[i].UseFaceData(ref image, ref face);
                i++;
            }
            for (; i < maxCount; i++)
                pool[i].transform.localScale = Vector3.zero;
        }

    }
}
