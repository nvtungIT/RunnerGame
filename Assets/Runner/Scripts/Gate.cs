using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Runner
{
    /// <summary>
    /// A class representing a Spawnable object.
    /// If a GameObject tagged "Player" collides
    /// with this object, it will trigger a fail
    /// state with the GameManager.
    /// </summary>
    public class Gate : Spawnable
    {
        const string k_PlayerTag = "Player";

        [SerializeField]
        GateType m_GateType;
        [SerializeField]
        float m_Value;
        [SerializeField]
        RectTransform m_Text;

        bool m_Applied;
        Vector3 m_TextInitialScale;

        enum GateType
        {
            ChangeSpeed,
            ChangeSize,
        }


        private float m_MoveDistance = 6f; // The distance the gate will move in each direction
       
        private float m_MoveDuration = 2f; // The time it takes for the gate to move to one side

        public enum GateStartPosition
        {
            Left,
            Right
        }

        public GateStartPosition gateStartPosition = GateStartPosition.Left;

        private bool m_IsMovingLeft = false;

        private Coroutine m_MoveCoroutine;
        private Vector3 m_DefaultPosition;

        /// <summary>
        /// Sets the local scale of this spawnable object
        /// and ensures the Text attached to this gate
        /// does not scale.
        /// </summary>
        /// <param name="scale">
        /// The scale to apply to this spawnable object.
        /// </param>
        public override void SetScale(Vector3 scale)
        {
            // Ensure the text does not get scaled
            if (m_Text != null)
            {
                float xFactor = Mathf.Min(scale.y / scale.x, 1.0f);
                float yFactor = Mathf.Min(scale.x / scale.y, 1.0f);
                m_Text.localScale = Vector3.Scale(m_TextInitialScale, new Vector3(xFactor, yFactor, 1.0f));

                m_Transform.localScale = scale;
            }
        }

        /// <summary>
        /// Reset the gate to its initial state. Called when a level
        /// is restarted by the GameManager.
        /// </summary>
        public override void ResetSpawnable()
        {
            m_Applied = false;
            StopMoving();
            ResetPosition();
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Text != null)
            {
                m_TextInitialScale = m_Text.localScale;
            }

            m_DefaultPosition = m_Transform.localPosition;
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag) && !m_Applied)
            {
                ActivateGate();
                StartMoving();
            }
        }

        void ActivateGate()
        {
            switch (m_GateType)
            {
                case GateType.ChangeSpeed:
                    PlayerController.Instance.AdjustSpeed(m_Value);
                    break;

                case GateType.ChangeSize:
                    PlayerController.Instance.AdjustScale(m_Value);
                    break;
            }

            m_Applied = true;
        }

        private void Start()
        {
            if (gateStartPosition == GateStartPosition.Left)
            {
                m_IsMovingLeft = false;
            }
            else
            {
                m_IsMovingLeft = true;
            }

            StartMoving();
        }

        private void StartMoving()
        {
            if (m_MoveCoroutine == null)
            {
                m_MoveCoroutine = StartCoroutine(MoveCoroutine());
            }
        }

        private void StopMoving()
        {
            if (m_MoveCoroutine != null)
            {
                StopCoroutine(m_MoveCoroutine);
                m_MoveCoroutine = null;
            }
        }

        private void ResetPosition()
        {
            if (gateStartPosition == GateStartPosition.Left)
            {
                m_Transform.localPosition = new Vector3(-3f, m_DefaultPosition.y, m_DefaultPosition.z);
            }
            else
            {
                m_Transform.localPosition = new Vector3(3f, m_DefaultPosition.y, m_DefaultPosition.z);
            }
        }

        private IEnumerator MoveCoroutine()
        {
            while (true)
            {
                Vector3 targetPosition;

                if (m_IsMovingLeft)
                {
                    targetPosition = m_Transform.localPosition + new Vector3(-m_MoveDistance, 0f, 0f);
                }
                else
                {
                    targetPosition = m_Transform.localPosition + new Vector3(m_MoveDistance, 0f, 0f);
                }

                float startTime = Time.time;
                float elapsedTime = 0f;

                while (elapsedTime < m_MoveDuration)
                {
                    float t = elapsedTime / m_MoveDuration;
                    m_Transform.localPosition = Vector3.Lerp(m_Transform.localPosition, targetPosition, t);
                    elapsedTime = Time.time - startTime;
                    yield return null;
                }

                m_IsMovingLeft = !m_IsMovingLeft;
            }
        }
    }
}
