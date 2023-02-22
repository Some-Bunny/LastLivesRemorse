using Dungeonator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LastLivesRemorse
{
    public class CustomScarfDoerActorless : MonoBehaviour
    {
        public CustomScarfDoerActorless()
        {
            StartWidth = 0.0625f;
            EndWidth = 0.125f;
            AnimationSpeed = 1f;
            ScarfLength = 1.5f;
            AngleLerpSpeed = 15f;
            BackwardZOffset = -0.2f;
            CatchUpScale = 2f;
            SinSpeed = 1f;
            AmplitudeMod = 0.25f;
            WavelengthMod = 1f;

            m_currentLength = 0.05f;
            var scarfMat = PickupObjectDatabase.GetById(436) as BlinkPassiveItem;
            ScarfMaterial = scarfMat.ScarfPrefab.ScarfMaterial;
        }

        public Material ScarfMaterial;

        public float StartWidth;
        public float EndWidth;
        public float AnimationSpeed;
        public float ScarfLength;
        public float BaseScarfLength;

        public float AngleLerpSpeed;
        public float ForwardZOffset;
        public float BackwardZOffset;
        public float CatchUpScale;
        public float SinSpeed;
        public float AmplitudeMod;
        public float WavelengthMod;
        public Vector2 MinOffset;
        public Vector2 MaxOffset;
        public Vector2 offset = Vector2.zero;


        public Vector2 LastDirection = Vector2.down;
        public Vector2 BaseLastDirection = Vector2.down;

        public GameObject AttachTarget;
        public Transform AttachTransform;


        private Vector2 m_currentOffset;
        private Vector3[] m_vertices;
        private Mesh m_mesh;
        private MeshFilter m_stringFilter;
        private MeshRenderer m_mr;
        private bool m_isLerpingBack;
        private float m_additionalOffsetTime;
        private float m_lastVelAngle;
        private float m_targetLength;
        private float m_currentLength;


        public void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            m_targetLength = ScarfLength;
            m_currentOffset = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), 2f);
            m_mesh = new Mesh();
            m_vertices = new Vector3[20];
            m_mesh.vertices = m_vertices;
            int[] array = new int[54];
            Vector2[] uv = new Vector2[20];
            int num = 0;
            for (int i = 0; i < 9; i++)
            {
                array[i * 6] = num;
                array[i * 6 + 1] = num + 2;
                array[i * 6 + 2] = num + 1;
                array[i * 6 + 3] = num + 2;
                array[i * 6 + 4] = num + 3;
                array[i * 6 + 5] = num + 1;
                num += 2;
            }
            m_mesh.triangles = array;
            m_mesh.uv = uv;
            GameObject gameObject = new GameObject("balloon string");
            m_stringFilter = gameObject.AddComponent<MeshFilter>();
            m_mr = gameObject.AddComponent<MeshRenderer>();
            m_mr.material = ScarfMaterial;

            m_stringFilter.mesh = m_mesh;
            transform.position = AttachTransform.position + m_currentOffset.ToVector3ZisY(-3f);
        }

        private void LateUpdate()
        {
            transform.position = AttachTransform.position + m_currentOffset.ToVector3ZisY(-3f) + offset.ToVector3ZUp();

            if (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating) { return; }
            if (AttachTarget != null)
            {

                m_targetLength = ScarfLength;
                bool flag = false;


                m_currentLength = Mathf.MoveTowards(m_currentLength, m_targetLength, BraveTime.DeltaTime * 2.5f);
                if (m_currentLength < 0.1f) { m_mr.enabled = false; }

                Vector2 lastCommandedDirection = LastDirection;
                if (BraveMathCollege.Atan2Degrees(lastCommandedDirection) <= 155f && BraveMathCollege.Atan2Degrees(lastCommandedDirection) >= 25f)
                {
                    flag = true;
                }

                if (lastCommandedDirection.magnitude < 0.125f) { m_isLerpingBack = true; } else { m_isLerpingBack = false; }

                this.m_mr.enabled = true;
                this.m_mr.gameObject.layer = AttachTarget.gameObject.layer - 2;

                float num = m_lastVelAngle;
                if (m_isLerpingBack)
                {
                    float num2 = Mathf.DeltaAngle(m_lastVelAngle, -45f);
                    float num3 = Mathf.DeltaAngle(m_lastVelAngle, 135f);
                    float num4 = ((num2 <= num3) ? 0 : 180);
                    num = num4;
                }
                else
                {
                    num = BraveMathCollege.Atan2Degrees(lastCommandedDirection);
                }

                m_lastVelAngle = Mathf.LerpAngle(m_lastVelAngle, num, BraveTime.DeltaTime * AngleLerpSpeed * Mathf.Lerp(1f, 2f, Mathf.DeltaAngle(m_lastVelAngle, num) / 180f));

                float d = m_currentLength * Mathf.Lerp(2f, 1f, Vector2.Distance(transform.position.XY(), AttachTransform != null ? AttachTransform.PositionVector2() + offset : AttachTarget.transform.PositionVector2() + offset) / 3f);

                m_currentOffset = (Quaternion.Euler(0f, 0f, m_lastVelAngle) * Vector2.left * d).XY();
                Vector2 b = Vector2.Lerp(MinOffset, MaxOffset, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.realtimeSinceStartup * AnimationSpeed, 3f) / 3f));

                m_currentOffset += b;

                Vector3 vector = AttachTransform != null? AttachTransform.transform.PositionVector2() + new Vector2(0f, -0.3125f) + offset : AttachTarget.transform.PositionVector2() + new Vector2(0f, -0.3125f) + offset;

                Vector3 vector2 = vector + m_currentOffset.ToVector3ZisY(-3f);

                float num5 = Vector3.Distance(transform.position, vector2);
                if (num5 > 10f)
                {
                    transform.position = vector2;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, vector2, BraveMathCollege.UnboundedLerp(1f, 10f, num5 / CatchUpScale) * BraveTime.DeltaTime);
                }
                Vector2 b2 = vector2.XY() - transform.position.XY();
                m_additionalOffsetTime += UnityEngine.Random.Range(0f, BraveTime.DeltaTime);
                BuildMeshAlongCurve(vector, vector.XY() + new Vector2(0f, 0.1f), transform.position.XY() + b2, transform.position.XY(), (!flag) ? ForwardZOffset : BackwardZOffset);
                m_mesh.vertices = m_vertices;
                m_mesh.RecalculateBounds();
                m_mesh.RecalculateNormals();
            }
        }

        private void OnDestroy()
        {
            if (m_stringFilter) { Destroy(m_stringFilter.gameObject); }
        }

        private void BuildMeshAlongCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float zOffset)
        {
            Vector3[] vertices = m_vertices;
            Vector2? vector = null;
            Vector2 v = p3 - p0;
            Vector2 vector2 = (Quaternion.Euler(0f, 0f, 90f) * v).XY();
            for (int i = 0; i < 10; i++)
            {
                Vector2 vector3 = BraveMathCollege.CalculateBezierPoint(i / 9f, p0, p1, p2, p3);
                Vector2? vector4 = (i != 9) ? new Vector2?(BraveMathCollege.CalculateBezierPoint(i / 9f, p0, p1, p2, p3)) : null;
                Vector2 a = Vector2.zero;
                if (vector != null)
                {
                    a += (Quaternion.Euler(0f, 0f, 90f) * (vector3 - vector.Value)).XY().normalized;
                }
                if (vector4 != null)
                {
                    a += (Quaternion.Euler(0f, 0f, 90f) * (vector4.Value - vector3)).XY().normalized;
                }
                a = a.normalized;
                float d = Mathf.Lerp(StartWidth, EndWidth, i / 9f);
                vector3 += vector2.normalized * Mathf.Sin(Time.realtimeSinceStartup * SinSpeed + i * WavelengthMod) * AmplitudeMod * (i / 9f);
                vertices[i * 2] = (vector3 + a * d).ToVector3ZisY(zOffset);
                vertices[i * 2 + 1] = (vector3 + -a * d).ToVector3ZisY(zOffset);
                vector = new Vector2?(vector3);
            }
        }
    }

}
