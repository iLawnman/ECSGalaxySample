using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scaffold
{
    [Serializable]
    public class SceneConfig
    {
        public List<EntityConfig> entities = new List<EntityConfig>();
    }

    [Serializable]
    public class EntityConfig
    {
        public string prefabPath;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public SerializableVector3 scale = new SerializableVector3(1f, 1f, 1f);
    }

    [Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(SerializableVector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator SerializableVector3(Vector3 v)
        {
            return new SerializableVector3(v.x, v.y, v.z);
        }
    }

    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Quaternion(SerializableQuaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator SerializableQuaternion(Quaternion q)
        {
            return new SerializableQuaternion(q.x, q.y, q.z, q.w);
        }
    }
}