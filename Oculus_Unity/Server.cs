using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    /*private State state;

    private enum State
    {
        AcceptClient = 0,
        ReceiveData,
        SendData,
        StopListening,
    }*/

    public class AsyncObject 
    {
        public byte[] buf;
        public Socket workSock = null;
        public AsyncObject(int bufSize) {
            this.buf = new byte[bufSize];
        }
    }

    private string address = "";

    private Socket serverSock = null;
    private System.AsyncCallback fncAcceptHandler;
    private System.AsyncCallback fncReceiveHandler;
    private System.AsyncCallback fncSendHandler;
    
    // Start is called before the first frame update
    void Start() // StartListening()
    {
        // hostInfo는 GetHostName함수로 호스트를 얻는다
        // (얻는 이유? 일반적으로는 아이피주소를     얻기 힘들기 때문에)
        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
        // 해당 호스트를 IP주소의 리스트에 넣는 것
        IPAddress hostAddress = hostInfo.AddressList[0];
        IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 11000);
        //IPEndPoint localEP = new IPEndPoint(hostInfo.AddressList[0], 11000);
        
        // 받아온 IP주소를 String으로 변환
        address = hostAddress.ToString();

        fncAcceptHandler = new System.AsyncCallback(AcceptClient);
        fncReceiveHandler = new System.AsyncCallback(ReceiveData);
        fncSendHandler = new System.AsyncCallback(SendData);

        try
        {
            serverSock = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);
            serverSock.Bind(localEP); serverSock.Listen(1);
            serverSock.BeginAccept(fncAcceptHandler, null);
        }

        catch (SocketException e)
        {
            Debug.Log("Socket Error: " + e.Message);
        }
    }

    // 클라이언트의 접속 요청을 받아들이기 위한 함수
    void AcceptClient(System.IAsyncResult ar) {
        // 클라이언트가 접속을 요청하면 Accept 함수는 요청한 클라이언트와 통신하기 위해
        // Socket 클래스의 인스턴스를 반환한다.
        Socket sockClient = serverSock.EndAccept(ar);
        AsyncObject state = new AsyncObject(4096);
        state.workSock = sockClient;
        sockClient.BeginReceive(state.buf, 0, state.buf.Length, 
                SocketFlags.None, fncReceiveHandler, state);
    }

    void SendMsg(string msg) {
        AsyncObject state = new AsyncObject(1);
        state.buf = Encoding.Unicode.GetBytes(msg);
        state.workSock = serverSock;
        serverSock.BeginSend(state.buf, 0, state.buf.Length,
            SocketFlags.None, fncSendHandler, state);
    }

    void ReceiveData(System.IAsyncResult ar) {
        AsyncObject state = (AsyncObject) ar.AsyncState;
        int recvBytes = state.workSock.EndReceive(ar);
        if(recvBytes > 0) {
            //Debug.Log("Received Msg: {0}", Encoding.Unicode.GetString(state.buf));
        }
        state.workSock.BeginReceive(state.buf, 0, state.buf.Length,
            SocketFlags.None, fncReceiveHandler, state);
        /*byte[] buf = new byte[1400];
        int recvSize = serverSock.BeginReceive(buf, 0, buf.Length, SocketFlags.None);
        if(recvSize > 0) {
            string msg = Encoding.UTF8.GetString(buf);
            Debug.Log(msg);*/
    }

    void SendData(System.IAsyncResult ar) {
        AsyncObject state = (AsyncObject) ar.AsyncState;
        int sentbytes = state.workSock.EndSend(ar);
        if(sentbytes > 0) {
            //Debug.Log("Sent Msg: {0}", Encoding.Unicode.GetString(state.buf));
        }
    }

    void StopListening() {
        if(serverSock != null) {
            serverSock.Close();
            serverSock = null;
        }
        Debug.Log("End server communication");
    }
}