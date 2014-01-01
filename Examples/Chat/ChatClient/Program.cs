﻿using ChatMessages;
using Pigeon;
using Pigeon.Actor;
using Pigeon.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = new ActorSystem())
            {
                var chatClient = system.ActorOf<ChatClientActor>();
                chatClient.Tell(new ConnectRequest()
                {
                    Username = "Roggan",
                }, ActorRef.NoSender);

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input.StartsWith("/"))
                    {
                        var parts = input.Split(' ');
                        var cmd = parts[0].ToLowerInvariant();
                        var rest = string.Join(" ",parts.Skip(1));

                        if (cmd == "/nick")
                        {
                            chatClient.Tell(new NickRequest
                            {
                                NewUsername = rest
                            }, ActorRef.NoSender);
                        }                        
                    }
                    else
                    {
                        chatClient.Tell(new SayRequest()
                        {
                            Text = input,
                        }, ActorRef.NoSender);
                    }
                }
            }
        }
    }

    class ChatClientActor : TypedActor,
        IHandle<ConnectRequest>,
        IHandle<ConnectResponse>,
        IHandle<NickRequest>,
        IHandle<NickResponse>,
        IHandle<SayRequest>,
        IHandle<SayResponse>
    {
        private string nick = "Roggan";
        private ActorRef server;

        public ChatClientActor()
        {
            server = Context.ActorSelection("http://localhost:8090/ChatServer");
        }        
        
        public void Handle(ConnectResponse message)
        {
            Console.WriteLine("Connected!");
            Console.WriteLine(message.Message);
        }

        public void Handle(NickRequest message)
        {
            message.OldUsername = this.nick;
            Console.WriteLine("Changing nick to {0}", message.NewUsername);
            this.nick = message.NewUsername;
            server.Tell(message,Self);
        }

        public void Handle(NickResponse message)
        {
            Console.WriteLine("{0} is now known as {1}", message.OldUsername, message.NewUsername);
        }

        public void Handle(SayResponse message)
        {
            Console.WriteLine("{0}: {1}", message.Username, message.Text);
        }

        public void Handle(ConnectRequest message)
        {
            Console.WriteLine("Connecting....");
            server.Tell(message,Self);
        }

        public void Handle(SayRequest message)
        {
            message.Username = this.nick;
            server.Tell(message,Self);
        }
    }
}