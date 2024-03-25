using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviourPunCallbacks
{
    public PhotonView View;
    int dir;
    
    void Start()
    {
        Destroy(gameObject, 3.5f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * 7 * Time.deltaTime * dir);
    }

    [PunRPC]
    void DirRPC(int dir) => this.dir = dir;

    [PunRPC]
    void DestroyRPC() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Ground")) {
            View.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

        if(!View.IsMine && collision.CompareTag("Player") && collision.GetComponent<PhotonView>().IsMine) {
            collision.GetComponent<PlayerController>().Hit();
            View.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
}
