using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

[System.Serializable]
public class BaseSerializable {}

[System.Serializable]
public class Vec3 : BaseSerializable {
    public float x;
    public float y;
    public float z;

    public Vec3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Vec3 from(Vector3 v) {
        return new Vec3(v.x, v.y, v.z);
    }
}

[System.Serializable]
public class ResponseBase {
    public string status;
    public int time;
}

[System.Serializable]
public class CreateSessionResponse : ResponseBase {
    public ApiData data;
}

[System.Serializable]
public class ApiData {
    public string session;
}