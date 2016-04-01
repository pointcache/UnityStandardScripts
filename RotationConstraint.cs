﻿using UnityEngine;
using System.Collections;
namespace USTD
{
    public class RotationConstraint : USTDBase
    {
        public Vector3 targetRotation; // The current rotation to constraint to, can be set manually if the target Transform is null
        public Transform targetTransform; // Current target transform to constrain to, can be left null for use of provided Vector3

        public bool X, Y, Z;

        private Transform _tr;
        public Transform tr { get { if (!_tr) _tr = GetComponent<Transform>(); return _tr; } }

        public override void stdUpdate()
        {
            if (targetTransform != null)
            {
                targetRotation = targetTransform.rotation.eulerAngles;
            }
            Vector3 rot = tr.rotation.eulerAngles;
            if (X)
            {
                rot.x = targetRotation.x;
            }
            if (Y)
            {
                rot.y = targetRotation.y;
            }
            if (Z)
            {
                rot.z = targetRotation.z;
            }
            tr.rotation = Quaternion.Euler(rot);
        }
    }
}