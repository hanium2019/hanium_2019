using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : MonoBehaviour
{
    /*private State state;

    private enum State {
        StartConnecting = 0,
        CliecntCommunication,
    }*/

    public class AsyncObject 
    {
        public byte[] buf;
        public Socket workSock = null;
        public AsyncObject(int bufSize) {
            this.buf = new byte[bufSize];
        }
    }

    private bool g_connected;

    private Socket clientSock = null;
    private System.AsyncCallback fncConnectHandler;
    private System.AsyncCallback fncReceiveHandler;
    private System.AsyncCallback fncSendHandler;


    // Start is called before the first frame update
    void Start()
    {
        fncReceiveHandler = new System.AsyncCallback(ReceiveData);
        fncSendHandler = new System.AsyncCallback(SendData);

        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress hostIP = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(hostIP, 11000);

        bool isConnected = false;

        try 
        {
            clientSock.Connect(remoteEP);
            isConnected = true;
        }

        catch (SocketException e)
        {
            Debug.Log("Connection Error: " + e.Message);
            isConnected = false;
        }

        g_connected = isConnected;

        if(isConnected) {
            AsyncObject state = new AsyncObject(4096);
            state.workSock = clientSock;
            clientSock.BeginReceive(state.buf, 0, state.buf.Length,
                SocketFlags.None, fncReceiveHandler, state);
            Debug.Log("Connection Success!");
        }
        else Debug.Log("Connection Failed!");
    }

    void SendMsg(string msg) {
        AsyncObject state = new AsyncObject(1);
        state.buf = Encoding.Unicode.GetBytes(msg);
        state.workSock = clientSock;
        clientSock.BeginSend(state.buf, 0, state.buf.Length,
            SocketFlags.None, fncSendHandler, state);
    }

    void ReceiveData(System.IAsyncResult ar) {
        AsyncObject state = (AsyncObject) ar.AsyncState;
        int recvBytes = state.workSock.EndReceive(ar);
        if(recvBytes > 0) {
            byte[] msgByte = new byte[recvBytes];
            System.Array.Copy(state.buf, msgByte, recvBytes);
            //Debug.Log("Received Msg: {0}", Encoding.Unicode.GetString(msgByte));
        }
        state.workSock.BeginReceive(state.buf, 0, state.buf.Length,
            SocketFlags.None, fncReceiveHandler, state);
    }

    void SendData(System.IAsyncResult ar) {
        AsyncObject state = (AsyncObject) ar.AsyncState;
        int sentBytes = state.workSock.EndSend(ar);
        if(sentBytes > 0) {
            byte[] msgByte = new byte[sentBytes];
            System.Array.Copy(state.buf, msgByte, sentBytes);
            //Debug.Log("Send Msg: {0}", Encoding.Unicode.GetString(msgByte));
        }
    }

    void StopConnecting() {
        if(clientSock != null) {
            clientSock.Close();
            clientSock = null;
        }
        Debug.Log("End client communication");
    }
}
