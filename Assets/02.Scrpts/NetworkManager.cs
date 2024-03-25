using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconectPanel;
    public GameObject RespawnPanel;

    private void Awake() {
        //해상도 x, 해상도 y, 전체화면 활성화 유무
        Screen.SetResolution(960, 540, false);

        //초당 Send 횟수
        PhotonNetwork.SendRate = 60;

        //초당 패킷 직렬화 횟수
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() {
        //포톤 클라우드에 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// PhotonNetwork.ConnectUsingSettings()으로 인해 자동으로 호출됨.
    /// </summary>
    public override void OnConnectedToMaster() {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }
    }

    IEnumerator DestroyBullet() {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Bullet"))
            go.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
    }

    public void Spawn() {
        //리소스 폴더에서 생성.
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f,19f),4f,0), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    /// <summary>
    /// PhotonNetwork.Disconnect()함수가 실행되면 호출됨
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause) {
        DisconectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }

    /// <summary>
    /// PhotonNetwork.JoinOrCreateRoom함수가 실행되면 호출됨
    /// </summary>
    public override void OnJoinedRoom() {
        DisconectPanel.SetActive(false);
        StartCoroutine(DestroyBullet());
        Spawn();

    }
}
