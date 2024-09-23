using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using Caro.Models.Enums;
using Caro.Models.Network;
using Caro.Models.Player;

namespace Caro.Models.GameStrategy
{
    public class LANGameStrategy : IGameStrategy
    {
        private ClientHandler?  clientHandler;
        private TcpClient       tcpClient;
        private bool    _isConnectingToServer       = true;
        private bool    _readyToStart               = false;
        private bool    _isWaitingForOtherPlayer    = false;

        private const string SERVER_IP_ADDRESS  = "127.0.0.1";
        private const int    SERVER_PORT        = 5000;

        private bool _isServerAcceptJoinRequest = false;

        public LANGameStrategy(Board board) : base(board)
        {
            _player2.Name   = "👤 LAN Player";
            tcpClient       = new TcpClient();

            // Thread to connect to server
            Task.Run(async () => await ConnectToServer());
        }

        private async Task ConnectToServer()
        {
            // Check if the client is connected to server
            bool isConnected = false;

            while (!isConnected)
            {
                try
                {
                    await tcpClient.ConnectAsync(SERVER_IP_ADDRESS, SERVER_PORT);
                    Console.WriteLine($"Connected to server successfully on port {SERVER_PORT}");
                    isConnected = true; 

                    _isConnectingToServer   = false;
                    clientHandler           = new ClientHandler(tcpClient);
                    //clientHandler.SendMessage("<JOIN>");
                    clientHandler.ReceivedMessage += ReceivedMessage_Event;
                    await clientHandler.ReadMessageAsync(); 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to server: {ex.Message}");
                    await Task.Delay(2000); 
                }
            }
        }

        public override void SendJoinRequestToServer(int boardRatio = 9)
        {
            Task.Run(() => {
                while(true)
                {
                    Thread.Sleep(200);
                    if (clientHandler == null) 
                        continue;

                    clientHandler.SendMessage($"<JOIN> {boardRatio}");
                    Console.WriteLine("Sent join request to server");
                    break;
                }
            });
        }

        public override void MakeNewRound()
        {
            if (_board.IsWin())
            {
                if (_currentPlayer == _player2)
                {
                    _player1.Score++;
                    if (_player1.Piece == CellState.X)
                    {
                        _player1.SetPiece(CellState.O);
                        _player2.SetPiece(CellState.X);
                        //_isWaitingForOtherPlayer = true;
                    }
                }
                else
                {
                    _player2.Score++;
                    if (_player2.Piece == CellState.X)
                    {
                        _player1.SetPiece(CellState.X);
                        _player2.SetPiece(CellState.O);
                        //_isWaitingForOtherPlayer = false;
                    }
                }
            }

            CurPiece = true;
        }

        public override void DoPlayAt(Board board, FPoint pos)
        {
            if (_isWaitingForOtherPlayer) 
                return;

            if (clientHandler != null) 
            {
                clientHandler.SendMessage($"<MOVE> {pos.X} {pos.Y}");
                _isWaitingForOtherPlayer = true;
            }
        }

        public override void Undo(Board board)
        {
            return;
        }

        public override bool IsConnectingToServer()
        {
            return _isConnectingToServer;
        }

        public override bool IsReadyToStart()
        {
            return _readyToStart;
        }

        private void ReceivedMessage_Event(object? sender, string msg)
        {

            string[] parts = msg.Split(' ');

            if (parts.Length == 0)
                return;

            string command = parts[0];
            switch(command)
            {
                case "<ACCEPT_JOIN>":
                    clientHandler?.SendMessage("<GOOD_CONNECT>");
                    _isServerAcceptJoinRequest = true;
                    break;
                case "<REJECT_JOIN>":
                    break;
                case "<JOIN_AGAIN>":
                    SendJoinRequestToServer();
                    break;
                case "<START>": 
                    if (!_isServerAcceptJoinRequest) 
                        return;
                    Start(parts);
                    break;

                case "<ACCEPT_MOVE>":
                    if (!_isServerAcceptJoinRequest) 
                        return;
                    int x1 = int.Parse(parts[1]);
                    int y1 = int.Parse(parts[2]);
                    _player1.MakeMove(new FPoint(x1, y1));
                    break;

                case "<MOVE>": 
                    if (!_isServerAcceptJoinRequest) 
                        return;
                    int x2 = int.Parse(parts[1]);
                    int y2 = int.Parse(parts[2]);
                    _player2.MakeMove(new FPoint(x2, y2));
                    _isWaitingForOtherPlayer = false;
                    break;

                case "<QUIT>": 
                    if (!_isServerAcceptJoinRequest) 
                        return;
                    _readyToStart = false;
                    PlayerDisconnected?.Invoke(this, EventArgs.Empty);
                    break;

                case "<PAUSE>":
                    if (!_isServerAcceptJoinRequest) 
                        return;
                    break;

                default: 
                    break;
            }
        }

        private void Start(string[] msg)
        {
            _readyToStart = true;
            if (msg[1] == "1")
            {
                _isWaitingForOtherPlayer = false;
                _currentPlayer  = _player1;
                _player1.SetPiece(CellState.X);
                _player2.SetPiece(CellState.O);
            }
            else 
            {
                _isWaitingForOtherPlayer = true;
                _currentPlayer  = _player2;
                _player1.SetPiece(CellState.O);
                _player2.SetPiece(CellState.X);
            }
        }

        public override void DisconnectedToServer()
        {
            if (clientHandler != null)
                clientHandler.Disconnected();
                
            tcpClient.Close();
        }

    }
}