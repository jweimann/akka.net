using Akka.Actor;
using Akka.Configuration;
using RTS.Commands;
using RTS.Commands.Buildings;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
using RTS.Commands.Team;
using RTS.Commands.Units;
using RTS.Commands.Weapons;
using RTS.Core.Structs;
using RTS.Entities;
using RTS.Entities.Client;
using RTS.Entities.Factories;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Stats;
using RTS.Entities.Movement;
using RTS.Entities.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MmoHost
{
    class Program
    {
        private Program()
        {
            //InitializeMEF();
            //DoTempSetup(null);
        }
        static Config GetConfig(int port)
        {
            if (port == 2525)
            {
                return ConfigurationFactory.ParseString(@"
akka {  
    log-config-on-start = on
    stdout-loglevel = DEBUG
    loglevel = ERROR
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        
        debug {  
          receive = off 
          autoreceive = on
          lifecycle = on
          event-stream = on
          unhandled = on
        }
    }

    remote {
        #this is the new upcoming remoting support, which enables multiple transports
       helios.tcp {
            transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
		    applied-adapters = []
		    transport-protocol = tcp
		    port = 2525
		    hostname = localhost
        }
        log-remote-lifecycle-events = INFO
    }
}
");
            }
            else
            {
                return ConfigurationFactory.ParseString(@"
akka {  
    log-config-on-start = on
    stdout-loglevel = DEBUG
    loglevel = ERROR
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        
        debug {  
          receive = off 
          autoreceive = on
          lifecycle = on
          event-stream = on
          unhandled = on
        }
    }

    remote {
        #this is the new upcoming remoting support, which enables multiple transports
       helios.tcp {
            transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
		    applied-adapters = []
		    transport-protocol = tcp
		    port = 2020
		    hostname = localhost
        }
        log-remote-lifecycle-events = INFO
    }
}
");
            }
        }

        private CompositionContainer _container;
        private void InitializeMEF()
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Entity).Assembly));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        static void StartSingleServer()
        {
            var config = GetConfig(2020);
            using (var system = ActorSystem.Create("MyServer", config))
            {
                DoTempSetup(system);
                return;
            }
        }

        static void Main(string[] args)
        {
            InitializeSerializer();
            StartSingleServer();
            //StartMultiServer();
        }

        private static void StartMultiServer()
        {
            bool spawnClient = false;
            bool spawnBots = false;
            Console.WriteLine("Server #\r\n1. Clients Proxies/Areas of Interest\r\n2. Entities/Bots");

            spawnClient = false;
            spawnBots = false;

            while (!spawnClient && !spawnBots)
            {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.NumPad1 || key == ConsoleKey.D1)
                {
                    spawnClient = true;
                    break;
                }
                else if (key == ConsoleKey.NumPad2 || key == ConsoleKey.D2)
                {
                    spawnBots = true;
                    break;
                }
            }
            //Console.WriteLine("HandleBots (Y/N)");
            //if (Console.ReadKey().Key == ConsoleKey.Y)
            //{
            //    spawnBots = true;
            //}

            var config = GetConfig(spawnClient ? 2020 : 2525);

            using (var system = ActorSystem.Create("MyServer", config))
            {
                //DoTempSetup(system);
                //return;
                //var physicsProxy = system.ActorOf<PhysicsZone>("PhysicsZone0");
                //var server = system.ActorOf<GroundPathingActor>("GroundPathingServer");


                if (spawnClient)
                {
                    system.ActorOf<ClientProxyListenerActor>("ClientProxyListener");
                }
                if (spawnBots)
                {
                    //system.ActorOf<AreaOfInterestCollectionActor>("AreaOfInterestCollection");
                    //system.ActorOf<EntitySpawner>("EntitySpawner1");
                    //system.ActorOf<EntitySpawner>("EntitySpawner2");
                }
                //system.ActorSelection("[akka://MyServer/user/PhysicsZone0").Anchor;

                //var entityDebugger = (ActorRefWithCell)system.ActorOf<EntityDebugger>("Debugger");
                //var debuggerStatusRequest = new DebuggerStatusRequest();

                //system.Scheduler.Schedule(TimeSpan.FromSeconds(0),
                //                              TimeSpan.FromMilliseconds(1000),
                //                              entityDebugger,
                //                              debuggerStatusRequest);





                /*
                for (int i = 0; i < 100; i++)
                {
                    if (spawnBots)
                    {
                        var chatClient = system.ActorOf(Props.Create(() => new GroundPathingActor(entityDebugger, areaOfInterestCollectionActor)), "GroundPathingActor_" + i.ToString());
                    }

                    //var updateRequest = new UpdateRequest { };

                    //system.Scheduler.Schedule(TimeSpan.FromSeconds(0),
                    //                          TimeSpan.FromMilliseconds(100),
                    //                          chatClient,
                    //                          updateRequest);
                    //System.Threading.Thread.Sleep(30000);
                }
                 */

                //var tmp = system.ActorSelection("akka.tcp://MyServer@localhost:8081/user/ChatServer");
                //tmp.Tell(new MoveRequest { Direction = "Down", Location = "0,0" });



                //while (true)
                //{
                //    Console.WriteLine(String.Format("Move Count: {0}  Turn Count: {1}", GroundPathingActor.MovedCount, GroundPathingActor.TurnedCount));
                //    GroundPathingActor.MovedCount = 0;
                //    GroundPathingActor.TurnedCount = 0;
                //    System.Threading.Thread.Sleep(1000);
                //}

                Console.ReadLine();
            }
        }

        private static void InitializeSerializer()
        {
            var serializerTypes = new List<Type>() { 
                typeof(Vector3)
                //, 
                //typeof(MoveCommand), 
                //typeof(MmoCommand), 
                //typeof(DamageEntityCommand),
                //typeof(UpdateStatsCommand),
                //typeof(SpawnEntityCommand),
                //typeof(SetDestinationCommand),
                //typeof(BuildEntityCommand),
                //typeof(MoveUnitsCommand),
                //typeof(SetPathOnClientCommand),
                //typeof(SetTargetCommand),
                //typeof(DestroyEntityCommand),
                //typeof(FireWeaponCommand),
                //typeof(SetTeamCommand)
            };

            var assembly = Assembly.LoadFrom("RTS.Commands.dll");
            IEnumerable<Type> myTypes = assembly.GetTypes().Where(t => t.FullName.EndsWith("Command") && t.IsSerializable == true && typeof(IMmoCommand).IsAssignableFrom(t));

            foreach (var t in myTypes)
            {
                Console.WriteLine("Registered Command " + t.ToString());
            }

            serializerTypes.AddRange(myTypes);

            NetSerializer.Serializer.Initialize(serializerTypes);
        }

        private static void DoTempSetup(ActorSystem system)
        {
            //system.ActorOf<AreaOfInterestCollectionActor>("AreaOfInterestCollection");
            system.ActorOf<ClientProxyListenerActor>("ClientProxyListener");

            //NPCEntityFactory npcFactory = new NPCEntityFactory(system);

            for (int i = 0; i < 0; i++)
            {
           //     npcFactory.GetEntity();
            }
            

           // var entity = system.ActorOf<Entity>();
            Console.ReadLine();
        }
    }

}
