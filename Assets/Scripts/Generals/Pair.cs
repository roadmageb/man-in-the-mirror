using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<L, R>
{
    public L l;
    public R r;

    public Pair(L _l, R _r)
    {
        this.l = _l;
        this.r = _r;
    }

    public Pair<R, L> Swap()
    {
        return new Pair<R, L>(r, l);
    }
}
