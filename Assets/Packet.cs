using UnityEngine;
using DG.Tweening;

public class Packet : MonoBehaviour {

    public void MoveThenPool(Vector3 from, Vector3 to, float time, PacketPooling pool) {
        transform.position = from;
        transform.DOMove(to, time).OnComplete(() => {
            pool.EnqueuePacket(this);
        });
    }

}
