using System.Collections.Generic;
using UnityEngine;

public class PacketPooling : MonoBehaviour {

    [SerializeField]
    private GameObject packetPrefab;
    private Queue<Packet> packets = new Queue<Packet>();

    public Packet GetPacket() {
        Packet p;

        if (packets.Count == 0) {
            p = Instantiate(packetPrefab, transform).GetComponent<Packet>();
        } else {
            p = packets.Dequeue();

            p.gameObject.SetActive(true);
        }

        return p;
    }

    public void EnqueuePacket(Packet p) {
        p.gameObject.SetActive(false);
        p.transform.parent = transform;

        packets.Enqueue(p);
    }
}
