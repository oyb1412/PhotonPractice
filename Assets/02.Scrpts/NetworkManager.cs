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
        //�ػ� x, �ػ� y, ��üȭ�� Ȱ��ȭ ����
        Screen.SetResolution(960, 540, false);

        //�ʴ� Send Ƚ��
        PhotonNetwork.SendRate = 60;

        //�ʴ� ��Ŷ ����ȭ Ƚ��
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() {
        //���� Ŭ���忡 ����
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// PhotonNetwork.ConnectUsingSettings()���� ���� �ڵ����� ȣ���.
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
        //���ҽ� �������� ����.
        PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-6f,19f),4f,0), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }

    /// <summary>
    /// PhotonNetwork.Disconnect()�Լ��� ����Ǹ� ȣ���
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause) {
        DisconectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }

    /// <summary>
    /// PhotonNetwork.JoinOrCreateRoom�Լ��� ����Ǹ� ȣ���
    /// </summary>
    public override void OnJoinedRoom() {
        DisconectPanel.SetActive(false);
        StartCoroutine(DestroyBullet());
        Spawn();

    }
}
