using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class Draw2DSkeleton : MonoBehaviour, IHandFactory, IHandListConsumer
    {
        public GameObject Parent;
        public GameObject leftPrefab;
        public GameObject rightPrefab;

        IHand[] leftHands;
        GameObject[] leftPool;

        IHand[] rightHands;
        GameObject[] rightPool;

        bool need3D;
        int maxCount;

        void Start()
        {
            maxCount = GetComponent<HandService>().maxCount;
            need3D = GetComponent<HandService>().need3D;

            leftHands = new IHand[maxCount];
            leftPool = new GameObject[maxCount];

            rightHands = new IHand[maxCount];
            rightPool = new GameObject[maxCount];

            for (int i = 0; i < maxCount; i++)
            {
                leftHands[i] = Create(out leftPool[i], isLeft: true);
                rightHands[i] = Create(out rightPool[i], isLeft: false);
                leftPool[i].transform.localScale = Vector3.zero;
                rightPool[i].transform.localScale = Vector3.zero;
            }
        }

        public IHand Create(out GameObject obj, bool isLeft)
        {
            obj = Instantiate(isLeft ? leftPrefab : rightPrefab, Parent.transform);
            var hand = obj.GetComponent<IHand>();
            if (hand == null)
                throw new UnityException($"IHand not found with prefab: {(isLeft ? leftPrefab.name : rightPrefab.name)}");
            return hand;
        }

        public void Consume(ref ImageData image, IEnumerable<HandData> list)
        {
            //풀이 없으면 아무일도 하지 않는다.
            if (leftPool[0] == null || rightPool[0] == null) return; 
            if (need3D == true)
            {
                Debug.LogError("Need3D should NOT be checked for using Draw2DSkeleton");
                return;
            }

            int l = 0;
            int r = 0;
            HandData hand;
            foreach (var item in list)
            {
                hand = item;
                if (item.LeftOrRight == LeftOrRightType.Left)
                {
                    leftPool[l].transform.localScale = Vector3.one;
                    leftHands[l].UseHandData(ref image, ref hand, isLeft: true);
                    l++;
                }
                else if (item.LeftOrRight == LeftOrRightType.Right)
                {
                    rightPool[r].transform.localScale = Vector3.one;
                    rightHands[r].UseHandData(ref image, ref hand, isLeft: false);
                    r++;
                }//if hand LeftOrRight is unknown, do nothing
            }
            for (; l < maxCount; l++)
                leftPool[l].transform.localScale = Vector3.zero;
            for (; r < maxCount; r++)
                rightPool[r].transform.localScale = Vector3.zero;
        }
    }
}
