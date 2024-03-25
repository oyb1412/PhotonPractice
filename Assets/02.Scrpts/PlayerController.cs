using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {

    public Rigidbody2D Rigid;
    public Animator Anime;
    public SpriteRenderer Spriter;
    public PhotonView View;
    public Text NickNameText;
    public Image HealthImage;
    public 
    bool isGround;
    Vector3 curPos;

    private void Awake() {
        NickNameText.text = View.IsMine ? PhotonNetwork.NickName : View.Owner.NickName;
        NickNameText.color = View.IsMine ? Color.green : Color.red;

        if(View.IsMine) {
            var cm = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
            cm.Follow = transform;
            cm.LookAt = transform;
        }
    }

    private void Update() {
        if (View.IsMine) {
            float axis = Input.GetAxisRaw("Horizontal");
            Rigid.velocity = new Vector2(axis * 4, Rigid.velocity.y);

            if (axis != 0) {
                Anime.SetBool("Walk", true);
                View.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
            } else
                Anime.SetBool("Walk", false);

            isGround = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.down * 0.5f, 0.07f, 1 << LayerMask.NameToLayer("Ground"));
            Anime.SetBool("Jump", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) {
                View.RPC("JumpRPC", RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                PhotonNetwork.Instantiate("Bullet", transform.position + new Vector3(Spriter.flipX ? -0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                    .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, Spriter.flipX ? -1 : 1);
                Anime.SetTrigger("Shot");
            }
        } else if ((transform.position - curPos).sqrMagnitude >= 100)
            transform.position = curPos;
        else
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10f);
    }
    [PunRPC]
    void JumpRPC() {
        Rigid.velocity = Vector2.zero;
        Rigid.AddForce(Vector2.up * 700);
    }

    void DestroyRPC() {
        Destroy(gameObject);
    }

    public void Hit() {
        HealthImage.fillAmount -= 0.1f;
        if(HealthImage.fillAmount <= 0) {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            View.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void FlipXRPC(float axis) => Spriter.flipX = axis == -1;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.fillAmount);
        }
        else {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
