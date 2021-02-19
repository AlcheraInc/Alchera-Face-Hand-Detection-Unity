using UnityEngine;

namespace Alchera
{
    public class Skeleton2DPrefab : MonoBehaviour, IHand
    {
        Transform[] skeleton;
        Transform[] box;
        TextMesh Posture;
        AutoBackgroundQuad quad;
        float adjustment;
        float centerX;
        float centerY;
        int mirrorX;
        int mirrorY;
        void Start()
        {
            //  wrist   : 0
            //  thumb   : 1~4
            //  1st     : 5~8
            //  2nd     : 9~12
            //  3rd     : 13~16
            //  4th     : 17~20
            //  link fingerpoints       : 21~35
            //  link finger with wrist  : 36~41
            skeleton = new Transform[HandData.NumPoints + 21];
            box = new Transform[4 + 4];
            for (int i = 0; i < skeleton.Length; i++)
                skeleton[i] = transform.GetChild(i);
            for (int i = 0; i < box.Length; i++)
                box[i] = transform.GetChild(skeleton.Length + i);

            Posture = transform.GetComponentInChildren<TextMesh>();
        }
        public void UseHandData(ref ImageData image, ref HandData hand, bool isLeft)
        {
            if (skeleton == null) return;
            if (quad == null)
            {
                quad = FindObjectOfType<AutoBackgroundQuad>();
                return;
            }

            SetPoints(ref image,ref hand);
            SetLinks();
            SetBox(ref image, ref hand);
            SetPosture(ref image, ref hand);
        }
        public unsafe void SetPoints(ref ImageData image,ref HandData hand)
        {
            var ptr = hand.Points;

            var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by zero 방지 
            adjustment = System.Math.Abs(quad.transform.localScale.y / height);

            centerX = quad.texture.width / 2;
            centerY = quad.texture.height / 2;
            if (ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0)
            {
                centerX = quad.texture.height / 2;
                centerY = quad.texture.width / 2;
            }
            ReadWebcam.instance.GetMirrorValue(out mirrorX, out mirrorY);

            for (var p = 0; p < HandData.NumPoints; ++p)
            {
                var posX = mirrorX * (ptr[p].x - centerX + image.OffsetX) * adjustment;
                var posY = mirrorY * (ptr[p].y - centerY + image.OffsetY) * adjustment;
                var posZ = quad.transform.localPosition.z;

                var newPos = Vector3.Lerp(skeleton[p].localPosition, new Vector3(posX, posY, posZ), 0.75f);
                skeleton[p].localPosition = newPos;
                skeleton[p].localScale = Vector3.one * adjustment * 15;
            }
        }
        public unsafe void SetBox(ref ImageData image, ref HandData hand)
        {
            Vector2[] boxPos = { new Vector2(hand.Box.x, hand.Box.y),
                                 new Vector2(hand.Box.x + hand.Box.width, hand.Box.y),
                                 new Vector2(hand.Box.x, hand.Box.y + hand.Box.height),
                                 new Vector2(hand.Box.x + hand.Box.width, hand.Box.y + hand.Box.height)};

            for (int i = 0; i < 4; i++)
            {
                var posX = mirrorX * (boxPos[i].x - centerX + image.OffsetX) * adjustment;
                var posY = mirrorY * (boxPos[i].y - centerY + image.OffsetY) * adjustment;
                var posZ = quad.transform.localPosition.z;

                var newPos = Vector3.Lerp(box[i].localPosition, new Vector3(posX, posY, posZ), 0.75f);
                box[i].localPosition = newPos;
                box[i].localScale = Vector3.one * adjustment * 15;
            }
            SetBoxLink(4, 0, 1, adjustment * 20);
            SetBoxLink(5, 1, 3, adjustment * 20);
            SetBoxLink(6, 2, 3, adjustment * 20);
            SetBoxLink(7, 2, 0, adjustment * 20);
        }
        public unsafe void SetPosture(ref ImageData image, ref HandData hand)
        {
            Posture.text =  $"{hand.Posture}";
        }
        public void SetLinks()
        {
            var scaleFactor = adjustment * 40;
            SetSkeletonLink(21, 1, 2, scaleFactor);
            SetSkeletonLink(22, 2, 3, scaleFactor);
            SetSkeletonLink(23, 3, 4, scaleFactor);

            SetSkeletonLink(24, 5, 6, scaleFactor);
            SetSkeletonLink(25, 6, 7, scaleFactor);
            SetSkeletonLink(26, 7, 8, scaleFactor);

            SetSkeletonLink(27, 9, 10, scaleFactor);
            SetSkeletonLink(28, 10, 11, scaleFactor);
            SetSkeletonLink(29, 11, 12, scaleFactor);

            SetSkeletonLink(30, 13, 14, scaleFactor);
            SetSkeletonLink(31, 14, 15, scaleFactor);
            SetSkeletonLink(32, 15, 16, scaleFactor);

            SetSkeletonLink(33, 17, 18, scaleFactor);
            SetSkeletonLink(34, 18, 19, scaleFactor);
            SetSkeletonLink(35, 19, 20, scaleFactor);

            SetSkeletonLink(36, 0, 1, scaleFactor);
            SetSkeletonLink(37, 1, 5, scaleFactor);
            SetSkeletonLink(38, 5, 9, scaleFactor);
            SetSkeletonLink(39, 9, 13, scaleFactor);
            SetSkeletonLink(40, 13, 17, scaleFactor);
            SetSkeletonLink(41, 17, 0, scaleFactor);
        }

        unsafe void SetSkeletonLink(int linkIdx, int baseIdx, int targetIdx, float scaleFactor)
        {
            if (skeleton == null)
            {
                return;
            }
            Vector3 basePos = skeleton[baseIdx].localPosition;
            Vector3 targetPos = skeleton[targetIdx].localPosition;
            skeleton[linkIdx].localPosition = (basePos + targetPos) * 0.5f;
            Vector3 diff = targetPos - basePos;

            float dist = diff.magnitude;

            if (dist < 1.0e-8)
            {
                skeleton[linkIdx].localRotation = Quaternion.identity;
            }
            else
            {
                skeleton[linkIdx].localScale = new Vector3(scaleFactor * 0.25f, dist / 2.0f, scaleFactor * 0.25f);

                Quaternion localQuaternion = Quaternion.identity;
                localQuaternion.SetFromToRotation(new Vector3(0, 1, 0), diff / dist);

                skeleton[linkIdx].localRotation = localQuaternion;
            }
        }
        unsafe void SetBoxLink(int linkIdx, int baseIdx, int targetIdx, float scaleFactor)
        {
            if (box == null)
            {
                return;
            }
            Vector3 basePos = box[baseIdx].localPosition;
            Vector3 targetPos = box[targetIdx].localPosition;
            box[linkIdx].localPosition = (basePos + targetPos) * 0.5f;
            Vector3 diff = targetPos - basePos;

            float dist = diff.magnitude;

            if (dist < 1.0e-8)
            {
                box[linkIdx].localRotation = Quaternion.identity;
            }
            else
            {
                box[linkIdx].localScale = new Vector3(scaleFactor * 0.25f, dist / 2.0f, scaleFactor * 0.25f);

                Quaternion localQuaternion = Quaternion.identity;
                localQuaternion.SetFromToRotation(new Vector3(0, 1, 0), diff / dist);

                box[linkIdx].localRotation = localQuaternion;
            }
        }
    }
}

