using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net.Sockets; 
using System.Net;
using System.IO;

public class VideoStreamer : MonoBehaviour {

    const int port = 8010;

    public string IP = "";

    TcpClient client;

    Texture2D tex;

    void Start(){
        client = new TcpClient();
        client.Connect(IPAddress.Loopback, port);
    }

    void Update(){
        if (!client.Connected)
            return;

        var serverStream = client.GetStream();

        if (serverStream.CanRead){
            using (var writer = new MemoryStream()){
                var readBuffer = new byte[client.ReceiveBufferSize];

                while (serverStream.DataAvailable){
                    int numberOfBytesRead = serverStream.Read(readBuffer, 0, readBuffer.Length);
                    if (numberOfBytesRead <= 0)
                        break;

                    writer.Write(readBuffer, 0, numberOfBytesRead);
                }

                if (writer.Length > 0){
                    var tex = new Texture2D(0, 0);
                    tex.LoadImage(writer.ToArray());
                    image.texture = tex;
                }   
            }
        }
    }

    void OnApplicationQuit(){
        client.Close();
    }
}