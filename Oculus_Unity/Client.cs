using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public class AsyncObject 
    {
        public byte[] buf;
        public Socket workSock = null;
        public AsyncObject(int bufSize) {
            this.buf = new byte[bufSize];
        }
        public StringBuilder sb = new StringBuilder();
    }

    private Socket clientSock = null;
    private System.AsyncCallback fncSendHandler;
    private System.AsyncCallback fncRecvHandler;

    // Start is called before the first frame update
    void Start()
    {
        IPAddress hostIP = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(hostIP, 11000);

        fncSendHandler = new System.AsyncCallback(SendCallback);
        fncRecvHandler = new System.AsyncCallback(RecvCallback);

        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try 
        {
            clientSock.Connect(remoteEP);
        }

        catch (SocketException e)
        {
            Debug.Log("Connection Error: " + e.Message);
        }

        SendMsg(clientSock, "This is a test<EOF>");
        Receive(clientSock);
        clientSock.Shutdown(SocketShutdown.Both);
        clientSock.Close();
    }

    void SendMsg(Socket client, string msg) {
        byte[] byteMsg = Encoding.Unicode.GetBytes(msg);
        client.BeginSend(byteMsg, 0, byteMsg.Length,
            SocketFlags.None, fncSendHandler, client);
    }

    void SendCallback(System.IAsyncResult ar) {
        try
        {
            Socket client = (Socket) ar.AsyncState;
            int sentBytes = client.EndSend(ar);
            //Debug.Log("Send Msg: {0}", Encoding.Unicode.GetString(msgByte));
        }

        catch (Exception e) {
            Debug.Log(e.ToString());
        }

    }

    void Recv(Socket client) {
        try
        {
            AsyncObject ao = new AsyncObject();
            ao.workSock = client;
            client.BeginReceive(ao.buf, 0, ao.buf.Length,
            SocketFlags.None, fncRecvHandler, ao);
        }
        
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void RecvCallback(System.IAsyncResult ar) {
        string response = string.Empty;
        try
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;
            int recv = ao.workSock.EndReceive(ar);
            if(recv > 0) {
                ao.sb.Append(Encoding.Unicode.GetString(ao.buf, 0, recv));
                ao.workSock.BeginReceive(ao.buf, 0, ao.buf.Length,
                SocketFlags.None, fncRecvHandler, ao);
            } else {
                if(ao.sb.Length > 1) {
                    response = ao.sb.ToString();
                }

            }
        }

        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // 입력
    private Text displayed;
    private InputField newone;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            displayed.text = newone.ToString();
    }
}
