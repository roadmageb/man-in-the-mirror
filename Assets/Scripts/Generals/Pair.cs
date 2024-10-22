﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair
{
    public float l;
    public float r;

    public Pair(float _l, float _r)
    {
        this.l = _l;
        this.r = _r;
        if (_l > _r) Swap();
    }

    public void Swap()
    {
        float temp = l;
        l = r;
        r = temp;
    }

    public Pair ApplyMargin(float margin)
    {
        return new Pair(l - margin, r + margin);
    }
}
