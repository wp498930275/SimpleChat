using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{

    protected GameFacade mFacade;

    protected BaseManager(GameFacade facade)
    {
        mFacade = facade;
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public virtual void Update()
    {
        
    }
    
}
