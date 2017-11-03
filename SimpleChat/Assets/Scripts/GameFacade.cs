using UnityEngine;

public class GameFacade : MonoBehaviour
{
    NetworkManager mNetworkManager;

    void Start()
    {
        Init();
    }

    void Init()
    {
        Debuger.EnableTime = false;
        mNetworkManager = new NetworkManager(this);
        mNetworkManager.Connect(AppConst.IpAddress, AppConst.Port);
    }

    void OnDestroy()
    {
        mNetworkManager.OnDestroy();
    }

    void Update()
    {
        mNetworkManager.Update();
    }

    public void LoginRequest()
    {
        LoginNetMsg.LoginRequest("123", "345");
    }

    public void RegisterRequest()
    {
        LoginNetMsg.RegisterRequest("123", "456");
    }
}