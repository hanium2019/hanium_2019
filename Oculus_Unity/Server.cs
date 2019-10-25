using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour
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

    private string address = "";

    private Socket serverSock = null;
    private System.AsyncCallback fncAcceptHandler;
    private System.AsyncCallback fncRecvHandler;
    private System.AsyncCallback fncSendHandler;
    
    // Start is called before the first frame update
    void Start() // StartListening()
    {
        // hostInfo는 GetHostName함수로 호스트를 얻는다
        // (얻는 이유? 일반적으로는 아이피주소를 얻기 힘들기 때문에)
        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
        // 해당 호스트를 IP주소의 리스트에 넣는 것
        //IPAddress hostAddress = hostInfo.AddressList[0];
        //IPEndPoint localEP = new IPEndPoint(hostAddress, 11000);
        IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 11000);
        
        // 받아온 IP주소를 String으로 변환
        //address = hostAddress.ToString();

        fncAcceptHandler = new System.AsyncCallback(AcceptClient);
        fncRecvHandler = new System.AsyncCallback(ReceiveCallback);
        fncSendHandler = new System.AsyncCallback(Sendcallback);

        serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            serverSock.Bind(localEP); serverSock.Listen(1);
            serverSock.BeginAccept(fncAcceptHandler, serverSock);
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
        Socket listener = (Socket) ar.AsyncState;
        Socket handler = listener.EndAccept(ar);
        AsyncObject ao = new AsyncObject(4096);
        ao.workSock = handler;
        handler.BeginReceive(ao.buf, 0, ao.buf.Length, 
                SocketFlags.None, fncRecvHandler, ao);
    }

    void RecvCallback(System.IAsyncResult ar) {
        string content = string.Empty;
        AsyncObject ao = (AsyncObject) ar.AsyncState;
        int recv = ao.workSock.EndReceive(ar);
        if(recv > 0) {
            ao.sb.Append(Encoding.Unicode.GetString(ao.buf, 0, recv));
            ao.workSock.BeginReceive(ao.buf, 0, ao.buf.Length,
            SocketFlags.None, fncRecvHandler, ao);

            content = state.sb.ToString();
            if(content.IndexOf("<EOF>") > -1) {
                // 모든 데이터가 클라이언트로부터 도착
                // 클라이언트 화면에 디스플레이한다.
                SendMsg(handler, content);
            } else {
                ao.workSock.BeginReceive(ao.buf, 0, ao.buf.Length,
                SocketFlags.None, fncRecvHandler, ao);
            }
        }
    }

    // 클라이언트가 보낸 메시지 에코
    void SendMsg(Socket handler, string msg) {
        byte[] byteMsg = Encoding.Unicode.GetBytes(msg);
        handler.BeginSend(byteMsg, 0, byteMsg.Length,
            SocketFlags.None, fncSendHandler, handler);
    }

    // 메시지 에코에 대한 콜백 함수
    void SendCallback(System.IAsyncResult ar) {
        Socket handler = (Socket) ar.AsyncState;
        int sent = handler.EndSend(ar);
        // 메시지 에코에 대한 처리
        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }

    // 입력 . . .
    private Text displayed;
    private InputField newone;
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return))
            displayed.text = newone.ToString();
    }
}